using System.Security.Cryptography;

namespace Simple.PasswordGenerator;

/// <summary>
/// Defines a service for generating secure passwords based on a configurable policy.
/// </summary>
public interface IPasswordGenerator
{
    string Generate(Action<PasswordPolicy>? policyConfiguration = null);
    string Generate(PasswordPolicy policyConfiguration);
    PasswordStrengthResult GenerateWithStrength(Action<PasswordPolicy>? policyConfiguration = null);
}

/// <summary>
/// Provides functionality to generate cryptographically secure passwords based on customizable policies.
/// </summary>
public class PasswordGenerator : IPasswordGenerator
{
    private const string UpperCaseSeed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string LowerCaseSeed = "abcdefghijklmnopqrstuvwxyz";
    private const string DigitSeed = "0123456789";

    public string Generate(Action<PasswordPolicy>? policyConfiguration = null)
    {
        try
        {
            var policy = new PasswordPolicy();
            policyConfiguration?.Invoke(policy);
            policy.Validate();

            var charCategories = new List<string>();
            var requiredChars = new List<char>();

            var upper = policy.RequireUppercase
                ? FilterAmbiguous(UpperCaseSeed, policy)
                : string.Empty;

            var lower = policy.RequireLowercase
                ? FilterAmbiguous(LowerCaseSeed, policy)
                : string.Empty;

            var digits = policy.RequireDigit
                ? FilterAmbiguous(DigitSeed, policy)
                : string.Empty;

            var special = policy.RequireSpecial
                ? FilterAmbiguous(policy.SpecialCharacters, policy)
                : string.Empty;

            var additional = FilterAmbiguous(policy.AdditionalCharacters, policy);

            if (policy.RequireUppercase && upper.Length > 0)
            {
                charCategories.Add(upper);
                requiredChars.Add(GetRandomChar(upper));
            }

            if (policy.RequireLowercase && lower.Length > 0)
            {
                charCategories.Add(lower);
                requiredChars.Add(GetRandomChar(lower));
            }

            if (policy.RequireDigit && digits.Length > 0)
            {
                charCategories.Add(digits);
                requiredChars.Add(GetRandomChar(digits));
            }

            if (policy.RequireSpecial && special.Length > 0)
            {
                charCategories.Add(special);
                requiredChars.Add(GetRandomChar(special));
            }

            if (!string.IsNullOrEmpty(additional))
            {
                charCategories.Add(additional);
                // Not adding to requiredChars because these are optional characters
            }

            var allChars = string.Concat(charCategories);
            if (string.IsNullOrEmpty(allChars))
                throw new PasswordGeneratorException("No valid characters available for password generation.");

            var remainingLength = policy.Length - requiredChars.Count;
            var passwordChars = new List<char>(requiredChars);

            for (int i = 0; i < remainingLength; i++)
            {
                passwordChars.Add(GetRandomChar(allChars));
            }

            Shuffle(passwordChars);

            return new string(passwordChars.ToArray());
        }
        catch (Exception e) when (e is not PasswordGeneratorException)
        {
            throw new PasswordGeneratorException("An error occurred while generating password. See inner exception for further details.", e);
        }
    }

    public string Generate(PasswordPolicy policyConfiguration)
    {
        ArgumentNullException.ThrowIfNull(policyConfiguration);

        return Generate(config =>
        {
            config.Length = policyConfiguration.Length;
            config.RequireUppercase = policyConfiguration.RequireUppercase;
            config.RequireLowercase = policyConfiguration.RequireLowercase;
            config.RequireDigit = policyConfiguration.RequireDigit;
            config.RequireSpecial = policyConfiguration.RequireSpecial;
            config.SpecialCharacters = policyConfiguration.SpecialCharacters;
            config.ExcludeAmbiguousCharacters = policyConfiguration.ExcludeAmbiguousCharacters;
            config.AmbiguousCharacters = policyConfiguration.AmbiguousCharacters;
            config.AdditionalCharacters = policyConfiguration.AdditionalCharacters;
        });
    }

    /// <summary>
    /// Generates a password based on an optional policy configuration and evaluates its strength.
    /// Calculates password entropy using the size of the character set and password length,
    /// then categorizes the password strength from VeryWeak to VeryStrong based on entropy thresholds.
    /// Returns the generated password along with its entropy value and strength rating.
    /// Throws a PasswordGeneratorException if no valid characters are available or if an error occurs.
    /// </summary>
    public PasswordStrengthResult GenerateWithStrength(Action<PasswordPolicy>? policyConfiguration = null)
    {
        try
        {
            var policy = new PasswordPolicy();
            policyConfiguration?.Invoke(policy);
            policy.Validate();

            var password = Generate(policy);
            var charsetSize = 0;

            if (policy.RequireUppercase)
            {
                charsetSize += FilterAmbiguous(UpperCaseSeed, policy).Length;
            }

            if (policy.RequireLowercase)
            {
                charsetSize += FilterAmbiguous(LowerCaseSeed, policy).Length;
            }

            if (policy.RequireDigit)
            {
                charsetSize += FilterAmbiguous(DigitSeed, policy).Length;
            }

            if (policy.RequireSpecial)
            {
                charsetSize += FilterAmbiguous(policy.SpecialCharacters, policy).Length;
            }

            if (!string.IsNullOrEmpty(policy.AdditionalCharacters))
            {
                charsetSize += FilterAmbiguous(policy.AdditionalCharacters, policy).Length;
            }

            if (charsetSize == 0)
            {
                throw new PasswordGeneratorException("No valid characters available for password generation.");
            }

            // Entropy = length * log2(charsetSize)
            var entropy = password.Length * Math.Log2(charsetSize);

            // Determine strength based on entropy (can be adjusted as needed)
            var strength = entropy switch
            {
                < 28 => PasswordStrength.VeryWeak,
                < 36 => PasswordStrength.Weak,
                < 60 => PasswordStrength.Moderate,
                < 128 => PasswordStrength.Strong,
                _ => PasswordStrength.VeryStrong
            };

            return new PasswordStrengthResult
            {
                Password = password,
                EntropyBits = entropy,
                Strength = strength
            };
        }
        catch (Exception e) when (e is not PasswordGeneratorException)
        {
            throw new PasswordGeneratorException("An error occurred while generating password. See inner exception for further details.", e);
        }
    }

    private static char GetRandomChar(string chars)
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes, 0);
        return chars[(int)(value % chars.Length)];
    }

    private static void Shuffle(List<char> list)
    {
        using var rng = RandomNumberGenerator.Create();
        Span<byte> buffer = stackalloc byte[4];

        for (var i = list.Count - 1; i > 0; i--)
        {
            rng.GetBytes(buffer);
            var j = (int)(BitConverter.ToUInt32(buffer) % (i + 1));
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static string FilterAmbiguous(string input, PasswordPolicy policy)
    {
        if (!policy.ExcludeAmbiguousCharacters || string.IsNullOrEmpty(policy.AmbiguousCharacters))
            return input;

        var toExclude = policy.AmbiguousCharacters.ToHashSet();
        return new string(input.Where(c => !toExclude.Contains(c)).ToArray());
    }
}

using System.Security.Cryptography;

namespace Simple.PasswordGenerator;

/// <summary>
/// Defines a service for generating secure passwords based on a configurable policy.
/// </summary>
public interface IPasswordGenerator
{
    /// <summary>
    /// Generates a password using the specified password policy.
    /// </summary>
    /// <param name="policyConfiguration">
    /// An optional delegate to configure the password policy before generation.
    /// If null, the default policy will be used.
    /// </param>
    /// <returns>A randomly generated password that satisfies the configured policy.</returns>
    string Generate(Action<PasswordPolicy>? policyConfiguration = null);
    /// <summary>
    /// Generates a password based on the specified password policy configuration.
    /// </summary>
    /// <param name="policyConfiguration">The password policy settings used to customize the generated password.</param>
    /// <returns>A randomly generated password that conforms to the given policy.</returns>
    string Generate(PasswordPolicy policyConfiguration);
}

/// <summary>
/// Provides functionality to generate cryptographically secure passwords based on customizable policies.
/// </summary>
public class PasswordGenerator : IPasswordGenerator
{
    /// <summary>
    /// The character set for uppercase letters (A–Z).
    /// </summary>
    private const string UpperCaseSeed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// The character set for lowercase letters (a–z).
    /// </summary>
    private const string LowerCaseSeed = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// The character set for digits (0–9).
    /// </summary>
    private const string DigitSeed = "0123456789";

    /// <inheritdoc />
    public string Generate(Action<PasswordPolicy>? policyConfiguration = null)
    {
        var policy = new PasswordPolicy();
        policyConfiguration?.Invoke(policy);
        policy.Validate();

        var charCategories = new List<string>();
        var requiredChars = new List<char>();

        if (policy.RequireUppercase)
        {
            charCategories.Add(UpperCaseSeed);
            requiredChars.Add(GetRandomChar(UpperCaseSeed));
        }

        if (policy.RequireLowercase)
        {
            charCategories.Add(LowerCaseSeed);
            requiredChars.Add(GetRandomChar(LowerCaseSeed));
        }

        if (policy.RequireDigit)
        {
            charCategories.Add(DigitSeed);
            requiredChars.Add(GetRandomChar(DigitSeed));
        }

        if (policy.RequireSpecial && !string.IsNullOrEmpty(policy.SpecialCharacters))
        {
            charCategories.Add(policy.SpecialCharacters);
            requiredChars.Add(GetRandomChar(policy.SpecialCharacters));
        }

        var allChars = string.Concat(charCategories);
        var remainingLength = policy.Length - requiredChars.Count;

        var passwordChars = new List<char>(requiredChars);

        for (int i = 0; i < remainingLength; i++)
        {
            passwordChars.Add(GetRandomChar(allChars));
        }

        Shuffle(passwordChars);

        return new string(passwordChars.ToArray());
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
        });
    }

    /// <summary>
    /// Selects a random character from the given character set.
    /// </summary>
    /// <param name="chars">The character set to choose from.</param>
    /// <returns>A randomly selected character.</returns>
    private static char GetRandomChar(string chars)
    {
        var bytes = new byte[4];
        RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes, 0);
        return chars[(int)(value % chars.Length)];
    }

    /// <summary>
    /// Randomly shuffles a list of characters using the Fisher–Yates algorithm and cryptographic RNG.
    /// </summary>
    /// <param name="list">The list of characters to shuffle.</param>
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
}

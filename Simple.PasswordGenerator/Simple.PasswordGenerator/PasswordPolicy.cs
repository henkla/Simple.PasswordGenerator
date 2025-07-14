namespace Simple.PasswordGenerator;

/// <summary>
/// Represents a policy used for generating secure passwords.
/// </summary>
public class PasswordPolicy
{
    /// <summary>
    /// Gets or sets the total length of the generated password.
    /// Must be at least 12 characters.
    /// Default value: <c>16</c>.
    /// </summary>
    public int Length { get; set; } = 16;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one uppercase letter (A–Z).
    /// Default value: <c>true</c>.
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one lowercase letter (a–z).
    /// Default value: <c>true</c>.
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one digit (0–9).
    /// Default value: <c>true</c>.
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether ambiguous characters should be excluded
    /// from the generated password. Ambiguous characters include characters like
    /// '0' (zero), 'O' (uppercase o), '1' (one), 'l' (lowercase L), and 'I' (uppercase i).
    /// Default value: <c>false</c>.
    /// </summary>
    public bool ExcludeAmbiguousCharacters { get; set; } = false;

    /// <summary>
    /// Gets or sets the set of characters considered ambiguous and excluded
    /// from the generated password if <see cref="ExcludeAmbiguousCharacters"/> is <c>true</c>.
    /// Default value: <c>1lI0O</c>.
    /// </summary>
    public string AmbiguousCharacters { get; set; } = "1lI0O";
    
    /// <summary>
    /// Gets or sets a value indicating whether the password must contain at least one special character.
    /// Default value: <c>true</c>.
    /// </summary>
    public bool RequireSpecial { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the set of special characters allowed in the password.
    /// Only used if <see cref="RequireSpecial"/> is true.
    /// Default value: <c>!@#$%^&amp;*()-_=+[]{};:,.&lt;&gt;?</c>
    /// </summary>
    public string SpecialCharacters { get; set; } = "!@#$%^&*()-_=+[]{};:,.<>?";

    /// <summary>
    /// Optional additional characters to include in the allowed character pool.
    /// </summary>
    public string AdditionalCharacters { get; set; } = string.Empty;
    
    /// <summary>
    /// Validates the current password policy settings to ensure they are consistent and secure.
    /// Throws a <see cref="PasswordGeneratorException"/> if any of the following conditions are met:
    /// <list type="bullet">
    /// <item><description>The password length is less than 12 characters.</description></item>
    /// <item><description>No character types (uppercase, lowercase, digit, special) are enabled and no additional characters are defined.</description></item>
    /// <item><description>Special characters are required, but no special characters are defined.</description></item>
    /// <item><description>Ambiguous characters are to be excluded, but none are defined.</description></item>
    /// <item><description>The password length is shorter than the number of required character types.</description></item>
    /// </list>
    /// </summary>
    public void Validate()
    {
        if (Length < 12)
        {
            throw new PasswordGeneratorException("Password must be at least 12 characters.");
        }

        if (!RequireUppercase && !RequireLowercase && !RequireDigit && !RequireSpecial && string.IsNullOrEmpty(AdditionalCharacters))
        {
            throw new PasswordGeneratorException("At least one character type or additional characters must be allowed.");
        }

        if (RequireSpecial && string.IsNullOrEmpty(SpecialCharacters))
        {
            throw new PasswordGeneratorException("Special characters are required, but none are defined.");
        }

        if (ExcludeAmbiguousCharacters && string.IsNullOrEmpty(AmbiguousCharacters))
        {
            throw new PasswordGeneratorException("Ambiguous characters must be defined when exclusion is enabled.");
        }

        var requiredTypes = new[] { RequireUppercase, RequireLowercase, RequireDigit, RequireSpecial }
            .Count(req => req);

        if (Length < requiredTypes)
        {
            throw new PasswordGeneratorException("Password length is too short to meet all required character types.");
        }
    }
}

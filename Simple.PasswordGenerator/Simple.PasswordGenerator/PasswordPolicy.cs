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
    /// Validates the current policy settings.
    /// Throws an exception if the policy is invalid.
    /// </summary>
    /// <exception cref="PasswordGeneratorException">
    /// Thrown if the password length is less than 12 characters.
    /// </exception>
    public void Validate()
    {
        if (Length < 12)
            throw new PasswordGeneratorException("Password must be at least 12 characters.");
    }
}

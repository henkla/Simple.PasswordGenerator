namespace Simple.PasswordGenerator;

/// <summary>
/// Represents the result of evaluating the strength of a password,
/// including the password itself, its entropy in bits, and its calculated strength classification.
/// </summary>
public class PasswordStrengthResult
{
    /// <summary>
    /// The password that was evaluated.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The estimated entropy of the password in bits.
    /// Higher values indicate greater unpredictability.
    /// </summary>
    public double EntropyBits { get; set; }

    /// <summary>
    /// The evaluated strength category of the password (e.g., Weak, Strong).
    /// </summary>
    public PasswordStrength Strength { get; set; }
}
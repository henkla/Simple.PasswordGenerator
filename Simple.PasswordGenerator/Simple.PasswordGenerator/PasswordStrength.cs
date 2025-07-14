namespace Simple.PasswordGenerator;

/// <summary>
/// Defines the different levels of password strength,
/// ranging from VeryWeak to VeryStrong.
/// </summary>
public enum PasswordStrength
{
    /// <summary>
    /// Password is very weak and easily guessable.
    /// </summary>
    VeryWeak = 0,

    /// <summary>
    /// Password is weak and offers limited security.
    /// </summary>
    Weak,

    /// <summary>
    /// Password has moderate strength and offers reasonable security.
    /// </summary>
    Moderate,

    /// <summary>
    /// Password is strong and difficult to guess.
    /// </summary>
    Strong,

    /// <summary>
    /// Password is very strong and highly secure.
    /// </summary>
    VeryStrong
}
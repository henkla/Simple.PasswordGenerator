namespace Simple.PasswordGenerator;

public class PasswordStrengthResult
{
    public string Password { get; set; } = string.Empty;
    public double EntropyBits { get; set; }
    public PasswordStrength Strength { get; set; }
}
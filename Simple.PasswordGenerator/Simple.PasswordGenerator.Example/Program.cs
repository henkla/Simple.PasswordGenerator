using Simple.PasswordGenerator;

// Create an instance of PasswordGenerator
var passwordGenerator = new PasswordGenerator();

// Generate a password using the default policy
var defaultPassword = passwordGenerator.Generate();
Console.WriteLine($"Password with default policy: {defaultPassword}");

// Generate a password with a custom policy via lambda, now including AdditionalCharacters
var customPassword = passwordGenerator.Generate(policy =>
{
    policy.Length = 20;                         // Set the password length
    policy.RequireSpecial = false;              // Do not require special characters
    policy.RequireDigit = true;                 // Require digits
    policy.RequireUppercase = true;             // Require uppercase letters
    policy.RequireLowercase = true;             // Require lowercase letters
    policy.ExcludeAmbiguousCharacters = true;   // Exclude ambiguous characters (e.g., 0, O, l, 1, etc.)
    policy.AdditionalCharacters = "åäöüß";      // Add culture-specific characters (Swedish/German letters)
});
Console.WriteLine($"Password with custom policy (with additional chars): {customPassword}");

// Generate a password using a predefined PasswordPolicy instance with AdditionalCharacters
var policyInstance = new PasswordPolicy
{
    Length = 12,                                // Set the password length
    RequireSpecial = true,                      // Require special characters
    RequireDigit = true,                        // Require digits
    RequireUppercase = false,                   // Do not require uppercase letters
    RequireLowercase = true,                    // Require lowercase letters
    SpecialCharacters = "@#$",                  // Define allowed special characters
    ExcludeAmbiguousCharacters = true,         // Exclude ambiguous characters
    AdditionalCharacters = "çéñ"                // Add other culture-specific characters (e.g., Spanish letters)
};
var policyPassword = passwordGenerator.Generate(policyInstance);
Console.WriteLine($"Password from policy instance (with additional chars): {policyPassword}");

// Generate password with strength info including entropy and strength rating
var strengthResult = passwordGenerator.GenerateWithStrength(policy =>
{
    policy.Length = 64;                         // Set the password length
    policy.RequireDigit = true;                 // Require digits
    policy.RequireLowercase = true;             // Require lowercase letters
    policy.RequireUppercase = true;             // Require uppercase letters
    policy.RequireSpecial = true;               // Require special characters
    policy.SpecialCharacters = "!@#";           // Define allowed special characters
    policy.ExcludeAmbiguousCharacters = true;   // Exclude ambiguous characters
    policy.AdditionalCharacters = "åäöüß";      // Add culture-specific characters
});

Console.WriteLine("Password with strength info:");
Console.WriteLine($"  Password: {strengthResult.Password}");
Console.WriteLine($"  Entropy (bits): {strengthResult.EntropyBits:F2}");
Console.WriteLine($"  Strength: {strengthResult.Strength}");

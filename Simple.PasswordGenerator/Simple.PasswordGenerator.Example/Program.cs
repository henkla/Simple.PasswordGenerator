using Simple.PasswordGenerator;

// Create an instance of PasswordGenerator
var passwordGenerator = new PasswordGenerator();

// Generate a password using the default policy
var defaultPassword = passwordGenerator.Generate();
Console.WriteLine($"Password with default policy: {defaultPassword}");

// Generate a password with a custom policy via lambda
var customPassword = passwordGenerator.Generate(policy =>
{
    policy.Length = 20;                         // Set password length
    policy.RequireSpecial = false;              // No special characters
    policy.RequireDigit = true;                 // Must contain digits
    policy.RequireUppercase = true;             // Must contain uppercase letters
    policy.RequireLowercase = true;             // Must contain lowercase letters
    policy.ExcludeAmbiguousCharacters = true;   // Exclude ambiguous characters (e.g., 0, O, l, 1, etc.)
});
Console.WriteLine($"Password with custom policy: {customPassword}");

// Generate a password using a predefined PasswordPolicy instance
var policyInstance = new PasswordPolicy
{
    Length = 12,                                // Set password length
    RequireSpecial = true,                      // Must contain special characters
    RequireDigit = true,                        // Must contain digits
    RequireUppercase = false,                   // No uppercase letters
    RequireLowercase = true,                    // Must contain lowercase letters
    SpecialCharacters = "@#$",                  // Define allowed special characters
    ExcludeAmbiguousCharacters = true           // Exclude ambiguous characters
};
var policyPassword = passwordGenerator.Generate(policyInstance);
Console.WriteLine($"Password from policy instance: {policyPassword}");
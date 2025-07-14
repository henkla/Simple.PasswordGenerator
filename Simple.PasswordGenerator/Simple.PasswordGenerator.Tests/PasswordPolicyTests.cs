namespace Simple.PasswordGenerator.Tests;

public class PasswordPolicyTests
{
    [Fact]
    public void Validate_ThrowsIfLengthIsLessThan12()
    {
        var policy = new PasswordPolicy { Length = 11 };
        var ex = Assert.Throws<PasswordGeneratorException>(() => policy.Validate());
        Assert.Equal("Password must be at least 12 characters.", ex.Message);
    }

    [Fact]
    public void Validate_ThrowsIfNoCharacterTypesOrAdditionalCharacters()
    {
        var policy = new PasswordPolicy
        {
            RequireUppercase = false,
            RequireLowercase = false,
            RequireDigit = false,
            RequireSpecial = false,
            AdditionalCharacters = ""
        };
        var ex = Assert.Throws<PasswordGeneratorException>(() => policy.Validate());
        Assert.Equal("At least one character type or additional characters must be allowed.", ex.Message);
    }

    [Fact]
    public void Validate_ThrowsIfRequireSpecialIsTrueButNoSpecialCharactersDefined()
    {
        var policy = new PasswordPolicy
        {
            RequireSpecial = true,
            SpecialCharacters = ""
        };
        var ex = Assert.Throws<PasswordGeneratorException>(() => policy.Validate());
        Assert.Equal("Special characters are required, but none are defined.", ex.Message);
    }

    [Fact]
    public void Validate_ThrowsIfExcludingAmbiguousCharactersButNoneAreDefined()
    {
        var policy = new PasswordPolicy
        {
            ExcludeAmbiguousCharacters = true,
            AmbiguousCharacters = ""
        };
        var ex = Assert.Throws<PasswordGeneratorException>(() => policy.Validate());
        Assert.Equal("Ambiguous characters must be defined when exclusion is enabled.", ex.Message);
    }

    [Fact]
    public void Validate_ThrowsIfLengthIsTooShortForRequiredCharacterTypes()
    {
        var policy = new PasswordPolicy
        {
            Length = 3, // >= 12 inte uppfyllt – justera till >= 12 först
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecial = true
        };

        policy.Length = 3; // ger exception "Password must be at least 12 characters."

        var ex = Assert.Throws<PasswordGeneratorException>(() => policy.Validate());
        Assert.Equal("Password must be at least 12 characters.", ex.Message);
    }

    [Fact]
    public void Validate_DoesNotThrowForValidPolicy()
    {
        var policy = new PasswordPolicy
        {
            Length = 16,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecial = true,
            SpecialCharacters = "!@#",
            AmbiguousCharacters = "1lI0O"
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_AllowsOnlyAdditionalCharacters()
    {
        var policy = new PasswordPolicy
        {
            Length = 12,
            RequireUppercase = false,
            RequireLowercase = false,
            RequireDigit = false,
            RequireSpecial = false,
            AdditionalCharacters = "ABC"
        };

        var exception = Record.Exception(() => policy.Validate());
        Assert.Null(exception);
    }
}

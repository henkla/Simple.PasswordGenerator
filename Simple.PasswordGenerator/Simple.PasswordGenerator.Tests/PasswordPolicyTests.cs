namespace Simple.PasswordGenerator.Tests;

public class PasswordPolicyTests
{
    [Fact]
    public void Validate_LengthIsLessThan12_ShouldThrow()
    {
        // arrange

        var subjectUnderTest = new PasswordPolicy { Length = 11 };

        // act & assert

        var result = Should.Throw<PasswordGeneratorException>(() => subjectUnderTest.Validate());
        result.Message.ShouldBe("Password must be at least 12 characters.");
    }

    [Fact]
    public void Validate_WhenNoCharacterTypesOrAdditionalCharacters_ShouldThrow()
    {
        // arrange

        var subjectUnderTest = new PasswordPolicy
        {
            RequireUppercase = false,
            RequireLowercase = false,
            RequireDigit = false,
            RequireSpecial = false,
            AdditionalCharacters = ""
        };

        // act & assert

        var result = Should.Throw<PasswordGeneratorException>(() => subjectUnderTest.Validate());
        result.Message.ShouldBe("At least one character type or additional characters must be allowed.");
    }

    [Fact]
    public void Validate_WhenRequireSpecialIsTrueButNoSpecialCharactersDefined_ShouldThrow()
    {
        // arrange

        var subjectUnderTest = new PasswordPolicy
        {
            RequireSpecial = true,
            SpecialCharacters = ""
        };

        // act & assert

        var result = Should.Throw<PasswordGeneratorException>(() => subjectUnderTest.Validate());
        result.Message.ShouldBe("Special characters are required, but none are defined.");
    }

    [Fact]
    public void Validate_WhenExcludingAmbiguousCharactersButNoneAreDefined_ShouldThrow()
    {
        // arrange

        var subjectUnderTest = new PasswordPolicy
        {
            ExcludeAmbiguousCharacters = true,
            AmbiguousCharacters = ""
        };

        // act & assert

        var result = Should.Throw<PasswordGeneratorException>(() => subjectUnderTest.Validate());
        result.Message.ShouldBe("Ambiguous characters must be defined when exclusion is enabled.");
    }

    [Fact]
    public void Validate_WhenLengthIsTooShortForRequiredCharacterTypes_ShouldThrow()
    {
        // arrange

        var subjectUnderTest = new PasswordPolicy
        {
            Length = 3,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecial = true
        };

        // act & assert
        
        var result = Should.Throw<PasswordGeneratorException>(() => subjectUnderTest.Validate());
        result.Message.ShouldBe("Password must be at least 12 characters.");
    }

    [Fact]
    public void Validate_WhenPolicyIsValid_DoesNotThrow()
    {
        // arrange
        
        var subjectUnderTest = new PasswordPolicy
        {
            Length = 16,
            RequireUppercase = true,
            RequireLowercase = true,
            RequireDigit = true,
            RequireSpecial = true,
            SpecialCharacters = "!@#",
            AmbiguousCharacters = "1lI0O"
        };

        // act & assert
        
        Should.NotThrow(() => subjectUnderTest.Validate());
    }

    [Fact]
    public void Validate_WhenOnlyAdditionalCharactersProvided_ShouldNotThrow()
    {
        // arrange
        
        var subjectUnderTest = new PasswordPolicy
        {
            Length = 12,
            RequireUppercase = false,
            RequireLowercase = false,
            RequireDigit = false,
            RequireSpecial = false,
            AdditionalCharacters = "ABC"
        };

        // act & assert
        
        Should.NotThrow(() => subjectUnderTest.Validate());
    }
}
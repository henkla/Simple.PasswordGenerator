namespace Simple.PasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    [Fact]
    public void Generate_WhenPolicyIsDefault_GeneratesValidPassword()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        
        // act
        
        var result = subjectUnderTest.Generate();

        // assert
        
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Length.ShouldBe(16);
    }

    [Fact]
    public void Generate_WhenPolicyIsPassedAsObject_GeneratesValidPassword()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        var policy = new PasswordPolicy
        {
            Length = 32
        };

        // act
        
        var result = subjectUnderTest.Generate(policy);

        // assert
        
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Length.ShouldBe(policy.Length);
    }

    [Fact]
    public void Generate_WhenPolicyIsDefined_GeneratesValidPassword()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.RequireDigit = true;
            policy.RequireSpecial = false;
            policy.RequireUppercase = false;
            policy.RequireLowercase = false;
            policy.Length = 128;
        });

        // assert
        
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.All(char.IsDigit).ShouldBeTrue();
        result.Length.ShouldBe(128);
    }

    [Fact]
    public void Generate_WhenPolicyDefineLengthBelowMinimum_GenerateThrowsException()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act & assert
        
        var exception = Should.Throw<PasswordGeneratorException>(() => { subjectUnderTest.Generate(policy => { policy.Length = 11; }); });
        exception.Message.ShouldBe("Password must be at least 12 characters.");
    }

    [Fact]
    public void Generate_WhenExcludingAmbiguousCharacters_DoesNotContainAmbiguousCharacters()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 1024;
            policy.ExcludeAmbiguousCharacters = true;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
        });

        // assert
        
        var ambiguousChars = new PasswordPolicy().AmbiguousCharacters.ToCharArray();
        result.Any(c => ambiguousChars.Contains(c)).ShouldBeFalse();
    }

    [Fact]
    public void Generate_WhenAllCharacterTypesRequired_ContainsAllCharacterTypes()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 256;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = "!@#";
        });

        // assert
        
        result.Any(char.IsLower).ShouldBeTrue();
        result.Any(char.IsUpper).ShouldBeTrue();
        result.Any(char.IsDigit).ShouldBeTrue();
        result.Any(c => "!@#".Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenNoCharacterTypesEnabled_ThrowsException()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act & assert
        
        var exception = Should.Throw<PasswordGeneratorException>(() =>
        {
            subjectUnderTest.Generate(policy =>
            {
                policy.RequireDigit = false;
                policy.RequireLowercase = false;
                policy.RequireUppercase = false;
                policy.RequireSpecial = false;
            });
        });

        exception.Message.ShouldBe("At least one character type or additional characters must be allowed.");
    }

    [Fact]
    public void Generate_WhenCustomSpecialCharactersUsed_PasswordOnlyContainsThose()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        const string allowed = "*+-";

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 20;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = allowed;
        });

        // assert
        
        result.All(c => allowed.Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenAmbiguousCharactersIncluded_CanContainAmbiguousCharacters()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 1024;
            policy.ExcludeAmbiguousCharacters = false;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
        });

        // assert
        
        result.Any(c => "0O1lI".Contains(c)).ShouldBeTrue(); // Not guaranteed, but statistically very likely with 1024 chars
    }

    [Fact]
    public void Generate_WhenAdditionalCharactersProvided_IncludesAdditionalCharacters()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        const string additionalChars = "åäöüß";

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 64;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
        });

        // assert
        
        result.All(c => additionalChars.Contains(c)).ShouldBeTrue(); // All characters should be from the additionalChars (since no others are required)

    }

    [Fact]
    public void Generate_WhenExcludeAmbiguousCharacters_ExcludesAmbiguousFromAdditionalCharacters()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        var ambiguous = new PasswordPolicy().AmbiguousCharacters;
        var additionalChars = "abc" + ambiguous; // Include ambiguous chars in AdditionalCharacters for test

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 50;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
            policy.ExcludeAmbiguousCharacters = true;
        });

        // assert
        
        result.Any(c => ambiguous.Contains(c)).ShouldBeFalse(); // None of the ambiguous characters should be present
        result.All(c => "abc".Contains(c)).ShouldBeTrue(); // All characters should be from the non-ambiguous subset of additionalChars
    }

    [Fact]
    public void Generate_WhenAdditionalCharactersUsedWithOtherRequirements_IncludesAdditionalCharacters()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        const string additionalChars = "çéñ";

        // act
        
        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 1024;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
        });

        // assert 
        
        // Should contain at least one digit, lowercase, uppercase, and possibly additional characters
        result.Any(char.IsDigit).ShouldBeTrue();
        result.Any(char.IsLower).ShouldBeTrue();
        result.Any(char.IsUpper).ShouldBeTrue();

        // Check that at least one character from AdditionalCharacters is present (statistically very likely with length 1024)
        result.Any(c => additionalChars.Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void GenerateWithStrength_WhenPolicyIsValid_ReturnsPasswordAndStrengthInfo()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.GenerateWithStrength(policy =>
        {
            policy.Length = 20;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = "!@#";
            policy.ExcludeAmbiguousCharacters = true;
        });

        // assert
        
        result.ShouldNotBeNull();
        result.Password.ShouldNotBeNullOrEmpty();
        result.Password.Length.ShouldBe(20);
        result.EntropyBits.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GenerateWithStrength_WhenPolicyInvalid_ThrowsException()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act & assert
        
        var exception = Should.Throw<PasswordGeneratorException>(() =>
        {
            subjectUnderTest.GenerateWithStrength(policy => { policy.Length = 5; });
        });

        exception.Message.ShouldBe("Password must be at least 12 characters.");
    }

    [Fact]
    public void GenerateWithStrength_WhenAdditionalCharactersIncluded_ReturnsAsExpected()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();
        const string additionalChars = "åäöüß";

        // act
        
        var result = subjectUnderTest.GenerateWithStrength(policy =>
        {
            policy.Length = 30;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
        });

        // assert
        
        result.Password.ShouldNotBeNullOrEmpty();
        result.Password.All(c => additionalChars.Contains(c)).ShouldBeTrue();
        result.EntropyBits.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GenerateWithStrength_WhenPolicyIsValid_ContainsAllRequiredCharacterTypes()
    {
        // arrange
        
        var subjectUnderTest = new PasswordGenerator();

        // act
        
        var result = subjectUnderTest.GenerateWithStrength(policy =>
        {
            policy.Length = 1024;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = "!@#";
        });

        // assert
        
        var password = result.Password;
        password.Any(char.IsDigit).ShouldBeTrue();
        password.Any(char.IsLower).ShouldBeTrue();
        password.Any(char.IsUpper).ShouldBeTrue();
        password.Any(c => "!@#".Contains(c)).ShouldBeTrue();
    }
}
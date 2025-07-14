using Shouldly;

namespace Simple.PasswordGenerator.Tests;

public class PasswordGeneratorTests
{
    [Fact]
    public void Generate_WhenPolicyIsDefault_GeneratesValidPassword()
    {
        var subjectUnderTest = new PasswordGenerator();
        var result = subjectUnderTest.Generate();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Length.ShouldBe(16);
    }

    [Fact]
    public void Generate_WhenPolicyIsPassedAsObject_GeneratesValidPassword()
    {
        var subjectUnderTest = new PasswordGenerator();
        var policy = new PasswordPolicy
        {
            Length = 32
        };

        var result = subjectUnderTest.Generate(policy);

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Length.ShouldBe(policy.Length);
    }

    [Fact]
    public void Generate_WhenPolicyIsDefined_GeneratesValidPassword()
    {
        var subjectUnderTest = new PasswordGenerator();

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.RequireDigit = true;
            policy.RequireSpecial = false;
            policy.RequireUppercase = false;
            policy.RequireLowercase = false;
            policy.Length = 128;
        });

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.All(char.IsDigit).ShouldBeTrue();
        result.Length.ShouldBe(128);
    }

    [Fact]
    public void Generate_WhenPolicyDefineLengthBelowMinimum_GenerateThrowsException()
    {
        var subjectUnderTest = new PasswordGenerator();

        var exception = Should.Throw<PasswordGeneratorException>(() => { subjectUnderTest.Generate(policy => { policy.Length = 11; }); });

        exception.Message.ShouldBe("Password must be at least 12 characters.");
    }

    [Fact]
    public void Generate_WhenExcludingAmbiguousCharacters_DoesNotContainAmbiguousCharacters()
    {
        var subjectUnderTest = new PasswordGenerator();

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 64;
            policy.ExcludeAmbiguousCharacters = true;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
        });

        var ambiguousChars = new PasswordPolicy().AmbiguousCharacters.ToCharArray();

        result.Any(c => ambiguousChars.Contains(c)).ShouldBeFalse();
    }

    [Fact]
    public void Generate_WhenAllCharacterTypesRequired_ContainsAllCharacterTypes()
    {
        var subjectUnderTest = new PasswordGenerator();

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 32;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = "!@#";
        });

        result.Any(char.IsLower).ShouldBeTrue();
        result.Any(char.IsUpper).ShouldBeTrue();
        result.Any(char.IsDigit).ShouldBeTrue();
        result.Any(c => "!@#".Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenNoCharacterTypesEnabled_ThrowsException()
    {
        var subjectUnderTest = new PasswordGenerator();

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

        exception.Message.ShouldBe("No valid characters available for password generation.");
    }

    [Fact]
    public void Generate_WhenCustomSpecialCharactersUsed_PasswordOnlyContainsThose()
    {
        var subjectUnderTest = new PasswordGenerator();

        var allowed = "*+-";

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 20;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = true;
            policy.SpecialCharacters = allowed;
        });

        result.All(c => allowed.Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenAmbiguousCharactersIncluded_CanContainAmbiguousCharacters()
    {
        var subjectUnderTest = new PasswordGenerator();

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 50;
            policy.ExcludeAmbiguousCharacters = false;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
        });

        result.Any(c => "0O1lI".Contains(c)).ShouldBeTrue(); // Not guaranteed, but statistically very likely with 50 chars
    }

    [Fact]
    public void Generate_WhenAdditionalCharactersProvided_IncludesAdditionalCharacters()
    {
        var subjectUnderTest = new PasswordGenerator();

        var additionalChars = "åäöüß";

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 64;
            policy.RequireDigit = false;
            policy.RequireLowercase = false;
            policy.RequireUppercase = false;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
        });

        // All characters should be from the additionalChars (since no others are required)
        result.All(c => additionalChars.Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenExcludeAmbiguousCharacters_ExcludesAmbiguousFromAdditionalCharacters()
    {
        var subjectUnderTest = new PasswordGenerator();

        var ambiguous = new PasswordPolicy().AmbiguousCharacters;
        // Include ambiguous chars in AdditionalCharacters for test
        var additionalChars = "abc" + ambiguous;

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

        // None of the ambiguous characters should be present
        result.Any(c => ambiguous.Contains(c)).ShouldBeFalse();

        // All characters should be from the non-ambiguous subset of additionalChars
        result.All(c => "abc".Contains(c)).ShouldBeTrue();
    }

    [Fact]
    public void Generate_WhenAdditionalCharactersUsedWithOtherRequirements_IncludesAdditionalCharacters()
    {
        var subjectUnderTest = new PasswordGenerator();

        var additionalChars = "çéñ";

        var result = subjectUnderTest.Generate(policy =>
        {
            policy.Length = 100;
            policy.RequireDigit = true;
            policy.RequireLowercase = true;
            policy.RequireUppercase = true;
            policy.RequireSpecial = false;
            policy.AdditionalCharacters = additionalChars;
        });

        // Should contain at least one digit, lowercase, uppercase, and possibly additional characters
        result.Any(char.IsDigit).ShouldBeTrue();
        result.Any(char.IsLower).ShouldBeTrue();
        result.Any(char.IsUpper).ShouldBeTrue();

        // Check that at least one character from AdditionalCharacters is present (statistically very likely with length 100)
        result.Any(c => additionalChars.Contains(c)).ShouldBeTrue();
    }
}
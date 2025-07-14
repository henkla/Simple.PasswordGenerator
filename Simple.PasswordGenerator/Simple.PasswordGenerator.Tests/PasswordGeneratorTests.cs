using Shouldly;

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
        
        // act
        
        var exception = Should.Throw<PasswordGeneratorException>(() =>
        {
            subjectUnderTest.Generate(policy =>
            {
                policy.Length = 11;
            });
        });
        
        // assert
        
        exception.Message.ShouldBe("Password must be at least 12 characters.");
    }
}
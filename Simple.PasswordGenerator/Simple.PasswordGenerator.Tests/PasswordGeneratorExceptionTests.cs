namespace Simple.PasswordGenerator.Tests;

public class PasswordGeneratorExceptionTests
{
    [Fact]
    public void Constructor_WhenMessageIsProvided_SetsMessage()
    {
        // arrange & act
        
        var subjectUnderTest = new PasswordGeneratorException("Test message");
        
        // assert
        
        subjectUnderTest.Message.ShouldBe("Test message");
    }

    [Fact]
    public void Constructor_WithInnerException_SetsMessageAndInner()
    {
        // arrange
        
        var inner = new InvalidOperationException("Inner");
        
        // act
        
        var subjectUnderTest = new PasswordGeneratorException("Outer", inner);

        // assert
        subjectUnderTest.Message.ShouldBe("Outer");
        subjectUnderTest.InnerException.ShouldBeSameAs(inner);
    }
}
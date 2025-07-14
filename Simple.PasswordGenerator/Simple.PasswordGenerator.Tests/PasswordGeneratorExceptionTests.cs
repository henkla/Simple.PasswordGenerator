namespace Simple.PasswordGenerator.Tests;

public class PasswordGeneratorExceptionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        var ex = new PasswordGeneratorException("Test message");
        Assert.Equal("Test message", ex.Message);
    }

    [Fact]
    public void Constructor_WithInnerException_SetsMessageAndInner()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new PasswordGeneratorException("Outer", inner);

        Assert.Equal("Outer", ex.Message);
        Assert.Equal(inner, ex.InnerException);
    }
}
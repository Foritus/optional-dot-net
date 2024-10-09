namespace Aornis.Tests;

public abstract class TestBase
{
    protected const string initialValue = "Metal gear?!";
    protected const string otherInitialValue = "Nanomachines son!";

    protected readonly Optional<string> value;
    protected readonly Optional<string> otherValue;

    public TestBase()
    {
        value = Optional.Of(initialValue);
        otherValue = Optional.Of(otherInitialValue);
    }
}
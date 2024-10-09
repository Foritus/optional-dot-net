# Optional.net
A simple .net Optional type designed for productivity.

# Nuget package

https://www.nuget.org/packages/Optional.net/

## Examples

Optional behaviour when returning values
```
void Main() 
{
   SomeOtherFunc().IfPresent(value => Console.WriteLine(value));
}
Optional<string> SomeOtherFunc() 
{
    return Optional.Of(someField).IfNotPresent(() => "fallback value");
}
```

Capture nulls and gracefully fall back
```
void Main() 
{
    var value = Optional.Of(NullableLibraryFunction()).OrElse("treachery!");
}
```

Implicitly converts values into Optionals
```
string SomeFunc() 
{
    return "test";
}

Optional<string> result = SomeFunc();
result.IfPresent(x => Console.WriteLine(x));
```

Automatically converts nulls to Optional.Empty
```
string SomeFunc()
{
	return null;
}

Optional<string> foo = SomeFunc();
Console.WriteLine($"Result HasValue = {foo.HasValue}");
```

Chain through multiple optional functions until one returns
```
void Main()
{
    var value = Optional.get(Func1, Func2, Func3);
}

Optional<int> Func1()
{
    return Optional.Empty;
}

Optional<int> Func2()
{
    return Optional.Empty;
}

Optional<int> Func3()
{
    return Optional.Of(42);
}
```

Async friendly
```
async Task Main()
{
    await SomeFunc().IfPresentAsync(value =>
    {
        await DoAsyncWork(value);
        Console.WriteLine("Async!");
    });
}

Optional<string> SomeFunc()
{
    return Optional.Of("test");
}

async Task DoAsyncWork(string value)
{
    await File.WriteAllTextAsync("./file.text", value);
}
```

Safely transform values
```
void Main()
{
    Func1().Map(x => x + "mapping ")
           .Map(x => x + "values ")
           .Map(x => new StringBuilder(x))
           .Map(x => x.Append("safely!"))
           .Map(x => SomeFunctionThatMightReturnNull(x))
           .IfPresent(x => Console.WriteLine(x.ToString()));
}
```

Fallback when a method returns empty
```
void Main()
{
    var result = MaybeSomething().OrElse(() => "Something else"))
}

Optional<string> MaybeSomething()
{
    return Optional.Empty;
}
```

Wrap values into collections and then filter those collections to just the present values
```
void Main()
{
    IEnumerable<Optional<string>> results = Optional.Pack(SomeLibraryFunctionThatReturnsAString(),
                                                          AnotherFunctionThatMightReturnNull(),
							  AnotherFunction());

    // Pick all of the non-empty values from the list, discarding the empty ones
    Optional.UnpackPartial(results).IfPresent(listOfOnlyPresentValues => 
    {
        Console.WriteLine("At least one value was present");
        foreach(var val in listOfOnlyPresentValues) {
            Console.WriteLine(val);
        }
    })
    .IfNotPresent(() => Console.WriteLine("All values were empty!"))

    // Create an IEnumerable<T> if ALL values in "results" are present, otherwise return empty
    Optional.UnpackAll(results).IfPresent(listOfAllValues =>
    {
        foreach(var val in listOfAllValues) {
	    Console.WriteLine(val);
        }
    });
}
```

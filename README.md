# Optional.net
A simple .net Optional type designed for productivity.

# Nuget package

https://www.nuget.org/packages/Optional.net/

# New in 3.0

* Added .FlatMapAsync() extensions to Task<T>
* Added .IfPresentAsync() extensions to Task<T>
* Added OrElseThrow() to rethrow an exception if the given `Optional` does not have a value. The original exception call stack is preserved.

```
// Chain async optional tasks together, the tasks only execute if the previous task returned a value
Task<Optional<long>> input = Task.FromResult(Optional.Of(1234567890L));
Optional<string> output = await input.FlatMapAsync(async x => x.ToString());

// Rethrow exceptions in a clean manner via OrElseThrow()
try
{
    SomethingThatMightThrow();
}
catch (Exception ex)
{
    SomeFallbackThingThatReturnsOptional().OrElseThrow(ex); // Original call stack of ex is preserved
}
```

# New in 2.2
* Added extension methods on Task<Optional<T>> to make chaining optional async methods nicer

```
string result = await SomeAsyncFunc().OrElseAsync(() => "hello!");

Optional<T> result = await SomeAsyncFunc().OrElseAsync(SomeOtherFunThatReturnsOptional);

Optional<string> result = await Task.FromResult("Hello").MapAsync(x => x + " World!");

Optional<string> result = await SomeAsyncFunc().MapAsync(async x => "Hello from an async Task {x}!");
```

## New in 2.1.1:
* Support for .net framework 4.6.1 and 4.7.2 alongside existing .net core support

## New in 2.1! .NET Core 3.0 build, readonly struct optimisations and F# interop

I've added support for .NET Core 3.0 and I've made `Optional<T>` a `readonly struct` (It was never mutable anyway), which should give some small perf wins through reduced copies on member invocations.

F#'s option type is fantastic _until_ anything in C# needs to deal with it. So I've added some implicit conversions from FSharpOption<T> and FSharpValueOption<T> into their equivalent Optional.net types.
Additionally I've added a .ToFsOption() method to Optional.net optionals to make them easy for F# to consume in a performant manner.

```
module MyFSharpModule =
    let DoSomething () = 42 option // Return an F# option like normal

...

void Main()
{
    Optional<int> result = MyFsharpModule.DoSomething(); // Implicitly converts F# option into C#-friendly Optional.net option	
}
```

```
class MyCSharpClass
{
    public static Optional<string> DoSomething() => Optional.Of(42) // Return an Optional.net option
}

let myFun () =
    match MyCSharpClass.DoSomething().ToFsOption() with
	| Some value -> printfn "Easily consume Optional.net from F#! %O" value
	| None -> printfn "Life is better when we all get along"
```

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
    // Mix sync and async functions using Optionals
    await SomeFunc().IfPresentAsync(value =>
    {
        await DoAsyncWork(value);
        Console.WriteLine("Async!");
    });

    // Compose optional async tasks together!
    Optional<char[]> _ = await SomeFunc().MapAsync(x =>
    {
        return x.ToArray();
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

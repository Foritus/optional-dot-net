# Optional.net
A simple .net Optional type designed for productivity.

# Nuget package

https://www.nuget.org/packages/Optional.net/1.0.0

## Examples

Optional behaviour when returning values
```
void Main() {
   SomeOtherFunc().IfPresent(value => Console.WriteLine(value));
}
Optional<string> SomeOtherFunc() {
    return Optional.Of(someField).IfNotPresent(() => "fallback value");
}
```

Capture nulls and gracefully fall back
```
void Main() {
    var value = Optional.Of(NullableLibraryFunction()).OrElse("treachery!");
}
```

Implicitly converts values into Optionals
```
	string SomeFunc() {
		return "test";
	}

    Optional<string> result = SomeFunc();
```

Automatically converts nulls to Optional.Empty
```
	string SomeFunc() {
		return null;
	}

    Optional<string> foo = SomeFunc();
	Console.WriteLine($"Result HasValue = {foo.HasValue}");
```

Chain through multiple optional functions until one returns
```
void Main() {
    var value = Optional.get(Func1, Func2, Func3);
}

Optional<int> Func1() {
    return Optional.Empty;
}

Optional<int> Func2() {
    return Optional.Empty;
}

Optional<int> Func3() {
    return Optional.Of(42);
}
```

Async friendly
```
async Task Main() {
    await SomeFunc().IfPresentAsync(value => {
        await DoAsyncWork(value);
        Console.WriteLine("Async!");
    });
}

Optional<string> SomeFunc() {
    return Optional.Of("test");
}

async Task DoAsyncWork(string value) {
    await File.WriteAllTextAsync("./file.text", value);
}
```

Safely transform values
```
void Main() {
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
void Main() {
    var result = MaybeSomething().OrElse(() => "Something else"))
}

Optional<string> MaybeSomething() {
    return Optional.Empty;
}
```

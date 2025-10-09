# PosInfoMoq1010: Use `+= null` syntax when raising events with `Raise()`/`RaiseAsync()`

| Property              | Value                                                                 |
|-----------------------|----------------------------------------------------------------------|
| **Rule ID**           | PosInfoMoq1010                                                       |
| **Title**             | Use `+= null` syntax when raising events with `Raise()`/`RaiseAsync()` |
| **Category**          | Design                                                               |
| **Default severity**  | Warning                                                              |

## Cause

When using `Mock<T>.Raise()` or `Mock<T>.RaiseAsync()`, the lambda expression used to identify the event should consistently use the `+= null` syntax.

## Rule description

While Moq is flexible and can identify the event even with `+= SomeMethod`, using `+= null` is the conventional and clearest way to indicate that you are merely referencing the event for Moq's internal use, not actually subscribing a handler. This improves code readability and intent.

### Example

```csharp
public class Service
{
    public event EventHandler Changed;

    public void DoSomething()
    {
        this.Changed?.Invoke(this, EventArgs.Empty);
    }
}
```

#### Correct usage (`+= null`)

```csharp
var serviceMock = new Mock<Service>();

// Clear intent: referencing the event for Moq
serviceMock.Raise(s => s.Changed += null, EventArgs.Empty);
```

#### Incorrect usage (`+= SomeMethod`)

```csharp
public class MyTests
{
    private void MyEventHandler(object sender, EventArgs e) { /* ... */ }

    [Fact]
    public void Test()
    {
        var serviceMock = new Mock<Service>();

        // While functional, less clear intent
        serviceMock.Raise(s => s.Changed += MyEventHandler, EventArgs.Empty); // ⚠️
    }
}
```

## How to fix violations

Change the lambda expression to use `+= null` when identifying the event for `Raise()` or `RaiseAsync()`.

Correct:

```csharp
serviceMock.Raise(s => s.Changed += null, EventArgs.Empty);
```

Incorrect:

```csharp
serviceMock.Raise(s => s.Changed += MyEventHandler, EventArgs.Empty);
```

## When to suppress warnings

This is a design guideline for code readability. While suppressing it won't cause runtime errors, it's generally recommended to follow for consistent and clear code.

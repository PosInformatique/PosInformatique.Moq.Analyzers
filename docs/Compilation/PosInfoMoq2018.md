# PosInfoMoq2018: The first parameter of `Raise()`/`RaiseAsync()` must be an event

| Property              | Value                                                                 |
|-----------------------|----------------------------------------------------------------------|
| **Rule ID**           | PosInfoMoq2018                                                       |
| **Title**             | The first parameter of `Raise()`/`RaiseAsync()` must be an event      |
| **Category**          | Compilation                                                          |
| **Default severity**  | Error                                                                |

## Cause

The first parameter passed to `Raise()` or `RaiseAsync()` must reference an **event** member of the mocked type.  
Using any other kind of member (property, method, field, etc.) is invalid.

## Rule description

When using Moq, `Mock<T>.Raise()` and `Mock<T>.RaiseAsync()` are designed to trigger **events** defined in the mocked type.  
If the provided lambda expression instead references something that is not an event (like a property assignment), the code is invalid.

### Example

```csharp
public class Service
{
    public event EventHandler Changed;
    public event EventHandler<DataEventArgs> DataChanged;
    public int Property { get; set; }
}

public class DataEventArgs : EventArgs
{
    public int Value { get; }
    public DataEventArgs(int value) => Value = value;
}
```

#### Correct usage (event targeted)

```csharp
var serviceMock = new Mock<Service>();

// Raise() with EventHandler
serviceMock.Raise(s => s.Changed += null, EventArgs.Empty);

// Raise() with EventHandler<T>
serviceMock.Raise(s => s.DataChanged += null, new DataEventArgs(42));

// RaiseAsync() with EventHandler<T>
await serviceMock.RaiseAsync(s => s.DataChanged += null, new DataEventArgs(42));
```

#### Incorrect usage (invalid target)

```csharp
var serviceMock = new Mock<Service>();

// Refers to a property, not an event
serviceMock.Raise(s => s.Property = 10, EventArgs.Empty);    // ❌

// Same problem with RaiseAsync
await serviceMock.RaiseAsync(s => s.Property = 10, EventArgs.Empty);    // ❌
```

## How to fix violations

Ensure that the first parameter of `Raise()` and `RaiseAsync()` always references an **event** of the mocked type, never a property or field.

Correct:

```csharp
serviceMock.Raise(s => s.Changed += null, EventArgs.Empty);
```

Incorrect:

```csharp
serviceMock.Raise(s => s.Property = 10, EventArgs.Empty);
```

## When to suppress warnings

Do not suppress this warning. If bypassed, Moq itself will throw an `ArgumentException` exception at runtime with a message such as
`Unsupported expression: m => (m.Property = 10)`.
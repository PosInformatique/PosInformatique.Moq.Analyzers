# PosInfoMoq2017: `Mock<T>.Raise()`/`RaiseAsync()` must use parameters matching the event signature

| Property              | Value                                                                 |
|-----------------------|----------------------------------------------------------------------|
| **Rule ID**           | PosInfoMoq2017                                                       |
| **Title**             | `Mock<T>.Raise()`/`RaiseAsync()` must use parameters matching the event signature |
| **Category**          | Compilation                                                          |
| **Default severity**  | Error                                                                |

## Cause

The parameters passed to `Raise()` or `RaiseAsync()` must exactly match the parameters of the corresponding event delegate.

## Rule description

When mocking events with **Moq**, calling `Mock<T>.Raise()` or `Mock<T>.RaiseAsync()` requires that the provided arguments match the event signature.

- With the **`Raise(event, EventArgs)`** overload:
  - Used for `EventHandler` or `EventHandler<T>`.
  - Moq automatically supplies the `sender` (the mocked object).
  - You only pass the `EventArgs` (or derived class) instance.

- With the **`Raise(event, params object[])`** overload:
  - You provide all the arguments of the event delegate yourself.
  - This includes the `sender` and all additional event arguments.

### Example

```csharp
public class Service
{
    public event EventHandler Changed;
    public event EventHandler<DataEventArgs> DataChanged;

    public void DoSomething()
    {
        this.Changed?.Invoke(this, EventArgs.Empty);
        this.DataChanged?.Invoke(this, new DataEventArgs(42));
    }
}

public class DataEventArgs : EventArgs
{
    public int Value { get; }
    public DataEventArgs(int value) => Value = value;
}
```

#### Correct usage (matching parameters)

```csharp
var serviceMock = new Mock<Service>();

// Raise with EventHandler (sender is mocked object, EventArgs required)
serviceMock.Raise(s => s.Changed += null, EventArgs.Empty);

// Raise with EventHandler<T> (sender is mocked object, T required)
serviceMock.Raise(s => s.DataChanged += null, new DataEventArgs(42));

// Raise with params object[] (sender + event args explicitly)
serviceMock.Raise(s => s.DataChanged += null, "CustomSender", new DataEventArgs(42));
```

#### Incorrect usage (parameters not matching)

```csharp
var serviceMock = new Mock<Service>();

// Missing EventArgs
serviceMock.Raise(s => s.Changed += null);   // ❌

// Wrong argument type
serviceMock.Raise(s => s.DataChanged += null, EventArgs.Empty);   // ❌
```

## How to fix violations

To fix a violation, ensure that the arguments passed to `Raise()` or `RaiseAsync()` match **exactly** the event delegate signature:

- `EventHandler`: `(object sender, EventArgs e)`
- `EventHandler<T>`: `(object sender, T e)`

Depending on the overload:
- With `Raise(event, EventArgs)` → provide only `EventArgs` or `T`.
- With `Raise(event, params object[])` → provide both `sender` and `EventArgs` (or `T`).

## When to suppress warnings

Do not suppress errors from this rule. If skipped, Moq will throw a runtime exception such as `TargetParameterCountException`
(with the following message *"Parameter count mismatch"*) or an `ArgumentException` to explain that a value can not be converted
to an other type.

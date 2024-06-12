# PosInfoMoq2007: The `As<T>()` method can be used only with interfaces.

| Property                            | Value															|
|-------------------------------------|-----------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2007													|
| **Title**                           | The `As<T>()` method can be used only with interfaces.          |
| **Category**                        | Compilation														|
| **Default severity**				  | Error															|

## Cause

The `As<T>()` method is used with a type which is not an interface.

## Rule description

Moq allows to add additional implementations for mocked class (or interface) by adding additional interfaces
with the `As<T>()` method.

```csharp
[Fact]
public void Test()
{
    var service = new Mock<Service>();
    service.As<IDisposable>()           // Add IDisposable implementation for the mocked Service class.
        .Setup(s => s.Dispose());
    service.As<OtherService>();         // An error will be raised, because we can't mock additional implementation of a class.
}

public abstract class Service
{
}

public abstract class OtherService
{
}
```

## How to fix violations

To fix a violation of this rule, use an interface when using the `As<T>()` method.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Can only add interfaces to the mock."* message.

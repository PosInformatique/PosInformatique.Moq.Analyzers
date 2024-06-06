# PosInfoMoq2004: The `Callback()` delegate expression must match the signature of the mocked method

| Property                            | Value																|
|-------------------------------------|---------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2004														|
| **Title**                           | Constructor arguments cannot be passed for interface mocks.  	    |
| **Category**                        | Compilation															|
| **Default severity**				  | Error																|

## Cause

Constructor arguments has been passed to a mocked interface.

## Rule description

It is not possible to pass contructor arguments to mocked interface. Is only possible with non-sealed class
or abstract class.

```csharp
[Fact]
public void Test()
{
    var service1 = new Mock<IService>("Argument 1", 2);                         // No constructor arguments can be passed to mocked interface.
    var service2 = new Mock<IService>(MockBehavior.Strict, "Argument 1", 2);    // Same if we use the MockBehavior.
}

public interface IService
{
}
```

## How to fix violations

To fix a violation of this rule, be sure to pass parameters to mocked abstract class.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Constructor arguments cannot be passed for interface mocks."* message.

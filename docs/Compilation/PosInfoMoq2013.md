# PosInfoMoq2013: The delegate in the argument of the `Returns()`/`ReturnsAsync()` method must have the same parameter types of the mocked method/property.

| Property                            | Value																    |
|-------------------------------------|-------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2013														    |
| **Title**                           | The delegate in the argument of the `Returns()`/`ReturnsAsync()` method must have the same parameter types of the mocked method/property.  |
| **Category**                        | Compilation															    |
| **Default severity**				  | Error																    |

## Cause

The delegate in the argument of the `Returns()`/`ReturnsAsync()` method must have the same parameter types of the mocked method/property.

## Rule description

The lambda expression, anonymous method or method in the argument of the `Returns()` must have the same parameter types of the mocked method/property.
> NB: Moq allows to pass a delegate with no argument in the `Returns()`/`ReturnsAsync()` method even the setup method contains arguments.

```csharp
[Fact]
public void Test()
{
    var validMock = new Mock<IService>();
    validMock.Setup(s => s.GetData(1234))
    .Returns((int a) =>                     // OK, the mocked GetData() take a int value as argument.
        {
            return 1234;
        });
    validMock.Setup(s => s.GetData(1234))
        .Returns(() =>                      // OK, Moq allows no arguments.
        {
            return 1234;
        });
    validMock.Setup(s => s.IsAvailable)     // OK, property don't have arguments.
        .Returns(() =>
        {
            return true;
        });

    var invalidMock = new Mock<IService>();
    invalidMock.Setup(s => s.GetData(1234))
        .Returns((string s) =>              // Error, the mocked GetData() take a int value as argument.
        {
            return "Foobar";
        });
    invalidMock.Setup(s => s.IsAvailable)
        .Returns((string s) =>              // Error, mocked property have no arguments.
        {
            return "Foobar";
        });
}

public interface IService
{
    int GetData(int id);

    bool IsAvailable { get; }
}
```

## How to fix violations

To fix a violation of this rule, be sure the lambda expression, anonymous method or method as parameter of the `Returns()`/`ReturnsAsync()`
method must have the same arguments type of the setup property/method.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Object of type 'xxx' cannot be converted to type 'yyy'."* message.

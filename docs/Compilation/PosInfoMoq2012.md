# PosInfoMoq2012: The delegate in the argument of the `Returns()` method must return a value with same type of the mocked method.

| Property                            | Value																    |
|-------------------------------------|-------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2012														    |
| **Title**                           | The delegate in the argument of the `Returns()` method must return a value with same type of the mocked method.  |
| **Category**                        | Compilation															    |
| **Default severity**				  | Error																    |

## Cause

The delegate in the argument of the `Returns()` must return return a value of the same type as the mocked method or property.

## Rule description

The lambda expression, anonymous method or method in the argument of the `Returns()` must return return a value of the same type as the mocked method or property.

```csharp
[Fact]
public void Test()
{
    var validMock = new Mock<IService>();
    validMock.Setup(s => s.GetData())
        .Returns(() =>
        {
            return 1234;        // OK, the mocked GetData() method return an int.
        });
    validMock.Setup(s => s.IsAvailable)
        .Returns(() =>
        {
            return true;        // OK, the mocked IsAvailable property return a bool.
        });

    var invalidMock = new Mock<IService>();
    invalidMock.Setup(s => s.GetData())
        .Returns(() =>
        {
            return "Foobar";    // Error, the mocked GetData() method must return an int.
        });
    invalidMock.Setup(s => s.IsAvailable)
        .Returns(() =>
        {
            return "Foobar";   // Error, the mocked IsAvailable property must return a bool.
        });
}

public interface IService
{
    int GetData();

    bool IsAvailable { get; }
}
```

## How to fix violations

To fix a violation of this rule, be sure the lambda expression, anonymous method or method as parameter of the `Returns()`
method returns values with the same type of mocked method/property.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Invalid callback. Setup on method with return type 'xxx' cannot invoke callback with return type 'yyy'."* message.

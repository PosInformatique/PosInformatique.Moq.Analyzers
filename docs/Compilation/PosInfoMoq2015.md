# PosInfoMoq2015: The `Protected().Setup()` method must match the return type of the mocked method

| Property                            | Value																                         |
|-------------------------------------|----------------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2015														                         |
| **Title**                           | The `Protected().Setup()` method must match the return type of the mocked method.            |
| **Category**                        | Compilation															                         |
| **Default severity**				  | Error																                         |

## Cause

The method setup with `Protected().Setup()` must match the return type of the mocked method.

## Rule description

When using the `Protected().Setup()`, the return type of mocked method must match of the generic
argument specified in the `Setup<T>()` method.

```csharp
[Fact]
public void Test()
{
    var service = new Mock<Service>();
    service.Protected().Setup<int>("GetData")       // OK.
        .Returns(10);
    service.Protected().Setup("GetData")            // Error: The GetData() return an int, Setup<int>() must be use.
    service.Protected().Setup<int>("SendEmail")     // Error: The SendEmail() method does not return a value, the `int` generic argument must be remove.0
        .Returns(10);
    service.Protected().Setup<string>("GetData")    // Error: The GetData() return an int, Setup<int>() must be use.
        .Returns("The data");
}

public abstract class Service
{
    protected abstract int GetData();

    protected abstract void SendEmail();
}
```

## How to fix violations

To fix a violation of this rule, use the generic parameter of the `Setup<T>()` method if the protected mocked
method return a value. Else do not specify a generic parameter for the `Setup<T>()` method of the protected mocked
method does not return a value (`void`).

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Can't set return value for void method xxx."* message.

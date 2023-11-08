# PosInfoMoq2003: The `Callback()` delegate expression must match the signature of the mocked method

| Property                            | Value																					|
|-------------------------------------|-----------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2003																			|
| **Title**                           | The `Callback()` delegate expression must match the signature of the mocked method		|
| **Category**                        | Compilation																				|
| **Default severity**				  | Error																					|

## Cause

The delegate in the argument of the `Callback()` method must match the signature of the mocked method.

## Rule description

The lambda expression in the argument of the `Callback()` method must match the signature of the mocked method.

For example, the `Callback()` have a lambda expression with the `(string, double)` signature
which does not match the `GetData()` mocked method which have the `(string, int)` signature.

```csharp
[Fact]
public void Test()
{
    var service = new Mock<Service>();
    service.Setup(s => s.GetData("TOURREAU", 1234))
        .Callback((string n, double age) =>		// Different signature of the GetData() method.
        {
        	// ...
        })
        .Returns(10);
}

public interface IService
{
	public int GetData(string name, int age) { }
}
```

## How to fix violations

To fix a violation of this rule, be sure to use the mocked method signature in the `Callback()` method.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Invalid callback. Setup on method with parameters (xxx) cannot invoke callback with parameters (yyy)."* message.

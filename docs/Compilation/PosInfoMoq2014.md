# PosInfoMoq2014: The `Callback()` delegate expression must not return a value.

| Property                            | Value																					|
|-------------------------------------|-----------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2014																			|
| **Title**                           | The `Callback()` delegate expression must not return a value.                		    |
| **Category**                        | Compilation																				|
| **Default severity**				  | Error																					|

## Cause

The delegate in the argument of the `Callback()` method must not return a value.

## Rule description

The lambda expression in the argument of the `Callback()` method must not return a value.

```csharp
[Fact]
public void Test()
{
    var service = new Mock<Service>();
    service.Setup(s => s.GetData("TOURREAU", 1234))
        .Callback((string n, int age) =>
        {
        	// ...
            return 1234;        // The delegate in the Callback() method must not return a value.
        })
        .Returns(10);
}

public interface IService
{
	public int GetData(string name, int age) { }
}
```

## How to fix violations

To fix a violation of this rule, be sure that the delegate method in the `Callback()` method does not return a value.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Invalid callback. This overload of the "Callback" method only accepts "void" (C#) or "Sub" (VB.NET) delegates with parameter types matching those of the set up method. (Parameter 'callback')"* message.

# PosInfoMoq2002: `Mock<T>` class can be used only to mock non-sealed class

| Property                            | Value                                                         |
|-------------------------------------|---------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2002                                                |
| **Title**                           | `Mock<T>` class can be used only to mock non-sealed class     |
| **Category**                        | Compilation													  |
| **Default severity**				  | Error														  |

## Cause

The `Mock<T>` can mock only interfaces or non-`sealed` classes.

## Rule description

The `Mock<T>` method must be use only on the interfaces or non-`sealed` classes.

For example, the following code can not mock the `Service` class because it is `sealed`.

```csharp
[Fact]
public void Test()
{
	var service = new Mock<Service>();		// The Service can not be mocked, because it is a sealed class.
	service.Setup(s => s.GetData())
		.Returns(10);
}

public class Service
{
	public virtual int GetData() { }
}
```

## How to fix violations

To fix a violation of this rule, be sure to mock interfaces or non-sealed classes.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Type to mock (xxx) must be an interface, a delegate, or a non-selead, non-static class"* message.

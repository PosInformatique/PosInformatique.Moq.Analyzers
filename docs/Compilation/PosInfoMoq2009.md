# PosInfoMoq2009: `Mock.Of<T>` method must be used only to mock non-sealed class

| Property                            | Value                                                         |
|-------------------------------------|---------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2009                                                |
| **Title**                           | `Mock.Of<T>` method must be used only to mock non-sealed class     |
| **Category**                        | Compilation													  |
| **Default severity**				  | Error														  |

## Cause

The `Mock.Of<T>` method must be used only to mock non-sealed class.

## Rule description

The `Mock.Of<T>` method must be use only on the interfaces or non-`sealed` classes.

For example, the following code can not mock the `Service` class because it is `sealed`.

```csharp
[Fact]
public void Test()
{
	var service = Mock.Of<Service>(s => s.Property == 1234);		// The Service can not be mocked, because it is a sealed class.
}

public class Service
{
	public virtual int Property { get; }
}
```

## How to fix violations

To fix a violation of this rule, be sure to mock interfaces or non-sealed classes.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Type to mock (xxx) must be an interface, a delegate, or a non-selead, non-static class"* message.

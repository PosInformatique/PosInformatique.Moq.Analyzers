# PosInfoMoq2010: `Mock.Of<T>` method must be used only with types that contains parameterless contructor

| Property                            | Value                                                         |
|-------------------------------------|---------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2010                                                |
| **Title**                           | `Mock.Of<T>` method must be used only with types that contains parameterless contructor. |
| **Category**                        | Compilation													  |
| **Default severity**				  | Error														  |

## Cause

The `Mock.Of<T>` method must be used only with types that contains accessible parameterless constructor.

## Rule description

The `Mock.Of<T>` method must be use only for non-`sealed` classes which contains accessible parameterless constructor.

For example, the following code can not mock the `Service` class because it does not contain a parameterless constructor.

```csharp
[Fact]
public void Test()
{
	var service = Mock.Of<Service>(s => s.Property == 1234);		// The Service can not be mocked, because not parameterless constructor exists.
}

public class Service
{
	public Service(int timeout)
	{
	}

	public virtual int Property { get; }
}
```

In this other example, the `Service` class cannot be mocked too because it contains a private constructor.

```csharp
[Fact]
public void Test()
{
	var service = Mock.Of<Service>();		// The Service can not be mocked, because the parameterless constructor is private.
}

public class Service
{
	private Service()
	{
	}

	public virtual int Property { get; }
}
```

## How to fix violations

To fix a violation of this rule, be sure to the mocked type contains an accessible parameterless constructor
(`public`, `protected` or `internal`)

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Can not instantiate proxy of class: xxx.
Could not find a parameterless constructor. (Parameter 'constructorArguments')"* message.

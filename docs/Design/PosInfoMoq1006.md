# PosInfoMoq1006: The `It.IsAny<T>()` or `It.Is<T>()` arguments must match the parameters of the mocked method.

| Property                            | Value                                                                                                |
|-------------------------------------|------------------------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq1006                                                                                       |
| **Title**                           | The `It.IsAny<T>()` or `It.Is<T>()` arguments must match the parameters of the mocked method.        |
| **Category**                        | Design																                                 |
| **Default severity**				  | Warning																                                 |

## Cause

When setting up a method using `It.IsAny<T>()` or `It.Is<T>()` as arguments,
the type `T` must exactly match the parameters of the configured method.

## Rule description

Although the compiler checks the validity between the parameters and arguments of the configured method,
the type specified in `It.IsAny<T>()` or `It.Is<T>()` must still match the method parameters.  
This is especially important when using nullable `struct` parameters, which can easily cause confusion.

For example, consider the following test code:

```csharp
[Fact]
public interface IRepository
{
	void DeleteUser(int? id)
}
```

If you mock the method from the `IRepository` type and use `It.IsAny<int>()` as the argument,
the following code will throw an exception because `It.IsAny<int>()` is treated as the integer value `0`:

```csharp
[Fact]
public void DeleteUser()
{
	var repository = new Mock<IRepository>(MockBehavior.Strict);
	repository.Setup(It.IsAny<int>());

	repository.Object.DeleteUser(null);		// Will raise an exception, because It.IsAny<int>() ('0') is different of `null`.
}
```

Instead, you should use `It.IsAny<int?>()` with a nullable `int?`,
which makes the method configuration explicit and avoids mismatches:

```csharp
[Fact]
public void DeleteUser()
{
	var repository = new Mock<IRepository>(MockBehavior.Strict);
	repository.Setup(It.IsAny<int?>());

	repository.Object.DeleteUser(null);
}
```

## How to fix violations

To fix a violation of this rule, ensure the type arguments used with `It.IsAny<T>()` or `It.Is<T>()` exactly match the parameters of the configured method.

## When to suppress warnings

You may suppress warnings from this rule, but for better readability and maintainability,
it is strongly recommended to always use matching type arguments in `It.IsAny<T>()` or `It.Is<T>()`.  
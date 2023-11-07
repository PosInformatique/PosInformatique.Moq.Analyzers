# PosInfoMoq2000: The `Returns()` or `ReturnsAsync()` methods must be call for Strict mocks

| Property                            | Value                                                                       |
|-------------------------------------|-----------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2000                                                              |
| **Title**                           | The `Returns()` or `ReturnsAsync()` methods must be call for Strict mocks   |
| **Category**                        | Compilation																	|
| **Default severity**				  | Error																		|

## Cause

A `Returns()` or `ReturnsAsync()` of an `Mock<T>` instance with `Strict` behavior has not been called after a `Setup()` call.

## Rule description

When a `Mock<T>` has been defined with the `Strict` behavior, the `Returns()` or `ReturnsAsync()` methods must be call
when setup a method to mock which returns a value.

```csharp
[Fact]
public void GetCustomer_ShouldCallRepository()
{
	// Arrange
	repository = new Mock<IRepository>(MockBehavior.Strict);
	repository.Setup(r => r.GetData());		// No Returns() method has been specified.
	...
	...
	// A MoqException will be thrown when the GetData() method will be called.
}
```

## How to fix violations

To fix a violation of this rule, call the `Returns()` or `ReturnsAsync()` method after the `Setup()`
call to setup the method to mock.

For example with the following code:

```csharp
public interface IRepository
{
    int GetData();
}

public class CustomerService
{
	private readonly IRepository repository;

    public CustomerService(IRepository repository)
	{
		this.repository = repository;
	}

	public int GetDataFromRepository()
	{
		return this.repository.GetData();
	}
}
```

For the associated unit test, the `Returns()` method have to be called for the `GetData()` method setup.

```csharp
[Fact]
public void GetCustomer_ShouldCallRepository()
{
	// Arrange
	repository = new Mock<IRepository>(MockBehavior.Strict);
	repository.Setup(r => r.GetData())
		.Returns(1234);					// The Returns() method is mandatory.

	var service = new CustomerService(repository.Object);

	// Act
	var result = service.GetDataFromRepository();

	// Arrange
	result.Should().Be(1234);
}
```

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Invocation needs to return a value and therefore must have a corresponding setup that provides it."* message.

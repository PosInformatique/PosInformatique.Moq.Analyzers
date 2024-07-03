# PosInfoMoq1003: The `Callback()` method should be used to check the parameters when mocking a method with `It.IsAny<T>()` arguments

| Property                            | Value                                                                  												|
|-------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq1003                                                         												|
| **Title**                           | The `Callback()` method should be used to check the parameters when mocking a method with `It.IsAny<T>()` arguments	|
| **Category**                        | Design																												|
| **Default severity**				  | Warning																												|

## Cause

A method has been setup with `It.IsAny<T>()` arguments without checking the parameters in a `Callback()`
method.

## Rule description

When setup a method using `It.IsAny<T>()` arguments, the parameters should be check in the `Callback()` method.

For example if we have the following code to test:

```csharp
[Fact]
public class CustomerService
{
	private readonly ISmtpService smtpService;

	public CustomerService(ISmtpService smtpService)
	{
		this.smtpService = smtpService;
	}

	public void SendMail(string emailAddress)
	{
		this.smtpService.SendMail("sender@domain.com", emailAddress);
	}
}
```

If we mock the `ISmtpService.SendMail()` with a `It.IsAny<string>()` for the `emailAddress` argument,
we can not check if the `CustomerService.SendMail()` has propagate correctly the value of the argument to the
parameter of the `ISmtpService.SendMail()` method.

```csharp
[Fact]
public void SendMail_ShouldCallSmtpService()
{
	var smtpService = new Mock<ISmtpService>();
	smtpService.Setup(s => s.SendMail("sender@domain.com", It.IsAny<string>()));	// With It.IsAny<string>() we can not check that emailAddress has been correctly passed in the CustomerService.SendMail() method.

	var service = new CustomerService(smtpService.Object);

	service.SendMail("Gilles");
}
```

The `emailAddress` parameter passed to the `ISmtpService.SendMail()` method should be tested
with the `Callback()` method, when mocking the `ISmtpService.SendMail()` method with a `It.IsAny<T>()` argument.

```csharp
[Fact]
public void SendMail_ShouldCallSmtpService()
{
	var smtpService = new Mock<ISmtpService>();
	smtpService.Setup(s => s.SendMail("sender@domain.com", It.IsAny<string>()))		// With It.IsAny() we should test the arguments if correctly propagated in the Callback() method.
		.Callback((string _, string emailAddress) =>
		{
			Assert.AreEqual("Gilles", em);	// Check the emailAddress parameter.
		});

	var service = new CustomerService(smtpService.Object);

	service.SendMail("Gilles");
}
```

### Remarks
- If the parameters of mocked methods are very simple (primitive values for example), pass directly the expected value in the arguments of the method. For example
  ```csharp
  smtpService.Setup(s => s.SendMail("sender@domain.com", "Gilles"))
  ```
  Instead of
  ```csharp
  smtpService.Setup(s => s.SendMail("sender@domain.com", It.IsAny<string>()))
  ```
- Use the `Callback()` to assert complex parameters.

## How to fix violations

To fix a violation of this rule, use the `Callback()` method to check the `It.IsAny<T>()` arguments.

## When to suppress warnings

Do not suppress a warning from this rule. Normally `It.IsAny<T>()` arguments should be check and asserted in the `Callback()` methods.
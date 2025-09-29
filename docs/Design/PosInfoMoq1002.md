# PosInfoMoq1002: `Verify()` methods should be called when `Verifiable()` has been setup

| Property                            | Value                                                                  |
|-------------------------------------|------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq1002                                                         |
| **Title**                           | `Verify()` methods should be called when `Verifiable()` has been setup |
| **Category**                        | Design																   |
| **Default severity**				  | Warning																   |

## Cause

A `Verify()` of an `Mock<T>` instance has not been called in the *Assert* phase of an unit test for `Verifiable()` setups.

## Rule description

In the *Arrange* phase of an unit test, when `Verifiable()` method has been setup to mocked member, the
`Verify()` method should be called in the *Assert* phase to check the setup member has been called.

```csharp
[Fact]
public void SendMail_ShouldCallSmtpService()
{
	// Arrange
	var smtpService = new Mock<ISmtpService>();
	smtpService.Setup(s => s.SendMail("sender@domain.com", "Gilles"))
		.Verifiable();

	var service = new CustomerService(smtpService.Object);

	// Act
	service.SendMail("Gilles");

	// Assert
	smtpService.Verify();	// The Verify() will check that the mocked ISmtpService.SendMail() has been called (because marked with the ".Verifiable()" method).
}
```

## How to fix violations

To fix a violation of this rule, call the `Verify()` in the *Assert* phase
on the `Mock<T>` instances, if some mocked members has been marked as `Verifiable()` in the *Arrange* phase.

## When to suppress warnings

Do not suppress a warning from this rule. Normally `Verifiable()` setup members must be call in the unit tests execution.

## Related rules

- [PosInfoMoq1007: The `Verify()` method must specify the `Times` argument](./PosInfoMoq1007.md)
- [PosInfoMoq1009: Avoid using `Verifiable()` method](./PosInfoMoq1009.md)
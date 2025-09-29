# PosInfoMoq1009: Avoid using `Verifiable()` method

| Property            | Value                                                                 |
|---------------------|-----------------------------------------------------------------------|
| **Rule ID**         | PosInfoMoq1009                                                        |
| **Title**           | Avoid using `Verifiable()` method                                     |
| **Category**        | Design                                                                |
| **Default severity**| Warning                                                               |

## Cause

Using the `Verifiable()` method in the *Arrange* phase of a unit test is anti-pattern.  
`Verifiable()` sets up expectations during the setup (*Arrange*) step, but the actual verification should occur in the *Assert* phase.

## Rule description

The `Verifiable()` method should be avoided because it is declared during the *Arrange* phase, which is not the correct place to assert test outcomes.  
Instead, you should explicitly call `Verify()` or `VerifyAll()` at the end of the test in the *Assert* phase to ensure mocked members have been correctly invoked.  

```csharp
[Fact]
public void SendMail_ShouldCallSmtpService()
{
    // Arrange
    var smtpService = new Mock<ISmtpService>();
    smtpService.Setup(s => s.SendMail("sender@domain.com", "Gilles"));
    var service = new CustomerService(smtpService.Object);

    // Act
    service.SendMail("Gilles");

    // Assert
    smtpService.VerifyAll();   // Explicit verification at the end of the test
}
```

## How to fix violations

Do **not** call `Verifiable()`.  
Instead, configure mocks without it, and perform explicit verifications using `Verify()` or `VerifyAll()` in the *Assert* phase.

## When to suppress warnings

Do not suppress warnings from this rule.  
Proper unit testing practices require verification in the *Assert* phase, not in the *Arrange* phase.

## Related rules

- [PosInfoMoq1002: `Verify()` methods should be called when `Verifiable()` has been setup](PosInfoMoq1002.md)
## Release 1.2.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq2001  | Compilation | Error  | The `Setup()` method can be used only on overridable members.
PosInfoMoq2002  | Compilation | Error  | `Mock<T>` class can be used only to mock non-sealed class.

## Release 1.1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq2000  | Compilation | Error  | The `Returns()` or `ReturnsAsync()` method must be called for Strict mocks.

## Release 1.0.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq1000  | Design   | Warning  | `Verify()` and `VerifyAll()` methods should be called when instantiate a Mock<T> instances.
PosInfoMoq1001  | Design   | Warning  | The `Mock<T>` instance behavior should be defined to Strict mode.

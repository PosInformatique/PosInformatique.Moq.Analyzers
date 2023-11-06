## Release 1.0.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
MQ1000  | Design   | Warning  | `Verify()` and `VerifyAll()` methods should be called when instantiate a Mock<T> instances
MQ1001  | Design   | Warning  | The `Mock<T>` instance behavior should be defined to Strict mode
MQ2000  | Compilation | Error  | The `Returns()` or `ReturnsAsync()` method must be called for Strict mocks
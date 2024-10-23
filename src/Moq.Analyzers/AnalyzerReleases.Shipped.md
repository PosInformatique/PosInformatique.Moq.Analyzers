## Release 1.11.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
PosInfoMoq2014 | Compilation | Error | The `Callback()` delegate expression must not return a value.
PosInfoMoq2015 | Compilation | Error | The `Protected().Setup()` method must match the return type of the mocked method.

## Release 1.10.0

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
PosInfoMoq2012 | Compilation | Error | The delegate in the argument of the `Returns()` method must return a value with same type of the mocked method.
PosInfoMoq2013 | Compilation | Error | The delegate in the argument of the `Returns()`/`ReturnsAsync()` method must have the same parameter types of the mocked method/property.

## Release 1.9.1

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
PosInfoMoq2009 | Compilation | Error | `Mock.Of<T>` method must be used only to mock non-sealed class.
PosInfoMoq2010 | Compilation | Error | `Mock.Of<T>` method must be used only with types that contains parameterless contructor.
PosInfoMoq2011 | Compilation | Error | Constructor of the mocked class must be accessible.

## Release 1.8.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq1003 | Design | Warning | The `Callback()` method should be used to check the parameters when mocking a method with `It.IsAny<T>()` arguments.
PosInfoMoq1004 | Design | Warning | The `Callback()` parameter should not be ignored if it has been setup as an `It.IsAny<T>()` argument.

## Release 1.7.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq1002 | Design | Warning | `Verify()` methods should be called when `Verifiable()` has been setup.

## Release 1.6.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq2007 | Compilation | Error | The `As<T>()` method can be used only with interfaces.
PosInfoMoq2008 | Compilation | Error | The `Verify()` method can be used only on overridable members.

## Release 1.5.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq2004 | Compilation | Error | Constructor arguments cannot be passed for interface mocks.
PosInfoMoq2005 | Compilation | Error | Constructor arguments must match the constructors of the mocked class.
PosInfoMoq2006 | Compilation | Error | The `Protected().Setup()` method must be use with overridable protected or internal methods.

## Release 1.3.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
PosInfoMoq2003  | Compilation | Error  | The `Callback()` delegate expression must match the signature of the mocked method.

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
PosInfoMoq1000  | Design   | Warning  | `VerifyAll()` method should be called when instantiate a Mock<T> instances.
PosInfoMoq1001  | Design   | Warning  | The `Mock<T>` instance behavior should be defined to Strict mode.

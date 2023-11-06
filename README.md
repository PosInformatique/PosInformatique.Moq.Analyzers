# PosInformatique.Moq.Analyzers
PosInformatique.Moq.Analyzers is a library to verify syntax and code design when writing the unit tests using the [Moq](https://github.com/devlooped/moq) library.

## Installing from NuGet
The [PosInformatique.Moq.Analyzers](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/)
library is available directly on the
[![Nuget](https://img.shields.io/nuget/v/PosInformatique.Moq.Analyzers)](https://www.nuget.org/packages/PosInformatique.Moq.Analyzers/)
official website.

To download and install the library to your Visual Studio unit test projects use the following NuGet command line 

```
Install-Package PosInformatique.Moq.Analyzers
```

The analyzer is automatically added and activated with their default severity levels.

## Rules

This section describes the list of the rules analyzed by the library to improve code quality of the unit tests using
the [Moq](https://github.com/devlooped/moq) library.

### Design

Design rules used to make your unit tests more strongly strict.

| Rule | Description |
| - | - |
| [Moq1000: `Verify()` and `VerifyAll()` methods should be called when instantiate a `Mock<T>` instances](docs/design/Moq1000.md) | When instantiating a `Mock<T>` in the *Arrange* phase of an unit test, `Verify()` or `VerifyAll()` method should be called in the *Assert* phase to check the setup methods has been called. |
| [Moq1001: The `Mock<T>` instance behavior should be defined to Strict mode](docs/design/Moq1001.md) | When instantiating a `Mock<T>` instance, the `MockBehavior` of the `Mock` instance should be defined to `Strict`. |


### Compilation

Compilation rules check some error during the compilation to be sure that the execution of the unit tests with `Mock<T>` will not raise exceptions.

| Rule | Description |
| - | - |
| [Moq2000: The `Returns()` or `ReturnsAsync()` methods must be call for Strict mocks](docs/design/Moq2000.md) | When a `Mock<T>` has been defined with the `Strict` behavior, the `Returns()` or `ReturnsAsync()` method must be called when setup a method to mock which returns a value. |

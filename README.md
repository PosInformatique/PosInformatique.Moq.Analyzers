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
| [PosInfoMoq1000: `Verify()` and `VerifyAll()` methods should be called when instantiate a `Mock<T>` instances](docs/Design/PosInfoMoq1000.md) | When instantiating a `Mock<T>` in the *Arrange* phase of an unit test, `Verify()` or `VerifyAll()` method should be called in the *Assert* phase to check the setup methods has been called. |
| [PosInfoMoq1001: The `Mock<T>` instance behavior should be defined to Strict mode](docs/Design/PosInfoMoq1001.md) | When instantiating a `Mock<T>` instance, the `MockBehavior` of the `Mock` instance should be defined to `Strict`. |


### Compilation

Compilation rules check some error during the compilation to be sure that the execution of the unit tests with `Mock<T>` will not raise exceptions.
All the rules of this category should not be disabled (or changed their severity differently of **Error**).

| Rule | Description |
| - | - |
| [PosInfoMoq2000: The `Returns()` or `ReturnsAsync()` methods must be call for Strict mocks](docs/Compilation/PosInfoMoq2000.md) | When a `Mock<T>` has been defined with the `Strict` behavior, the `Returns()` or `ReturnsAsync()` method must be called when setup a method to mock which returns a value. |
| [PosInfoMoq2001: The `Setup()` method must be used only on overridable members](docs/Compilation/PosInfoMoq2001.md)) | The `Setup()` method must be applied only for overridable members. |
| [PosInfoMoq2002: `Mock<T>` class can be used only to mock non-sealed class](docs/Compilation/PosInfoMoq2002.md) | The `Mock<T>` can mock only interfaces or non-`sealed` classes. |
| [PosInfoMoq2003: The `Callback()` delegate expression must match the signature of the mocked method](docs/Compilation/PosInfoMoq2003.md) | The delegate in the argument of the `Callback()` method must match the signature of the mocked method. |
| [PosInfoMoq2004: Constructor arguments cannot be passed for interface mocks](docs/Compilation/PosInfoMoq2004.md) | No arguments can be passed to a mocked interface. |
| [PosInfoMoq2005: Constructor arguments must match the constructors of the mocked class](docs/Compilation/PosInfoMoq2005.md) | When instantiating a `Mock<T>`, the parameters must match one of the constructors of the mocked type.  |
| [PosInfoMoq2006: The Protected().Setup() method must be use with overridable protected or internal methods](docs/Compilation/PosInfoMoq2006.md) | When using the `Protected().Setup()` configuration, the method mocked must be overridable and protected or internal. |


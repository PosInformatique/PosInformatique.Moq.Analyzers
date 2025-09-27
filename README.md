# PosInformatique.Moq.Analyzers
<div align="center">

[![Nuget](https://img.shields.io/nuget/v/PosInformatique.Moq.Analyzers)](https://www.nuget.org/packages/PosInformatique.Moq.Analyzers/)
[![NuGet downloads](https://img.shields.io/nuget/dt/PosInformatique.Moq.Analyzers)](https://www.nuget.org/packages/PosInformatique.Moq.Analyzers/)
[![License](https://img.shields.io/github/license/Nonanti/MathFlow?style=flat-square)](LICENSE)
[![Build Status](https://img.shields.io/github/actions/workflow/status/PosInformatique/PosInformatique.Moq.Analyzers/github-actions-ci.yaml?style=flat-square)](https://github.com/PosInformatique/PosInformatique.Moq.Analyzers/actions)
[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard-2.0-512BD4?style=flat-square)](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0)

</div>

PosInformatique.Moq.Analyzers is a set of analyzers to verify syntax and code design when writing the unit tests using the [Moq](https://github.com/devlooped/moq) library.

The analyzers are compiled against [.NET Standard 2.0](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0),
providing support for analyzing projects that target .NET Core or .NET Framework.

## 📦 Installing from NuGet
The [PosInformatique.Moq.Analyzers](https://www.nuget.org/packages/PosInformatique.FluentAssertions.Json/)
library is available directly on the
[![Nuget](https://img.shields.io/nuget/v/PosInformatique.Moq.Analyzers)](https://www.nuget.org/packages/PosInformatique.Moq.Analyzers/)
official website.

To download and install the library to your Visual Studio unit test projects use the following NuGet command line 

```
Install-Package PosInformatique.Moq.Analyzers
```

The analyzer is automatically added and activated with their default severity levels.

## 📋 Rules

This section describes the list of the rules analyzed by the library to improve code quality of the unit tests using
the [Moq](https://github.com/devlooped/moq) library.

### Design

Design rules used to make your unit tests more strongly strict.

| Rule | Description |
| - | - |
| [PosInfoMoq1000: `VerifyAll()` methods should be called when instantiate a `Mock<T>` instances](docs/Design/PosInfoMoq1000.md) | When instantiating a `Mock<T>` in the *Arrange* phase of an unit test, `VerifyAll()` method should be called in the *Assert* phase to check the setup methods has been called. |
| [PosInfoMoq1001: The mocked instances behaviors should be defined to `Strict` mode](docs/Design/PosInfoMoq1001.md) | When instantiating a `Mock<T>` instance, the `MockBehavior` of the `Mock` instance should be defined to `Strict`. |
| [PosInfoMoq1002: `Verify()` methods should be called when `Verifiable()` has been setup](docs/Design/PosInfoMoq1002.md) | When a mocked member has been setup with the `Verifiable()` method, the `Verify()` method must be called at the end of the unit test. |
| [PosInfoMoq1003: The `Callback()` method should be used to check the parameters when mocking a method with `It.IsAny<T>()` arguments](docs/Design/PosInfoMoq1003.md) | When a mocked method contains a `It.IsAny<T>()` argument, the related parameter should be checked in the `Callback()` method. |
| [PosInfoMoq1004: The `Callback()` parameter should not be ignored if it has been setup as an `It.IsAny<T>()` argument](docs/Design/PosInfoMoq1004.md) | When a mocked method contains a `It.IsAny<T>()` argument, the related parameter should not be ignored in the `Callback()` method. |
| [PosInfoMoq1005: Defines the generic argument of the `SetupSet()` method with the type of the mocked property](docs/Design/PosInfoMoq1005.md) | When mocking the setter of a property, use the `SetupSet<TProperty>()` method version. |
| [PosInfoMoq1006: The `It.IsAny<T>()` or `It.Is<T>()` arguments must match the parameters of the mocked method.](docs/Design/PosInfoMoq1006.md) | When setting up a method using `It.IsAny<T>()` or `It.Is<T>()` as arguments, the type `T` must exactly match the parameters of the configured method. |
| [PosInfoMoq1007: The `Verify()` method must specify the `Times` argument.](docs/Design/PosInfoMoq1007.md) | When calling the `Verify()` method, if the `Times` argument is not specified, Moq will assume `Times.AtLeastOnce()` by default. |

### Compilation

Compilation rules check some error during the compilation to be sure that the execution of the unit tests with `Mock<T>` will not raise exceptions.
All the rules of this category should not be disabled (or changed their severity differently of **Error**).

| Rule | Description |
| - | - |
| [PosInfoMoq2000: The `Returns()` or `ReturnsAsync()` methods must be call for Strict mocks](docs/Compilation/PosInfoMoq2000.md) | When a `Mock<T>` has been defined with the `Strict` behavior, the `Returns()` or `ReturnsAsync()` method must be called when setup a method to mock which returns a value. |
| [PosInfoMoq2001: The `Setup()`/`SetupSet()` method must be used only on overridable members](docs/Compilation/PosInfoMoq2001.md)) | The `Setup()` method must be applied only for overridable members. |
| [PosInfoMoq2002: `Mock<T>` class can be used only to mock non-sealed class](docs/Compilation/PosInfoMoq2002.md) | The `Mock<T>` class can mock only interfaces or non-`sealed` classes. |
| [PosInfoMoq2003: The `Callback()` delegate expression must match the signature of the mocked method](docs/Compilation/PosInfoMoq2003.md) | The delegate in the argument of the `Callback()` method must match the signature of the mocked method. |
| [PosInfoMoq2004: Constructor arguments cannot be passed for interface mocks](docs/Compilation/PosInfoMoq2004.md) | No arguments can be passed to a mocked interface. |
| [PosInfoMoq2005: Constructor arguments must match the constructors of the mocked class](docs/Compilation/PosInfoMoq2005.md) | When instantiating a `Mock<T>`, the parameters must match one of the constructors of the mocked type.  |
| [PosInfoMoq2006: The Protected().Setup() method must be use with overridable protected or internal methods](docs/Compilation/PosInfoMoq2006.md) | When using the `Protected().Setup()` configuration, the method mocked must be overridable and protected or internal. |
| [PosInfoMoq2007: The `As<T>()` method can be used only with interfaces.](docs/Compilation/PosInfoMoq2007.md) | The `As<T>()` can only be use with the interfaces. |
| [PosInfoMoq2008: The `Verify()` method must be used only on overridable members](docs/Compilation/PosInfoMoq2008.md)) | The `Verify()` method must be applied only for overridable members. |
| [PosInfoMoq2009: `Mock.Of<T>` method must be used only to mock non-sealed class](docs/Compilation/PosInfoMoq2009.md) | The `Mock.Of<T>` method can mock only interfaces or non-`sealed` classes |
| [PosInfoMoq2010: `Mock.Of<T>` method must be used only with types that contains parameterless contructor](docs/Compilation/PosInfoMoq2010.md) | The `Mock.Of<T>` method requires a non-private parameterless contructor |
| [PosInfoMoq2011: Constructor of the mocked class must be accessible.](docs/Compilation/PosInfoMoq2011.md) | The constructor of the instantiate mocked class must non-private. |
| [PosInfoMoq2012: The delegate in the argument of the `Returns()` method must return a value with same type of the mocked method.](docs/Compilation/PosInfoMoq2012.md) | The lambda expression, anonymous method or method in the argument of the `Returns()` must return return a value of the same type as the mocked method or property. |
| [PosInfoMoq2013: The delegate in the argument of the `Returns()`/`ReturnsAsync()` method must have the same parameter types of the mocked method/property.](docs/Compilation/PosInfoMoq2013.md) | The lambda expression, anonymous method or method in the argument of the `Returns()`/`ReturnsAsync()` must have the same arguments type of the mocked method or property. |
| [PosInfoMoq2014: The `Callback()` delegate expression must not return a value.](docs/Compilation/PosInfoMoq2014.md) | The `Callback()` delegate expression must not return a value. |
| [PosInfoMoq2015: The `Protected().Setup()` method must match the return type of the mocked method](docs/Compilation/PosInfoMoq2015.md) | The method setup with `Protected().Setup()` must match the return type of the mocked method. |
| [PosInfoMoq2016: `Mock<T>` constructor with factory lambda expression can be used only with classes.](docs/Compilation/PosInfoMoq2016.md) | The factory lambda expression used in `Mock<T>` instantiation must used only for the classes. |




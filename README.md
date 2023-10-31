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

The analyzers are automatically added and activated with their default warning levels.

## Rules

This section describes the list of the rules analyzed by the library to improve code quality of the unit tests using
the [Moq](https://github.com/devlooped/moq) library.

### Design

Design rules used to make your unit tests more strongly strict.

| Rule | Description |
| - | - |
| [MQ1000: Verify() or VerifyAll() methods should be called when instantiate a Mock<T> instances](docs/design/MQ1000.md) | When a static member of a generic type is called, the type argument must be specified for the type. When a generic instance member that does not support inference is called, the type argument must be specified for the member. In these two cases, the syntax for specifying the type argument is different and easily confused. |
| [MQ1001: The Mock<T> instance behavior should be defined to Strict mode](docs/design/MQ1001.md) | A class declares and implements an instance field that is a System.IDisposable type and the class does not implement IDisposable. A class that declares an IDisposable field indirectly owns an unmanaged resource and should implement the IDisposable interface. |

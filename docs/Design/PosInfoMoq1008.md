# PosInfoMoq1008: The `Mock.Verify()` and `Mock.VerifyAll()` methods must specify at least one mock

| Property            | Value                                                                                    |
|---------------------|------------------------------------------------------------------------------------------|
| **Rule ID**         | PosInfoMoq1008                                                                           |
| **Title**           | The `Mock.Verify()` and `Mock.VerifyAll()` methods must specify at least one mock        |
| **Category**        | Design                                                                                   |
| **Default severity**| Warning                                                                                  |

## Cause

When calling the static methods `Mock.Verify()` or `Mock.VerifyAll()` without providing any `Mock<T>` instances, no verification is performed.

## Rule description

The static methods `Mock.Verify()` and `Mock.VerifyAll()` are designed to verify multiple `Mock<T>` instances at once.  
However, calling these methods without specifying any `Mock<T>` instances results in no verification being performed,
which makes the test ineffective and potentially misleading.

For example, the following code calls `Mock.Verify()` without any arguments:

```csharp
[Fact]
public void ProcessData_ShouldVerifyMocks()
{
    var repositoryMock = new Mock<IRepository>();
    var serviceMock = new Mock<IService>();

    var processor = new DataProcessor(repositoryMock.Object, serviceMock.Object);
    processor.ProcessData();

    // This does nothing - no mocks are verified
    Mock.Verify();
}
```

This code is misleading because it appears to verify something, but actually performs no verification at all.

Instead, you should specify the `Mock<T>` instances you want to verify:

```csharp
[Fact]
public void ProcessData_ShouldVerifyMocks()
{
    var repositoryMock = new Mock<IRepository>();
    var serviceMock = new Mock<IService>();

    var processor = new DataProcessor(repositoryMock.Object, serviceMock.Object);
    processor.ProcessData();

    // Clear and explicit - verifies both mocks
    Mock.Verify(repositoryMock, serviceMock);
}
```

The same applies to `Mock.VerifyAll()`:

```csharp
// Wrong: no verification performed
Mock.VerifyAll();

// Correct: verifies all setups on the specified mocks
Mock.VerifyAll(repositoryMock, serviceMock);
```

## How to fix violations

To fix a violation of this rule, **always provide at least one mock instance** when calling `Mock.Verify()` or `Mock.VerifyAll()`.

Examples:
- `Mock.Verify(mockA)`  
- `Mock.Verify(mockA, mockB)`  
- `Mock.VerifyAll(mockA, mockB, mockC)`  

## When to suppress warnings

You should generally not suppress warnings from this rule, as calling these methods without arguments serves no purpose.  
If you need to verify individual mocks, use the instance methods `mock.Verify()` or `mock.VerifyAll()` instead.

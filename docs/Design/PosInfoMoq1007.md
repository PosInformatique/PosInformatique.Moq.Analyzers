# PosInfoMoq1007: The `Verify()` method must specify the `Times` argument

| Property            | Value                                                                 |
|---------------------|-----------------------------------------------------------------------|
| **Rule ID**         | PosInfoMoq1007                                                        |
| **Title**           | The `Verify()` method must specify the `Times` argument                 |
| **Category**        | Design                                                                |
| **Default severity**| Warning                                                               |

## Cause

When calling the `Verify()` method, if the `Times` argument is not specified, Moq will assume `Times.AtLeastOnce()` by default.

## Rule description

Although Moq provides a default behavior (`Times.AtLeastOnce()`), relying on it can make test intentions unclear or ambiguous. 
Specifying the `Times` argument explicitly improves readability and ensures that test failures are interpreted correctly.

For example, the following code omits the `Times` argument:

```csharp
[Fact]
public void SaveUser_ShouldBeCalled()
{
    var repository = new Mock<IRepository>();
    var service = new UserService(repository.Object);

    service.Save(new User());

    // Ambiguous: defaults internally to Times.AtLeastOnce()
    repository.Verify(x => x.Save(It.IsAny<User>()));
}
```

This code works the same as:

```csharp
repository.Verify(x => x.Save(It.IsAny<User>()), Times.AtLeastOnce());
```

However, omitting the `Times` argument makes it less obvious to a reader what the expected behavior is.

Instead, you should write the `Times` explicitly:

```csharp
[Fact]
public void SaveUser_ShouldBeCalledOnce()
{
    var repository = new Mock<IRepository>();
    var service = new UserService(repository.Object);

    service.Save(new User());

    // Clear and explicit
    repository.Verify(x => x.Save(It.IsAny<User>()), Times.Once());
}
```

## How to fix violations

To fix a violation of this rule, **always specify the `Times` argument explicitly** when calling `Verify()` method.

Examples:
- `Times.Once()`  
- `Times.Never()`  
- `Times.AtLeast(2)`  
- `Times.Exactly(3)`  

## When to suppress warnings

You may suppress warnings from this rule if you are fine with the implicit default of `Times.AtLeastOnce()`.  
However, for readability and maintainability, it is strongly recommended to always be explicit with the `Times` argument.

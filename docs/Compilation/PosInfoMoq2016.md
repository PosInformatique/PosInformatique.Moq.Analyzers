# PosInfoMoq2016: `Mock<T>` constructor with factory lambda expression can be used only with classes.

| Property                            | Value																    |
|-------------------------------------|-------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2016														    |
| **Title**                           | `Mock<T>` constructor with factory lambda expression can be used only with classes.  |
| **Category**                        | Compilation															    |
| **Default severity**				  | Error																    |

## Cause

The factory lambda expression used in `Mock<T>` instantiation must used only for the classes.

## Rule description

When using a lambda expression in the constructor of Mock<T> to create a mock instance, the mocked type must be a class.

```csharp
[Fact]
public void Test()
{
    var service1 = new Mock<IService>(() => new Service());    // The factory lambda expression can be used only on classes type.
    var service2 = new Mock<Service>(() => new Service());     // OK
}

public interface IService
{
}

public class Service : IService:
{
    public Service(string a)
    {
    }
}
```

## How to fix violations

To fix a violation of this rule, ensure that the lambda expression factory is used with a mocked type that is a class.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `ArgumentException`
thrown with the *"Constructor arguments cannot be passed for interface mocks."* message.

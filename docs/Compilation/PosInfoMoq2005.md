# PosInfoMoq2005: The `Callback()` delegate expression must match the signature of the mocked method

| Property                            | Value																    |
|-------------------------------------|-------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2005														    |
| **Title**                           | Constructor arguments must match the constructors of the mocked class.  |
| **Category**                        | Compilation															    |
| **Default severity**				  | Error																    |

## Cause

Constructor arguments must match the constructors of the mocked class.

## Rule description

When configurate mock, all the arguments must match one of the constructor of the mocked type.

```csharp
[Fact]
public void Test()
{
    var service1 = new Mock<Service>(1, 2, 3);                              // The arguments does not match one of the Service type constructors.
    var service2 = new Mock<Service>("Argument 1", 2);                      // OK
    var service3 = new Mock<Service>(MockBehavior.Strict, "Argument 1", 2); // OK
}

public abstract class Service
{
    public Service(string a)
    {
    }

    public Service(string a, int b)
    {
    }
}
```

## How to fix violations

To fix a violation of this rule, be sure to pass right arguments of one of the constructor of the mocked instance.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Constructor arguments must match the constructors of the mocked class."* message.

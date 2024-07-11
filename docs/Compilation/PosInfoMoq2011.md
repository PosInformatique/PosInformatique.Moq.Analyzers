# PosInfoMoq2011: Constructor of the mocked class must be accessible.

| Property                            | Value																    |
|-------------------------------------|-------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2011														    |
| **Title**                           | Constructor of the mocked class must be accessible.  |
| **Category**                        | Compilation															    |
| **Default severity**				  | Error																    |

## Cause

Constructor of the mocked class must be accessible (`public`, `protected` or `internal`)

## Rule description

The mocked class must not contain an inaccessible constructor.

```csharp
[Fact]
public void Test()
{
    var service1 = new Mock<Service>("Hello");                              // The constructor invoked is private.
    var service2 = new Mock<Service>("Argument 1", 2);                      // OK
    var service3 = new Mock<Service>(MockBehavior.Strict, "Argument 1", 2); // OK
}

public abstract class Service
{
    private Service(string a)
    {
    }

    public Service(string a, int b)
    {
    }
}
```

## How to fix violations

To fix a violation of this rule, be sure to call a non-private constructor when instantiate a mocked class.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `NotSupportedException`
thrown with the *"Parent does not have a default constructor. The default constructor must be explicitly defined."* message.

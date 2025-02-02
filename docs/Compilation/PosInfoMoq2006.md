# PosInfoMoq2006: The Protected().Setup() method must be use with overridable protected or internal methods

| Property                            | Value																                         |
|-------------------------------------|----------------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2006														                         |
| **Title**                           | The `Protected().Setup()` method must be use with overridable protected or internal methods. |
| **Category**                        | Compilation															                         |
| **Default severity**				  | Error																                         |

## Cause

A `Protected().Setup()` reference a method in the mocked type which is:
- Not existing in the mocked type.
- Not overridable (`abstract`, `virtual` or `override`, but not `sealed`).
- Not `protected` or `internal`.

## Rule description

When using the `Protected().Setup()`, the method mocked must be `protected`, `internal` or `protected internal`,
and must be overridable (`virtual`, `abstract` and `override`, but not `sealed`).

```csharp
[Fact]
public void Test()
{
    var service = new Mock<Service>();
    service.Protected().Setup("GetData")            // The GetData() is public and can't be mocked with Protected() feature.
        .Returns(10);
    service.Protected().Setup("NotExists")          // The NotExists() method does not exist.
        .Returns(10);
    service.Protected().Setup("YouCantOverrideMe")  // The YouCantOverrideMe() is not virtual or abstract.
        .Returns(10);
}

public abstract class Service
{
    public abstract int GetData();

    protected void YouCantOverrideMe() { };
}
```

## How to fix violations

To fix a violation of this rule, use the `Protected().Setup()` to mock method which are:
- `protected`
- `internal`
- `protected internal`
- Overridable (`virtual`, `abstract` or `override`, but not `sealed`).

Else use the standard mocking feature without the `Protected()` method.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Method X.xxxx is public. Use strong-typed."* message.

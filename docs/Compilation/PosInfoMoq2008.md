# PosInfoMoq2008: The `Verify()` method must be used only on overridable members

| Property                            | Value                                                         |
|-------------------------------------|---------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq2008                                                |
| **Title**                           | The `Verify()` method must be used only on overridable members |
| **Category**                        | Compilation													  |
| **Default severity**				  | Error														  |

## Cause

The `Verify()` method must be applied only for overridable members.
An overridable member is a **method** or **property** which is in:
- An `interface`.
- A non-`sealed` `class`. In this case, the member must be:
    - Defines as `abstract`.
	- Or defined as `virtual`

## Rule description

The `Verify()` method must be applied only for overridable members.

For example, the following methods and properties can be mock and used in the `Verify()` method:
- `IService.MethodCanBeMocked()`
- `IService.PropertyCanBeMocked`
- `Service.VirtualMethodCanBeMocked`
- `Service.VirtualPropertyCanBeMocked`
- `Service.AbstractMethodCanBeMocked`
- `Service.AbstractPropertyCanBeMocked`

```csharp
public interface IService
{
	void MethodCanBeMocked();

	string PropertyCanBeMocked { get; set; }
}

public abstract class Service
{
	public virtual void VirtualMethodCanBeMocked() { ... }

	public virtual void VirtualPropertyCanBeMocked() { ... }

	public abstract void AbstractMethodCanBeMocked();

	public abstract void AbstractPropertyCanBeMocked();
}
```

> **NOTE**: The extension methods can not be overriden. The C# syntax looks like a member method an interface or class, but the extension method are just simple
static methods which can not be overriden.

## How to fix violations

To fix a violation of this rule, be sure to mock a member in the `Verify()` method which can be overriden.

## When to suppress warnings

Do not suppress an error from this rule. If bypassed, the execution of the unit test will be failed with a `MoqException`
thrown with the *"Unsupported expression: m => m.Method(). Non-overridable members (here: Namespace.Class.Method) may not be used in setup / verification expressions."* message.

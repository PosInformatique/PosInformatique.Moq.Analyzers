# PosInfoMoq2020: `CallBase()` must be used only on virtual or override members

| Property              | Value                                                                 |
|-----------------------|----------------------------------------------------------------------|
| **Rule ID**           | PosInfoMoq2020                                                       |
| **Title**             | `CallBase()` must be used only on virtual or override members        |
| **Category**          | Compilation                                                          |
| **Default severity**  | Error                                                                |

## Cause

The `CallBase()` method is used on a `Setup()` of a member which is not `virtual` or `override`
(standard, `abstract`, `sealed` or interface members).

## Rule description

`CallBase()` calls the real implementation of the mocked member. Only `virtual` or `override` members
(not `abstract`, not `sealed`) provide an implementation that Moq can call.

## How to fix violations

Remove the `CallBase()` call, or make the member `virtual`.

## Example

```csharp
public class MyClass
{
	public virtual int VirtualMethod() => 1;

	public int Method() => 1;
}

var mock = new Mock<MyClass>();

mock.Setup(m => m.VirtualMethod()).CallBase(); // Valid

mock.Setup(m => m.Method()).CallBase(); // PosInfoMoq2020
```

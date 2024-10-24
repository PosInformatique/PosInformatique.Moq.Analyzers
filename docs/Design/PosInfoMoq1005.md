# PosInfoMoq1005: Defines the generic argument of the `SetupSet()` method with the type of the mocked property.

| Property                            | Value                                                                                                |
|-------------------------------------|------------------------------------------------------------------------------------------------------|
| **Rule ID**                         | PosInfoMoq1005                                                                                       |
| **Title**                           | Defines the generic argument of the `SetupSet()` method with the type of the mocked property.        |
| **Category**                        | Design																                                 |
| **Default severity**				  | Warning																                                 |

## Cause

A property setter has been set up using `SetupSet()` without a generic argument that represents the type of the mocked property.

## Rule description

Moq provides two methods to mock a property setter:
- `Mock<T>.SetupSet(Action<T>)`
- `Mock<T>.SetupSet<TProperty>(Action<T, TProperty>)`

When setting up a property setter, use `Mock<T>.SetupSet<TProperty>(Action<T, TProperty>)` by explicitly defining the type of the property to mock.
This overload of the `SetupSet()` method allows you to define a typed `Callback()` and avoid exceptions if the delegate argument in the `Callback()`
does not match the property type.

For example, consider the following code to test:

```csharp
[Fact]
public interface Customer
{
	string Name { get; set; }
}
```

If you mock the setter of the `Customer.Name` property, you should set up the property with the `SetupSet<string>()` method:

```csharp
[Fact]
public void SetNameOfCustomer()
{
	var customer = new Mock<Customer>();
	customer.SetupSet<string>(c => c.Name = "Gilles")   // The SetupSet<string>() version is used.
		.Callback((string value) =>
		{
			// Called when the setter of the property is set.
		});
}
```

The following code violates the rule because the `SetupSet()` method has no generic argument:

```csharp
[Fact]
public void SetNameOfCustomer()
{
	var customer = new Mock<Customer>();
	customer.SetupSet(c => c.Name = "Gilles")		// The SetupSet() has been used without set the generic argument.
		.Callback((string value) =>
		{
			// Called when the setter of the property is set.
		});
}
```

If the non-generic version of the `SetupSet()` method is used, the delegate in the `Callback()` method cannot be checked at compile time,
an exception will occur during the execution of the unit test:

```csharp
[Fact]
public void SetNameOfCustomer()
{
	var customer = new Mock<Customer>();
	customer.SetupSet(c => c.Name = "Gilles")
		.Callback((int value) =>		// The code compiles, but during the execution of the unit test
		{                               // an ArgumentException will be thrown.
		});
}
```

## How to fix violations

To fix a violation of this rule, use the `SetupSet<TProperty>()` method with the type of the mocked property as the generic argument.

## When to suppress warnings

Do not suppress a warning from this rule. Using the `SetupSet<T>()` method ensures that the delegate argument in the `Callback()`
method matches the type of the property.
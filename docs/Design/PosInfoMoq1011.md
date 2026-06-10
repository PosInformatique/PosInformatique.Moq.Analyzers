# PosInfoMoq1011: The `Setup()` expression must explicitly specify optional argument values

| Property             | Value                                                                 |
|----------------------|-----------------------------------------------------------------------|
| **Rule ID**          | PosInfoMoq1011                                                       |
| **Title**            | The `Setup()` expression must explicitly specify optional argument values |
| **Category**         | Design                                                               |
| **Default severity** | Warning                                                              |

## Cause

A `Setup()` expression uses omitted optional arguments instead of explicitly specifying their default values.

## Rule description

When setting up a method or member that declares optional arguments, the optional values must be written explicitly in the `Setup()` expression, even if C# allows omitting them.

For example, if a mocked interface declares the following method:

```csharp
public interface ICalculator
{
    void Method(int a = 10);
}
```

The following setup is valid in C#, but it is not accepted by this rule:

```csharp
calculatorMock.Setup(c => c.Method());
```

Instead, the default value must be written explicitly:

```csharp
calculatorMock.Setup(c => c.Method(10));
```

This makes the intended behavior of the test clearer and avoids relying on the compiler-generated default argument expansion inside the lambda expression.

## How to fix violations

To fix a violation of this rule, update the `Setup()` expression so that every optional argument is explicitly passed with its default value.

## When to suppress warnings

Do not suppress warnings from this rule. Optional arguments should be written explicitly in `Setup()` expressions for clarity and consistency.

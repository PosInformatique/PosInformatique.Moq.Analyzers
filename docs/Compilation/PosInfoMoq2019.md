# PosInfoMoq2019: `RaiseAsync()` must be used only for events with async handlers (returning `Task`)

| Property              | Value                                                                 |
|-----------------------|----------------------------------------------------------------------|
| **Rule ID**           | PosInfoMoq2019                                                       |
| **Title**             | `RaiseAsync()` must be used only for events with async handlers (returning `Task`) |
| **Category**          | Compilation                                                          |
| **Default severity**  | Error                                                                |

## Cause

`Mock<T>.RaiseAsync()` must only be used with events whose delegate type returns `Task` (i.e., async events).
Using `RaiseAsync()` on events whose handlers return `void` (or any non-`Task` type) is invalid.

## Rule description

Moq provides `RaiseAsync()` to trigger asynchronous events. This requires that the event delegate returns `Task` (or `Task<T>`).
If the event delegate returns `void` (like `EventHandler` or `EventHandler<T>`), `RaiseAsync()` is not appropriate and will cause runtime issues.

### Delegate examples

```csharp
// Async event delegate
public delegate Task AsyncEventHandler(object sender, EventArgs e);

// Classic sync delegates (return void)
public delegate void VoidEventHandler(object sender, EventArgs e);
public class DataEventArgs : EventArgs { public int Value { get; } public DataEventArgs(int v) => Value = v; }
```

### Mocked type with both kinds of events

```csharp
public class Service
{
    public event AsyncEventHandler AsyncChanged;                 // returns Task
    public event EventHandler SyncChanged;                       // returns void
    public event EventHandler<DataEventArgs> SyncDataChanged;    // returns void
}
```

### Correct usage (async delegate returning Task)

```csharp
var serviceMock = new Mock<Service>();

// Use RaiseAsync with async event (Task-returning delegate)
await serviceMock.RaiseAsync(s => s.AsyncChanged += null, EventArgs.Empty);
```

### Incorrect usage (void-returning delegates)

```csharp
var serviceMock = new Mock<Service>();

// EventHandler returns void — using RaiseAsync is invalid
await serviceMock.RaiseAsync(s => s.SyncChanged += null, EventArgs.Empty); // ❌

// EventHandler<T> returns void — using RaiseAsync is invalid
await serviceMock.RaiseAsync(s => s.SyncDataChanged += null, new DataEventArgs(42)); // ❌
```

## How to fix violations

- If the event delegate returns `void`, use `Raise()` instead of `RaiseAsync()`.
- If you need async semantics, change the event to use a `Task`-returning delegate, for example `AsyncEventHandler`.

### Fix examples

```csharp
// Using Raise() for void-returning events
serviceMock.Raise(s => s.SyncChanged += null, EventArgs.Empty);
serviceMock.Raise(s => s.SyncDataChanged += null, new DataEventArgs(42));

// Using RaiseAsync() for Task-returning events
await serviceMock.RaiseAsync(s => s.AsyncChanged += null, EventArgs.Empty);
```

## When to suppress warnings

Do not suppress this rule.
If disabled, Moq may throw a runtime exception (`NullReferenceException`) due to a known bug when `RaiseAsync()` is used on non-async events,
instead of providing a clear error message.

Reference: https://github.com/devlooped/moq/issues/1568

# PleaseWait

[![CI](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml)
[![CodeQL](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/PleaseWait?color=blue)](https://www.nuget.org/packages/PleaseWait)
[![NuGet](https://img.shields.io/nuget/dt/PleaseWait)](https://www.nuget.org/packages/PleaseWait)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

A lightweight, fluent Domain-Specific Language (DSL) for C# that simplifies testing asynchronous operations and waiting for conditions to occur. PleaseWait provides an intuitive API for synchronizing asynchronous systems without the complexity of manual thread management, timeouts, and concurrency handling.

## 🚀 Features

- **Fluent API**: Chain methods for readable, expressive test code
- **Flexible Timeouts**: Support for multiple time units (milliseconds, seconds, minutes, hours, days)
- **Exception Handling**: Configurable exception swallowing and silent failure modes
- **Prerequisite Actions**: Execute actions before checking conditions
- **Thread-Safe**: Built with thread safety in mind
- **Lightweight**: Minimal dependencies, focused on core functionality
- **Cross-Platform**: Targets .NET Standard 2.0 for broad compatibility

## 📦 Installation

Install the NuGet package:

```bash
dotnet add package PleaseWait
```

Or add to your `.csproj`:

```xml
<PackageReference Include="PleaseWait" Version="2.7.0" />
```

## 🎯 Quick Start

### Basic Usage

```csharp
using static PleaseWait.Dsl;
using static PleaseWait.TimeUnit;

[Test]
public void WaitForCondition()
{
    var orange = new Orange();
    _ = orange.PeelAsync(2); // Start async operation
    
    // Wait for the condition to be true
    Wait().AtMost(10, SECONDS).Until(() => orange.IsPeeled);
    
    Assert.That(orange.IsPeeled, Is.True);
}
```

### Advanced Usage

```csharp
[Test]
public void AdvancedWaitingExample()
{
    var orange = new Orange();
    
    Wait()
        .AtMost(5, SECONDS)
        .WithPollDelay(100, MILLIS)
        .WithPollInterval(200, MILLIS)
        .IgnoreExceptions()
        .WithPrereq(() => orange.Refresh())
        .Until(() => orange.IsPeeled && orange.CountSegments() > 8);
}
```

## 📚 API Examples

### Timeout Configuration

```csharp
// Using time units
Wait().AtMost(30, SECONDS).Until(condition);

// Using TimeSpan
Wait().AtMost(TimeSpan.FromMinutes(1)).Until(condition);

// Using the DSL
Wait().Timeout(30, SECONDS).Until(condition);
```

### Polling Configuration

```csharp
Wait()
    .AtMost(10, SECONDS)
    .WithPollDelay(100, MILLIS)    // Initial delay before first check
    .WithPollInterval(500, MILLIS) // Interval between subsequent checks
    .Until(condition);
```

### Exception Handling

```csharp
// Swallow exceptions (default behavior)
Wait().AtMost(5, SECONDS).Until(() => RiskyOperation());

// Re-throw exceptions
Wait().AtMost(5, SECONDS).IgnoreExceptions(false).Until(() => RiskyOperation());

// Fail silently (don't throw timeout exceptions)
Wait().AtMost(5, SECONDS).FailSilently().Until(() => condition);
```

### Prerequisite Actions

```csharp
// Single prerequisite action
Wait()
    .AtMost(10, SECONDS)
    .WithPrereq(() => RefreshData())
    .Until(() => DataIsReady());

// Multiple prerequisite actions
var actions = new List<Action> 
{ 
    () => RefreshData(),
    () => ClearCache(),
    () => LogStatus()
};

Wait()
    .AtMost(10, SECONDS)
    .WithPrereqs(actions)
    .Until(() => DataIsReady());
```

### Explicit Boolean Conditions

```csharp
// Wait for true
Wait().AtMost(5, SECONDS).UntilTrue(() => IsReady());

// Wait for false
Wait().AtMost(5, SECONDS).UntilFalse(() => IsLoading());

// Wait for specific boolean value
Wait().AtMost(5, SECONDS).Until(() => IsComplete(), expected: true);
```

### Cancellation Support

```csharp
using System.Threading;

// Basic cancellation
var cts = new CancellationTokenSource();
cts.CancelAfter(5000); // Cancel after 5 seconds

try
{
    Wait().AtMost(30, SECONDS)
        .Until(() => LongRunningOperation(), cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled!");
}

// User-initiated cancellation (e.g., in UI applications)
var userCancellation = new CancellationTokenSource();

// In a button click handler
userCancellation.Cancel();

Wait().AtMost(60, SECONDS)
    .Until(() => BackgroundTask(), cancellationToken: userCancellation.Token);

// Cancellation with UntilTrue/UntilFalse
Wait().AtMost(10, SECONDS)
    .UntilTrue(() => IsReady(), cts.Token);

Wait().AtMost(10, SECONDS)
    .UntilFalse(() => IsLoading(), cts.Token);

// Cancellation with prerequisites
Wait().AtMost(10, SECONDS)
    .WithPrereq(() => RefreshData())
    .Until(() => DataIsReady(), cancellationToken: cts.Token);
```

### Diagnostic Logging

PleaseWait supports diagnostic logging to help debug wait operations:

```csharp
using PleaseWait.Logging;

// Enable console logging
Wait()
    .WithLogger(new ConsoleLogger())
    .AtMost(10, SECONDS)
    .Until(() => SomeCondition());

// Enable debug logging (writes to System.Diagnostics.Debug)
Wait()
    .WithLogger(new DebugLogger())
    .AtMost(10, SECONDS)
    .Until(() => SomeCondition());

// Custom logger implementation
public class CustomLogger : IWaitLogger
{
    public void LogWaitStart(string condition, TimeSpan timeout) { /* ... */ }
    public void LogConditionCheck(string condition, bool result, TimeSpan elapsed) { /* ... */ }
    public void LogWaitSuccess(string condition, TimeSpan elapsed, int checks) { /* ... */ }
    public void LogTimeout(string condition, TimeSpan timeout) { /* ... */ }
    public void LogCancellation(string condition) { /* ... */ }
}

Wait()
    .WithLogger(new CustomLogger())
    .AtMost(10, SECONDS)
    .Until(() => SomeCondition());
```

**Console Logger Output:**
```
[PleaseWait] 🚀 Starting wait: condition (timeout: 10000ms)
[PleaseWait] ✗ Condition check: condition (elapsed: 100ms)
[PleaseWait] ✓ Condition check: condition (elapsed: 200ms)
[PleaseWait] ✅ Success: condition completed in 200ms (2 checks)
```

### Thread Sleep

```csharp
// Sleep for specific duration
Wait().Sleep(2, SECONDS);

// Sleep using TimeSpan
Wait().Sleep(TimeSpan.FromMilliseconds(500));
```

## 🧪 Testing Examples

### UI Testing
```csharp
[Test]
public void WaitForElementToBeVisible()
{
    var driver = new ChromeDriver();
    driver.Navigate().GoToUrl("https://example.com");
    
    Wait().AtMost(10, SECONDS)
        .Until(() => driver.FindElement(By.Id("login-button")).Displayed);
}
```

### API Testing
```csharp
[Test]
public async Task WaitForApiResponse()
{
    var client = new HttpClient();
    var response = await client.PostAsync("/api/process", content);
    
    Wait().AtMost(30, SECONDS)
        .Until(() => GetProcessingStatus(response.Id) == "completed");
}
```

### Database Testing
```csharp
[Test]
public void WaitForDatabaseUpdate()
{
    var repository = new UserRepository();
    repository.UpdateUserStatus(userId, "processing");
    
    Wait().AtMost(5, SECONDS)
        .Until(() => repository.GetUserStatus(userId) == "processed");
}
```

## 📋 Requirements

- **.NET Standard 2.0** or higher
- **C# 8.0** or higher

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Clone the repository
2. Install dependencies: `dotnet restore`
3. Run tests: `dotnet test`
4. Build: `dotnet build`

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## 📖 Documentation

- [Changelog](CHANGELOG.md) - Complete version history and changes
- [NuGet Package](https://www.nuget.org/packages/PleaseWait) - Package information and downloads

## 🙏 Acknowledgments

- Inspired by the need for better asynchronous testing patterns
- Built with performance and developer experience in mind
- Thanks to all contributors and users of this library

---

**Made with ❤️ for the .NET community**

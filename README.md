# PleaseWait

[![CI](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml)
[![CodeQL](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/PleaseWait?color=blue)](https://www.nuget.org/packages/PleaseWait)
[![NuGet](https://img.shields.io/nuget/dt/PleaseWait)](https://www.nuget.org/packages/PleaseWait)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## 📋 Table of Contents

- [Overview](#overview)
- [🚀 Features](#-features)
- [📦 Installation](#-installation)
- [🎯 Quick Start](#-quick-start)
  - [Basic Usage](#basic-usage)
  - [Advanced Usage](#advanced-usage)
- [📚 API Examples](#-api-examples)
  - [Timeout Configuration](#timeout-configuration)
  - [Polling Configuration](#polling-configuration)
  - [Exception Handling](#exception-handling)
  - [Prerequisite Actions](#prerequisite-actions)
  - [Explicit Boolean Conditions](#explicit-boolean-conditions)
  - [Cancellation Support](#cancellation-support)
  - [Diagnostic Logging](#diagnostic-logging)
  - [Performance Monitoring](#performance-monitoring)
  - [Wait Strategies](#wait-strategies)
  - [Global Configuration](#global-configuration)
  - [Thread Sleep](#thread-sleep)
- [🧪 Testing Examples](#-testing-examples)
  - [UI Testing](#ui-testing)
  - [API Testing](#api-testing)
  - [Database Testing](#database-testing)
- [📋 Requirements](#-requirements)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [📖 Documentation](#-documentation)

## Overview

A lightweight, fluent Domain-Specific Language (DSL) for C# that simplifies testing asynchronous operations and waiting for conditions to occur. PleaseWait provides an intuitive API for synchronizing asynchronous systems without the complexity of manual thread management, timeouts, and concurrency handling.

## 🚀 Features

- **Fluent API**: Chain methods for readable, expressive test code
- **Flexible Timeouts**: Support for multiple time units (milliseconds, seconds, minutes, hours, days)
- **Exception Handling**: Configurable exception swallowing and silent failure modes
- **Prerequisite Actions**: Execute actions before checking conditions
- **Cancellation Support**: Built-in CancellationToken support for graceful operation cancellation
- **Performance Monitoring**: Collect detailed metrics on wait operations and condition checks
- **Wait Strategies**: Choose from 5 different polling strategies (Linear, ExponentialBackoff, Aggressive, Conservative, Adaptive)
- **Diagnostic Logging**: Comprehensive logging system for debugging wait operations
- **Thread Sleep**: Explicit thread suspension with flexible time unit support
- **Lightweight**: Minimal dependencies, focused on core functionality
- **Cross-Platform**: Targets .NET Standard 2.0 for broad compatibility

⚠️ **IMPORTANT**: Thread safety and synchronization are the responsibility of the consuming application

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
    Wait().AtMost(10, Seconds).Until(() => orange.IsPeeled);
    
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
        .AtMost(5, Seconds)
        .WithPollDelay(100, Millis)
        .WithPollInterval(200, Millis)
        .IgnoreExceptions()
        .WithPrereq(() => orange.Refresh())
        .Until(() => orange.IsPeeled && orange.CountSegments() > 8);
}
```

## 📚 API Examples

### Timeout Configuration

```csharp
// Using time units
Wait().AtMost(30, Seconds).Until(condition);

// Using TimeSpan
Wait().AtMost(TimeSpan.FromMinutes(1)).Until(condition);

// Using the DSL
Wait().Timeout(30, Seconds).Until(condition);
```

### Polling Configuration

```csharp
Wait()
    .AtMost(10, Seconds)
    .WithPollDelay(100, Millis)    // Initial delay before first check
    .WithPollInterval(500, Millis) // Interval between subsequent checks
    .Until(condition);
```

### Exception Handling

```csharp
// Swallow exceptions (default behavior)
Wait().AtMost(5, Seconds).Until(() => RiskyOperation());

// Re-throw exceptions
Wait().AtMost(5, Seconds).IgnoreExceptions(false).Until(() => RiskyOperation());

// Fail silently (don't throw timeout exceptions)
Wait().AtMost(5, Seconds).FailSilently().Until(() => condition);
```

### Prerequisite Actions

```csharp
// Single prerequisite action
Wait()
    .AtMost(10, Seconds)
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
    .AtMost(10, Seconds)
    .WithPrereqs(actions)
    .Until(() => DataIsReady());
```

### Explicit Boolean Conditions

```csharp
// Wait for true
Wait().AtMost(5, Seconds).UntilTrue(() => IsReady());

// Wait for false
Wait().AtMost(5, Seconds).UntilFalse(() => IsLoading());

// Wait for specific boolean value
Wait().AtMost(5, Seconds).Until(() => IsComplete(), expected: true);
```

### Cancellation Support

```csharp
using System.Threading;

// Basic cancellation
var cts = new CancellationTokenSource();
cts.CancelAfter(5000); // Cancel after 5 seconds

try
{
    Wait().AtMost(30, Seconds)
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

Wait().AtMost(60, Seconds)
    .Until(() => BackgroundTask(), cancellationToken: userCancellation.Token);

// Cancellation with UntilTrue/UntilFalse
Wait().AtMost(10, Seconds)
    .UntilTrue(() => IsReady(), cts.Token);

Wait().AtMost(10, Seconds)
    .UntilFalse(() => IsLoading(), cts.Token);

// Cancellation with prerequisites
Wait().AtMost(10, Seconds)
    .WithPrereq(() => RefreshData())
    .Until(() => DataIsReady(), cancellationToken: cts.Token);
```

### Diagnostic Logging

PleaseWait supports diagnostic logging to help debug wait operations. By default, no logging occurs (using `NullLogger`). To enable logging, use one of the following approaches:

```csharp
using PleaseWait.Logging;

// Enable console logging
Wait()
    .WithLogger(new ConsoleLogger())
    .AtMost(10, Seconds)
    .Until(() => SomeCondition());

// Enable debug logging (writes to System.Diagnostics.Debug)
Wait()
    .WithLogger(new DebugLogger())
    .AtMost(10, Seconds)
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
    .AtMost(10, Seconds)
    .Until(() => SomeCondition());
```

**Console Logger Output:**
```
[PleaseWait] 🚀 Starting wait: condition (timeout: 10000ms)
[PleaseWait] ✗ Condition check: condition (elapsed: 100ms)
[PleaseWait] ✓ Condition check: condition (elapsed: 200ms)
[PleaseWait] ✅ Success: condition completed in 200ms (2 checks)
```

### Performance Monitoring

```csharp
// Enable metrics collection
var metrics = Wait()
    .WithMetrics()
    .AtMost(10, Seconds)
    .Until(() => SomeCondition());

// Analyze performance data
Console.WriteLine($"Checks: {metrics.ConditionChecks}");
Console.WriteLine($"Total time: {metrics.TotalTime}");
Console.WriteLine($"Average check time: {metrics.AverageCheckTime}");
Console.WriteLine($"Success: {metrics.WasSuccessful}");

// Use with logging for comprehensive diagnostics
Wait()
    .WithLogger(new ConsoleLogger())
    .WithMetrics()
    .Alias("Database Ready")
    .AtMost(30, Seconds)
    .Until(() => database.IsConnected);
```

**Metrics Output:**
```
[PleaseWait] 🚀 Starting wait: Database Ready (timeout: 30000ms)
[PleaseWait] ✗ Condition check: Database Ready (elapsed: 100ms)
[PleaseWait] ✓ Condition check: Database Ready (elapsed: 250ms)
[PleaseWait] ✅ Success: Database Ready completed in 250ms (2 checks)
[PleaseWait] 📊 Metrics: 2 checks, 250ms total, avg: 125ms, min: 100ms, max: 150ms
```

### Wait Strategies

Choose from different polling strategies to optimize for your specific use case:

```csharp
using static PleaseWait.Strategy.WaitStrategy;

// Linear (default) - consistent polling intervals
Wait()
    .WithStrategy(Linear)
    .AtMost(10, Seconds)
    .Until(() => condition);

// Exponential Backoff - increases delays over time
Wait()
    .WithStrategy(ExponentialBackoff)
    .AtMost(30, Seconds)
    .Until(() => database.IsReady());

// Aggressive - minimal delays for fast detection
Wait()
    .WithStrategy(Aggressive)
    .AtMost(5, Seconds)
    .Until(() => button.IsEnabled);

// Conservative - longer delays to minimize resource usage
Wait()
    .WithStrategy(Conservative)
    .AtMost(60, Seconds)
    .Until(() => heavyOperation.IsComplete());

// Adaptive - adjusts based on condition check performance
Wait()
    .WithStrategy(Adaptive)
    .WithMetrics()
    .AtMost(30, Seconds)
    .Until(() => condition);
```

**Strategy Details:**

| Strategy | Poll Delay | Poll Interval | Best For | Details |
|----------|------------|---------------|----------|---------|
| **Linear** | As configured | As configured | Predictable conditions with consistent timing | Uses configured values exactly as provided |
| **Exponential Backoff** | As configured | Increases exponentially (1x, 2x, 4x, 8x...) | Resource-intensive conditions that improve over time (database startup, service initialization) | Interval increases after each check, max 25% of timeout |
| **Aggressive** | 1/4 of configured | 1/4 of configured | UI testing, real-time monitoring, immediate response needed | Both delays reduced once at start, min 1ms interval |
| **Conservative** | 2x configured | 2x configured | Expensive operations, resource-constrained environments, reduced CPU usage | Both delays doubled once at start |
| **Adaptive** | As configured | Adjusts based on condition check performance | Conditions with varying performance characteristics that take multiple checks to resolve | Requires `WithMetrics()`, adjusts after each check: fast → 50% less, slow → 100% more |

### Global Configuration

PleaseWait supports global configuration to set default values for all wait operations. This is useful for setting application-wide defaults that apply to all `Wait()` instances.

```csharp
using static PleaseWait.TimeUnit;
using static PleaseWait.Strategy.WaitStrategy;

// Configure global defaults
Wait().Configure()
    .DefaultTimeout(30, Seconds)
    .DefaultPollDelay(200, Millis)
    .DefaultPollInterval(500, Millis)
    .DefaultLogger(new ConsoleLogger())
    .DefaultStrategy(Conservative)
    .DefaultIgnoreExceptions(true)
    .DefaultFailSilently(false)
    .DefaultPrerequisites(new List<Action> { () => RefreshData() });

// All subsequent Wait() calls will use these defaults
Wait().Until(() => SomeCondition()); // Uses 30s timeout, 200ms poll delay, etc.

// Individual instances can still override defaults
Wait()
    .AtMost(10, Seconds) // Override timeout
    .WithStrategy(Aggressive) // Override strategy
    .Until(() => SomeCondition());

// Reset to original defaults
Wait().ResetToDefaults();
```

**Global Configuration Options:**

| Method | Default Value | Description |
|--------|---------------|-------------|
| `DefaultTimeout(double, TimeUnit)` | 10 seconds | Default timeout for all wait operations |
| `DefaultPollDelay(double, TimeUnit)` | 100 milliseconds | Default initial delay before first condition check |
| `DefaultPollInterval(double, TimeUnit)` | 100 milliseconds | Default delay between condition checks |
| `DefaultLogger(IWaitLogger)` | `NullLogger` | Default logger for all wait operations |
| `DefaultStrategy(WaitStrategy)` | `Linear` | Default wait strategy for all operations |
| `DefaultIgnoreExceptions(bool)` | `false` | Whether to ignore exceptions during condition checks |
| `DefaultFailSilently(bool)` | `false` | Whether to return false instead of throwing on timeout |
| `DefaultPrerequisites(List<Action>)` | Empty list | Default actions to execute before each condition check |

**Best Practices:**
- Configure global defaults early in your application startup
- Use `ResetToDefaults()` in test teardown to prevent test interference
- Override defaults on individual instances when specific behavior is needed
- Consider using different configurations for different environments (dev, test, prod)

### Thread Sleep

```csharp
// Sleep for specific duration
Wait().Sleep(2, Seconds);

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
    
    Wait().AtMost(10, Seconds)
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
    
    Wait().AtMost(30, Seconds)
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
    
    Wait().AtMost(5, Seconds)
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
3. Build: `dotnet build`
4. Run tests: `dotnet test`

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

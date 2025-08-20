# PleaseWait

[![CI](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml)
[![CodeQL](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/codeql.yml)
[![NuGet](https://img.shields.io/nuget/v/PleaseWait?color=blue)](https://www.nuget.org/packages/PleaseWait)
[![NuGet](https://img.shields.io/nuget/dt/PleaseWait)](https://www.nuget.org/packages/PleaseWait)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/MartynNevers/please-wait/blob/main/LICENSE)

## 📋 Table of Contents

- [Overview](#overview)
- [🚀 Features](#-features)
- [📦 Installation](#-installation)
- [🎯 Quick Start](#-quick-start)
  - [Setup](#setup)
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
  - [Wait Options](#wait-options)
  - [Syntactic Sugar: With() and And()](#syntactic-sugar-with-and)
  - [Wait Strategies](#wait-strategies)
  - [Global Configuration](#global-configuration)
  - [Instance Configuration](#instance-configuration)
  - [Thread Sleep](#thread-sleep)
- [🧪 Testing Examples](#-testing-examples)
  - [UI Testing](#ui-testing)
  - [API Testing](#api-testing)
  - [Database Testing](#database-testing)
- [📋 Requirements](#-requirements)
- [💝 Support](#-support)
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
<PackageReference Include="PleaseWait" Version="*" />
```

## 🎯 Quick Start

### Setup

Add these using statements to your file for the cleanest PleaseWait experience:

```csharp
using PleaseWait.Logging;
using static PleaseWait.Dsl;
using static PleaseWait.TimeUnit;
using static PleaseWait.Strategy.WaitStrategy;
```

### Basic Usage

```csharp
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
        .PollDelay(100, Millis)
        .PollInterval(200, Millis)
        .IgnoreExceptions(true)
        .Prereq(() => orange.Refresh())
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
// Set both polling delay and interval in one call
Wait()
    .AtMost(10, Seconds)
    .Polling(100, Millis, 500, Millis)  // delay, interval
    .Until(condition);

// Or set them separately
Wait()
    .AtMost(10, Seconds)
    .PollDelay(100, Millis)    // Initial delay before first check
    .PollInterval(500, Millis) // Interval between subsequent checks
    .Until(condition);
```

### Exception Handling

```csharp
// Swallow exceptions (default behavior)
Wait().AtMost(5, Seconds).Until(() => RiskyOperation());

// Re-throw exceptions
Wait().AtMost(5, Seconds).IgnoreExceptions(false).Until(() => RiskyOperation());

// Fail silently (don't throw timeout exceptions)
Wait().AtMost(5, Seconds).FailSilently(true).Until(() => condition);

// Set both exception handling options in one call
Wait()
    .AtMost(10, Seconds)
    .ExceptionHandling(ignoreExceptions: true, failSilently: false)
    .Until(() => RiskyOperation());
```

### Prerequisite Actions

```csharp
// Single prerequisite action
Wait()
    .AtMost(10, Seconds)
    .Prereq(() => RefreshData())
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
    .Prereqs(actions)
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
    .Prereq(() => RefreshData())
    .Until(() => DataIsReady(), cancellationToken: cts.Token);
```

### Diagnostic Logging

PleaseWait supports diagnostic logging to help debug wait operations. By default, no logging occurs (using `NullLogger`). To enable logging, use one of the following approaches:

```csharp
// Enable console logging
Wait()
    .Logger(new ConsoleLogger())
    .AtMost(10, Seconds)
    .Until(() => SomeCondition());

// Enable debug logging (writes to System.Diagnostics.Debug)
Wait()
    .Logger(new DebugLogger())
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
    .Logger(new CustomLogger())
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
    .Metrics()
    .AtMost(10, Seconds)
    .Until(() => SomeCondition());

// Analyze performance data
Console.WriteLine($"Checks: {metrics.ConditionChecks}");
Console.WriteLine($"Total time: {metrics.TotalTime}");
Console.WriteLine($"Average check time: {metrics.AverageCheckTime}");
Console.WriteLine($"Success: {metrics.WasSuccessful}");

// Use with logging for comprehensive diagnostics
Wait()
    .Logger(new ConsoleLogger())
    .Metrics()
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

### Wait Options

The primary `Wait()` API provides a comprehensive set of options for configuring individual wait operations. These options can be chained together for fluent, readable code.

```csharp
// Basic wait with timeout
Wait().AtMost(10, Seconds).Until(() => condition);

// Advanced wait with multiple options
Wait()
    .AtMost(30, Seconds)
    .PollDelay(200, Millis)
    .PollInterval(500, Millis)
    .Strategy(ExponentialBackoff)
    .Logger(new ConsoleLogger())
    .Metrics(true)
    .IgnoreExceptions(true)
    .FailSilently(false)
    .Alias("Database Connection")
    .Prereq(() => RefreshConnection())
    .Until(() => database.IsConnected);
```

**Wait Options:**

| Method | Description |
|--------|-------------|
| `Timeout(double, TimeUnit)` | Set timeout for this wait operation |
| `AtMost(double, TimeUnit)` | → Alias for Timeout - sets the same timeout value |
| `PollDelay(double, TimeUnit)` | Set initial delay before first condition check |
| `PollInterval(double, TimeUnit)` | Set delay between condition checks |
| `Polling(double, TimeUnit, double, TimeUnit)` | → Combines PollDelay + PollInterval - sets both values in one call |
| `Strategy(WaitStrategy)` | Set wait strategy for this operation |
| `Logger(IWaitLogger)` | Set logger for this operation |
| `Metrics(bool)` | Enable metrics collection for this operation (default: true) |
| `IgnoreExceptions(bool)` | Set exception handling behavior (default: true) |
| `FailSilently(bool)` | Set fail silently behavior (default: true) |
| `ExceptionHandling(bool, bool)` | → Combines IgnoreExceptions + FailSilently - sets both values in one call |
| `Alias(string?)` | Set alias for this operation |
| `Prereq(Action?)` | Set a single prerequisite action (nullable) |
| `Prereqs(List<Action>?)` | Set multiple prerequisite actions |
| `Sleep(double, TimeUnit)` | Explicit thread sleep with time unit support |
| `With()` | Syntactic sugar for fluent chaining |
| `And()` | Syntactic sugar for fluent chaining |

**Note:** All time-based methods also have `TimeSpan` overloads (e.g., `AtMost(TimeSpan)`, `PollDelay(TimeSpan)`, `PollInterval(TimeSpan)`, `Polling(TimeSpan, TimeSpan)`, `Sleep(TimeSpan)`, `Timeout(TimeSpan)`).

**Wait Execution:**

| Method | Return Type | Description |
|--------|-------------|-------------|
| `Until(Func<bool>, bool, CancellationToken)` | `WaitMetrics?` | Wait for condition to return the expected boolean value (default: true) |
| `UntilTrue(Func<bool>, CancellationToken)` | `void` | Wait for condition to return true (convenience method) |
| `UntilFalse(Func<bool>, CancellationToken)` | `void` | Wait for condition to return false (convenience method) |

**Note:** The `CancellationToken` parameter defaults to `default` (no cancellation). You can call `Until(condition)` without the `expected` and `cancellationToken` parameters (defaults to `true` and `default` respectively).

**Benefits:**
- **Immediate**: Options take effect immediately for the current operation
- **Flexible**: Override any global defaults on a per-operation basis
- **Fluent**: Chain multiple options together for readable code
- **Non-Destructive**: Don't affect global configuration or other operations

### Syntactic Sugar: With() and And()

PleaseWait provides `With()` and `And()` methods as syntactic sugar to make your code more readable and expressive. These methods don't change any behavior - they simply return the current instance, allowing for more natural language-like chaining.

```csharp
// Without syntactic sugar - still perfectly valid
Wait()
    .AtMost(30, Seconds)
    .PollDelay(200, Millis)
    .PollInterval(500, Millis)
    .Strategy(ExponentialBackoff)
    .Metrics(true)
    .Alias("Database Connection")
    .Until(() => database.IsConnected);

// With syntactic sugar - more readable and expressive
Wait()
    .AtMost(30, Seconds)
    .With().PollDelay(200, Millis)
    .And().PollInterval(500, Millis)
    .With().Strategy(ExponentialBackoff)
    .And().Metrics(true)
    .With().Alias("Database Connection")
    .Until(() => database.IsConnected);
```

**Benefits of Using With() and And():**
- **Readability**: Makes the code read more like natural language
- **Grouping**: Helps visually group related configuration options
- **Expressiveness**: Makes the intent clearer in complex configurations
- **Consistency**: Available across all configuration contexts (Wait Options, Global Configuration, Instance Configuration)

**When to Use:**
- **Simple configurations**: Usually not needed for 2-3 options
- **Complex configurations**: Very helpful for 5+ options
- **Team preference**: Some teams prefer the more explicit style
- **Documentation**: Useful in examples to show relationships between options

**Note:** These methods are purely syntactic sugar and have no functional impact. Choose the style that makes your code most readable for your team.

### Wait Strategies

Choose from different polling strategies to optimize for your specific use case:

```csharp
// Linear (default) - consistent polling intervals
Wait()
    .Strategy(Linear)
    .AtMost(10, Seconds)
    .Until(() => condition);

// Exponential Backoff - increases delays over time
Wait()
    .Strategy(ExponentialBackoff)
    .AtMost(30, Seconds)
    .Until(() => database.IsReady());

// Aggressive - minimal delays for fast detection
Wait()
    .Strategy(Aggressive)
    .AtMost(5, Seconds)
    .Until(() => button.IsEnabled);

// Conservative - longer delays to minimize resource usage
Wait()
    .Strategy(Conservative)
    .AtMost(60, Seconds)
    .Until(() => heavyOperation.IsComplete());

// Adaptive - adjusts based on condition check performance
Wait()
    .Strategy(Adaptive)
    .Metrics(true)
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
| **Adaptive** | As configured | Adjusts based on condition check performance | Conditions with varying performance characteristics that take multiple checks to resolve | Requires `Metrics()`, adjusts after each check: fast → 50% less, slow → 100% more |

### Global Configuration

PleaseWait supports global configuration to set default values for all wait operations. This is useful for setting application-wide defaults that apply to all `Wait()` instances.

```csharp
// Configure global defaults
Wait().Global().Configure()
    .Timeout(30, Seconds)
    .Polling(200, Millis, 500, Millis)  // Set both polling values
    .Logger(new ConsoleLogger())
    .Strategy(Conservative)
    .IgnoreExceptions(true)
    .FailSilently(false)
    .Prereqs(new List<Action> { () => RefreshData() });

// All subsequent Wait() calls will use these defaults
Wait().Until(() => SomeCondition()); // Uses 30s timeout, 200ms poll delay, etc.

// Individual instances can still override defaults
Wait()
    .AtMost(10, Seconds) // Override timeout
    .Strategy(Aggressive) // Override strategy
    .Until(() => SomeCondition());

// Reset to original defaults
Wait().Global().ResetToDefaults();
```

**Global Configuration Options:**

| Method | Default Value | Description |
|--------|---------------|-------------|
| `Timeout(double, TimeUnit)` | 10 seconds | Default timeout for all wait operations |
| `AtMost(double, TimeUnit)` | 10 seconds | → Alias for Timeout - sets the same timeout value |
| `PollDelay(double, TimeUnit)` | 100 milliseconds | Default initial delay before first condition check |
| `PollInterval(double, TimeUnit)` | 100 milliseconds | Default delay between condition checks |
| `Polling(double, TimeUnit, double, TimeUnit)` | 100ms delay, 100ms interval | → Combines PollDelay + PollInterval - sets both values in one call |
| `Strategy(WaitStrategy)` | `Linear` | Default wait strategy for all operations |
| `Logger(IWaitLogger)` | `NullLogger` | Default logger for all wait operations (no logging by default) |
| `Metrics(bool)` | `false` | Whether to collect metrics by default |
| `IgnoreExceptions(bool)` | `true` | Whether to ignore exceptions during condition checks |
| `FailSilently(bool)` | `false` | Whether to suppress TimeoutException and return normally on timeout |
| `ExceptionHandling(bool, bool)` | `true, false` | → Combines IgnoreExceptions + FailSilently - sets both values in one call |
| `Alias(string?)` | `null` | Default alias for wait operations (uses "condition" when null) |
| `Prereq(Action?)` | `null` | Set a single default prerequisite action (no action when null) |
| `Prereqs(List<Action>?)` | `null` | Set multiple default prerequisite actions (no actions when null) |
| `With()` | N/A | Syntactic sugar for fluent chaining |
| `And()` | N/A | Syntactic sugar for fluent chaining |

**Note:** All time-based methods also have `TimeSpan` overloads (e.g., `AtMost(TimeSpan)`, `PollDelay(TimeSpan)`, `PollInterval(TimeSpan)`, `Polling(TimeSpan, TimeSpan)`, `Timeout(TimeSpan)`).

**Best Practices:**
- Configure global defaults early in your application startup
- Use `Wait().Global().ResetToDefaults()` in test teardown to prevent test interference
- Override defaults on individual instances when specific behavior is needed
- Consider using different configurations for different environments (dev, test, prod)

### Instance Configuration

For reusable configurations that don't affect global state, use `WaitConfig` objects. This allows you to create configuration templates that can be reused across multiple wait operations.

```csharp
// Create a reusable configuration
var fastConfig = Wait().Config()
    .Timeout(5, Seconds)
    .Polling(50, Millis, 100, Millis)
    .Strategy(Aggressive)
    .Alias("Fast Operations");

// Use the configuration for multiple operations
Wait(fastConfig).Until(() => SomeCondition());
Wait(fastConfig).Until(() => AnotherCondition());

// Create another configuration for different use case
var safeConfig = Wait().Config()
    .Timeout(30, Seconds)
    .Polling(200, Millis, 500, Millis)
    .Strategy(Conservative)
    .IgnoreExceptions(true)
    .Alias("Safe Operations");

Wait(safeConfig).Until(() => CriticalOperation());
```

**Partial Configuration Overrides:**
Only set the values you want to override - unset values will use captured global defaults:

```csharp
// Set global defaults
Wait().Global().Configure()
    .Timeout(30, Seconds)
    .PollDelay(200, Millis)
    .Strategy(Conservative);

// Create config with only timeout override
var config = Wait().Config()
    .Timeout(5, Seconds);
    // PollDelay and Strategy will use captured global defaults

Wait(config).Until(() => condition); // Uses 5s timeout + captured poll delay + captured strategy

// Update global defaults later
Wait().Global().Configure().PollDelay(100, Millis);

// Same config still uses CAPTURED global defaults (not updated ones)
Wait(config).Until(() => condition); // Uses 5s timeout + 200ms poll delay + Conservative strategy
```

⚠️ **IMPORTANT**: Configs capture global defaults when created.
When you create a `WaitConfig`, it captures the current global default values at creation time. This ensures predictable behavior - the config will always use the same defaults regardless of subsequent global configuration changes.

**Instance Configuration Options:**

| Method | Description |
|--------|-------------|
| `Timeout(double, TimeUnit)` | Set timeout for this configuration |
| `AtMost(double, TimeUnit)` | → Alias for Timeout - sets the same timeout value |
| `PollDelay(double, TimeUnit)` | Set poll delay for this configuration |
| `PollInterval(double, TimeUnit)` | Set poll interval for this configuration |
| `Polling(double, TimeUnit, double, TimeUnit)` | → Combines PollDelay + PollInterval - sets both values for this configuration |
| `Strategy(WaitStrategy)` | Set wait strategy for this configuration |
| `Logger(IWaitLogger)` | Set logger for this configuration |
| `Metrics(bool)` | Enable/disable metrics collection |
| `IgnoreExceptions(bool)` | Set exception handling behavior |
| `FailSilently(bool)` | Set fail silently behavior |
| `ExceptionHandling(bool, bool)` | → Combines IgnoreExceptions + FailSilently - sets both values for this configuration |
| `Alias(string?)` | Set alias for this configuration |
| `Prereq(Action?)` | Set a single prerequisite action |
| `Prereqs(List<Action>?)` | Set multiple prerequisite actions |
| `With()` | Syntactic sugar for fluent chaining |
| `And()` | Syntactic sugar for fluent chaining |

**Note:** All time-based methods also have `TimeSpan` overloads (e.g., `AtMost(TimeSpan)`, `PollDelay(TimeSpan)`, `PollInterval(TimeSpan)`, `Polling(TimeSpan, TimeSpan)`, `Timeout(TimeSpan)`).

**Benefits:**
- **Reusable**: Create once, use many times
- **Thread Safe**: No global state modification
- **Flexible**: Override only what you need
- **Predictable**: Captures global defaults at creation time
- **Composable**: Chain multiple configurations

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

- **.NET Standard 2.0**
- **C# 8.0**

## 💝 Support

PleaseWait has been downloaded thousands of times and is used by developers worldwide. If you find it helpful, consider supporting its continued development:

### 🐙 GitHub Sponsors
[![GitHub Sponsors](https://img.shields.io/badge/GitHub%20Sponsors-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white)](https://github.com/sponsors/MartynNevers)

### PayPal
[![PayPal](https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://paypal.me/martynnevers)

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Clone the repository
2. Install dependencies: `dotnet restore`
3. Build: `dotnet build`
4. Run tests: `dotnet test`

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](https://github.com/MartynNevers/please-wait/blob/main/LICENSE) file for details.

## 📖 Documentation

- [Changelog](https://github.com/MartynNevers/please-wait/blob/main/CHANGELOG.md) - Complete version history and changes
- [NuGet Package](https://www.nuget.org/packages/PleaseWait) - Package information and downloads

## 🙏 Acknowledgments

- Inspired by the need for better asynchronous testing patterns
- Built with performance and developer experience in mind
- Thanks to all contributors and users of this library

---

**Good things come to those who wait...** ⏳

# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Removed
- **Configuration Sleep Methods**: Remove no-op Sleep() methods from GlobalConfigurationBuilder and WaitConfig classes
  - These methods were API consistency placeholders that didn't actually modify any configuration
  - The actual `Wait().Sleep()` functionality remains intact and working

### Changed
- **Documentation**: Remove Sleep() references from configuration options tables in README

## [3.0.0] - 2025-08-20

### Breaking Changes
- **TimeUnit Enum**: Update enum values from uppercase to PascalCase (MILLIS → Millis, SECONDS → Seconds, etc.)

### Added
- **Cancellation Support**: Add CancellationToken parameter to Until() methods
- **Logging System**: IWaitLogger interface with ConsoleLogger, DebugLogger, and NullLogger
- **Performance Monitoring**: Metrics() method and WaitMetrics class
- **Wait Strategies**: 5 polling strategies (Linear, ExponentialBackoff, Aggressive, Conservative, Adaptive)
- **Global Configuration**: Wait().Global().Configure() and Wait().Global().ResetToDefaults()
- **Instance Configuration**: WaitConfig class and Wait().Config() method
- **API Consistency**: Complete method name consistency across Dsl, GlobalConfigurationBuilder, and WaitConfig

### Changed
- **Documentation**: Comprehensive README.md rewrite with examples

## [2.7.0] - 2024-07-12

### Changed
- **Breaking Change**: Target .NET Standard 2.0 instead of .NET Framework 4.8, .NET 6.0, .NET 7.0, and .NET 8.0
- Stop running tests for .NET 7
- Fix SA1133 warning (XML documentation)
- Add TestFixture attribute to test classes
- Update Microsoft.NET.Test.Sdk and NUnit.Analyzers dependencies

### Technical
- Trivial update to test subject for improved test reliability

## [2.6.0] - 2024-04-08

### Changed
- **API Enhancement**: Refactor `Sleep()` method to accept parameters instead of using timeout
  - `Sleep()` now accepts `TimeSpan` or `(double, TimeUnit)` parameters
  - Method now returns `Dsl` for fluent chaining
- Update NUnit.Analyzers, NUnit, coverlet.collector, and Microsoft.NET.Test.Sdk dependencies

## [2.5.0] - 2023-12-06

### Added
- **New Methods**: `UntilTrue()` and `UntilFalse()` methods for explicit boolean condition checking
- **API Enhancement**: Modified `Until()` method to accept optional `expected` parameter (defaults to `true`)
- Comprehensive tests for new UntilTrue and UntilFalse methods
- XML documentation to all public methods for better IntelliSense support

### Changed
- **API**: Use Async suffix on Spoil method for better naming convention
- Initialize a fresh orange in tests for improved test isolation
- Update NUnit and NUnit.Analyzers dependencies

## [2.1.0 - 2.4.1] - 2023-05-23 to 2023-11-21

### Changed
- Regular dependency updates for NUnit.Analyzers, NUnit, coverlet.collector, and Microsoft.NET.Test.Sdk (testing framework dependencies)

## [2.0.0] - 2023-05-19

### Added
- **Testing**: Additional tests for 100% code coverage

### Changed
- **API**: Improved exception message for better debugging experience

## [2.0.0-alpha2] - 2023-05-19

### Changed
- **Refactoring**: Rename class for better naming convention

## [2.0.0-alpha] - 2023-05-19

### Added
- **Major Feature**: Complete architectural redesign with enhanced DSL functionality
- **New Classes**: 
  - `Defaults.cs` - Configuration constants (timeout: 10s, poll delay: 100ms, poll interval: 100ms)
  - `TimeConstraint.cs` - Time constraint handling with `GetTimeSpan()` method
  - `Dsl.cs` - New domain-specific language implementation
- **Enhanced Testing**: 
  - `Orange.cs` - Test fixture with async methods (`PeelAsync`, `SpoilAsync`) and exception handling
  - `ReflectionExtensions.cs` - Reflection utilities for testing private fields/properties
- **Documentation**: Updated README with enhanced DSL examples

### Changed
- **API Redesign**: Complete rewrite of `PleaseWait` class with new fluent API:
  - `Wait()` - Static factory method
  - `AtMost()` - Timeout configuration (supports both `TimeUnit` and `TimeSpan`)
  - `WithPollDelay()` / `WithPollInterval()` - Separate polling configuration
  - `IgnoreExceptions()` / `FailSilently()` - Exception handling control
  - `WithPrereq()` / `WithPrereqs()` - Prerequisite actions (renamed from "action" to "prereq")
  - `Until()` - Condition checking with improved logic
- **API**: Rename var from action to prereq for better clarity
- **Documentation**: Minor updates to documentation

### Breaking Changes
- This is a major version update with significant architectural changes
- New DSL syntax and improved API design
- Method signatures and class structure completely changed

## [1.0.2 - 1.0.3] - 2023-05-08 to 2023-05-11

### Changed
- Regular dependency updates for NUnit.Analyzers, NUnit, coverlet.collector, and Microsoft.NET.Test.Sdk (testing framework dependencies)

## [1.0.1] - 2023-05-06

### Added
- **Testing**: Comprehensive tests to ensure exceptions thrown by conditions/actions are properly swallowed
  - Added `WhenConditionThrowsExceptionThenExceptionIsSwallowedTest()`
  - Added `WhenActionThrowsExceptionThenExceptionIsSwallowedTest()`
  - Added `IncrementValueWithExceptions()` helper method

### Changed
- **Performance**: Simplify member access by removing unnecessary `this.` prefixes
  - Changed `PleaseWait.ConvertToTimeSpan()` to `ConvertToTimeSpan()`
  - Changed `this.UpdateValue()` to `UpdateValue()` in tests
- **Code Quality**: Methods can be made static for better design
  - Made `UpdateValue()` method static

## [1.0.0] - 2023-05-05

### Added
- **Development**: Suppress SA0001 via EditorConfig file for better development experience

### Changed
- **CI/CD**: Version bump for setup-nuget action

## [1.0.0-alpha2] - 2023-05-05

### Added
- **Multi-Platform Support**: Target frameworks net6.0, net7.0 and netcoreapp3.1
- **NuGet Package**: Complete NuGet package configuration with metadata
  - Authors, Company, Description, PackageProjectUrl, RepositoryUrl, PackageTags, PackageLicenseFile
  - PackageReadmeFile integration
  - StyleCop.Analyzers dependency
- **Documentation**: Include NuGet badge in README

### Changed
- **Code Style**: Adjust indent for consistency

## [1.0.0-alpha] - 2023-05-05

### Added
- **Initial Release**: PleaseWait library for asynchronous testing
- **Core API**: 
  - `PleaseWait` class with fluent API
  - `AtMost()` method for timeout configuration
  - `WithPollingRate()` method for custom polling intervals (default: 1 second)
  - `AndThrows()` method for exception control (default: true)
  - `Until()` method for condition checking with three overloads:
    - `Until(Func<bool> condition)`
    - `Until(Func<bool> condition, Action action)`
    - `Until(Func<bool> condition, IList<Action>? actions)`
- **Time Units**: `TimeUnit` enum with MILLIS, SECONDS, MINUTES, HOURS, DAYS values
- **Testing**: Comprehensive unit tests covering:
  - Successful condition completion
  - Timeout handling with exceptions
  - Graceful timeout handling (no exceptions)
  - Custom polling rates
  - Single and multiple action execution
- **Test Utilities**: `Ref<T>` class for reference-type testing
- **Documentation**: CI status badge in README
- **Licensing**: Apache 2.0 License
- **Code Quality**: StyleCop configuration for consistent code style

### Features
- **DSL Design**: Domain-specific language for synchronizing asynchronous operations
- **Flexible Configuration**: Configurable timeout and polling rate
- **Exception Handling**: Robust exception handling for conditions and actions (swallows exceptions by default)
- **Action Support**: Support for single actions or lists of actions
- **Thread Safety**: Thread-safe implementation with Stopwatch timing
- **Performance**: Efficient polling mechanism with configurable intervals
- **Async Testing**: Built-in support for testing asynchronous operations

---

## Quick Summary by Major Version

### Version 3.x - Configuration System Overhaul (2025)
**Key Improvements:**
- **3.0.0**: Global configuration, instance configuration, logging system, wait strategies, cancellation support, and API consistency

**Breaking Changes:**
- 3.0.0: TimeUnit enum values changed to PascalCase

### Version 2.x - Enhanced DSL (2023-2024)
**Key Improvements:**
- **2.0.0**: Complete API redesign with new architecture and enhanced DSL
- **2.5.0**: Added UntilTrue/UntilFalse methods for explicit boolean checking
- **2.6.0**: Enhanced Sleep() method with parameter support
- **2.7.0**: .NET Standard 2.0 targeting for broader compatibility

**Breaking Changes:**
- 2.0.0: Complete API redesign
- 2.7.0: .NET Standard 2.0 targeting

### Version 1.x - Initial Release (2023)
**Key Features:**
- Basic DSL for asynchronous testing
- Fluent API with timeout and polling configuration
- Comprehensive test suite
- Multi-platform support (.NET 6.0, 7.0, 3.1)

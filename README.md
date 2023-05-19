# please-wait
[![CI](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml/badge.svg)](https://github.com/MartynNevers/please-wait/actions/workflows/ci.yml) ![NuGet](https://img.shields.io/nuget/v/PleaseWait?color=blue) ![NuGet](https://img.shields.io/nuget/dt/PleaseWait)

Testing asynchronous systems is not an easy task and is obscured by details such as handling threads, timeouts and concurrency. `PleaseWait` is a lightweight DSL that allows you to express the expectations of an asynchronous system in a straightforward manner by waiting for conditions to occur. For example:

```csharp
using static PleaseWait.Dsl;
using static PleaseWait.TimeUnit;

[Test]
public void DemonstratePleaseWait()
{
	var orange = new Orange();
	_ = orange.PeelFastAsync();
	
	// PleaseWait lets you wait until the asynchronous operation completes:
	Wait().AtMost(10, SECONDS).Until(() => orange.IsPeeled);
	...
}
```

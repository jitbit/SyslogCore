# SyslogCore

Simple way to write to `syslog` aka `/dev/log` aka `/var/log/syslog` in .NET Core on Linux. Consists of [one short C# file](Syslog.cs) (70 lines!) that you can throw into your project. Tested in ASP.NET Core 5.

## Usage

```csharp
Syslog.Write(Syslog.Level.Warning, "MyAwesomeApp", "something went wrong");
```

## The problem

.NET Core (aka .NET 5) does not have a [built-in](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0&tabs=aspnetcore2x#built-in-logging-providers-1) logging provider for linux. The official recommendation is to use a 3rd party logger, like Serilog, NLog, Log4Net etc.

All heavyweight large libraries.

There's an [ongoing discussion](https://github.com/aspnet/Logging/issues/441) if a default  logger should be a part of .NET runtime, but it's stall.

## File logging is tricky

Where do you place the logs? How do you grant permissions to that location? How should the files be formatted? When do the files rotate/roll over? Microsoft couldn't decide and... simply ignored the problem.

## Enter `syslog`

**Why reinvent the wheel?!**

Almost every linux distro comes with a built-in feature called `syslog`. It takes care of everything: receives messages, writes logs, rotates files, and exists on literally every linux machine. It has lots of ways to send messages to it, UDP-listener, TCP-listener, a "Unix socket" at `/dev/log`, a `logger` CLI command or a `syslog()` system function etc.

For Windows folks: think of it as an `EventLog.Write`, *but for Linux*

## How do we use it in C#?

Just use the good old `DllImport` to reference the external `libc` library, and call the original [syslog](https://linux.die.net/man/3/syslog) function. That's it. No Nugets, no dependency-injection.

Happy coding.

# FAQ

### 1. What if (for some weird reason) syslog is not installed on my Linux?

Run `sudo apt-get install rsyslog`

### 2. Syslog is present on my machine but there's no logs

Another issue is `rsyslog` might be present but it's not running (known issue with WSL2) in that case, check that its running, if not - start it:

```
$ service rsyslog status
 * rsyslogd is not running
$ sudo service rsyslog start
```

Then check that logging actually works:

```
$ logger testtesttest
$ tail -1 /var/log/syslog
Oct 11 13:51:18 DESKTOP-CDBR5NK jazz: testtesttest
```

### 3. What if I copy paste this into a cross-platform app that runs on both Windows and Linux?

No worries, the code checks if it runs on Windows or Linux before proceeding.

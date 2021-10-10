using System;
using System.Runtime.InteropServices;

namespace Jitbit.Utils
{
	public class Syslog
	{
		[Flags]
		public enum Option
		{
			Pid = 0x01,
			Console = 0x02,
			Delay = 0x04,
			NoDelay = 0x08,
			NoWait = 0x10,
			PrintError = 0x20
		}

		[Flags]
		private enum Facility
		{
			Kernel = 0 << 3,
			User = 1 << 3,
			Mail = 2 << 3,
			Daemon = 3 << 3,
			Auth = 4 << 3,
			Syslog = 5 << 3,
			Lpr = 6 << 3,
			News = 7 << 3,
			Uucp = 8 << 3,
			Cron = 8 << 3,
			AuthPriv = 10 << 3,
			Ftp = 11 << 3,
			Local0 = 16 << 3,
			Local1 = 17 << 3,
			Local2 = 18 << 3,
			Local3 = 19 << 3,
			Local4 = 20 << 3,
			Local5 = 21 << 3,
			Local6 = 22 << 3,
			Local7 = 23 << 3,
		}

		[Flags]
		public enum Level
		{
			Emerg = 0,
			Alert = 1,
			Crit = 2,
			Err = 3,
			Warning = 4,
			Notice = 5,
			Info = 6,
			Debug = 7
		}

		[DllImport("libc")]
		private static extern void openlog(IntPtr ident, Option option, Facility facility);

		[DllImport("libc")]
		private static extern void syslog(int priority, string message);

		[DllImport("libc")]
		private static extern void closelog();

		public static void Write(Syslog.Level level, string identity, string message)
		{
			//are we on linux?
			if (!OperatingSystem.IsLinux()) return;

			IntPtr ident = Marshal.StringToHGlobalAnsi(identity);
			openlog(ident, Option.Console | Option.Pid | Option.PrintError, Facility.User);
			syslog((int)Facility.User | (int)level, message);
			closelog();
			Marshal.FreeHGlobal(ident);
		}
	}
}

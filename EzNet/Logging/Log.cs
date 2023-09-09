using EzRPC.Logging;
using System.Diagnostics;

namespace EzNet.Logging
{
	internal static class Log
	{
		public static LOG_LEVEL Level = LOG_LEVEL.Info;

		public static TextWriter? Writer;
		private static bool _isInitialized = false;
		private static readonly Stopwatch Timer = Stopwatch.StartNew();

		public static void Trace(object? msg) => WriteMessage(LOG_LEVEL.Trace, (msg ?? string.Empty).ToString());
		public static void Debug(object? msg) => WriteMessage(LOG_LEVEL.Debug, (msg ?? string.Empty).ToString());
		public static void Info(object? msg) => WriteMessage(LOG_LEVEL.Info, (msg ?? string.Empty).ToString());
		public static void Warn(object? msg) => WriteMessage(LOG_LEVEL.Warn, (msg ?? string.Empty).ToString());
		public static void Error(object? msg) => WriteMessage(LOG_LEVEL.Error, (msg ?? string.Empty).ToString());
		public static void Fatal(object? msg) => WriteMessage(LOG_LEVEL.Fatal, (msg ?? string.Empty).ToString());
		
		public static void Trace(string message) => WriteMessage(LOG_LEVEL.Trace, message);
		public static void Debug(string message) => WriteMessage(LOG_LEVEL.Debug, message);
		public static void Info(string message) => WriteMessage(LOG_LEVEL.Info, message);
		public static void Warn(string message) => WriteMessage(LOG_LEVEL.Warn, message);
		public static void Error(string message) => WriteMessage(LOG_LEVEL.Error, message);
		public static void Fatal(string message) => WriteMessage(LOG_LEVEL.Fatal, message);

		public static void Trace(string format, params object[] args) => WriteMessage(LOG_LEVEL.Trace, string.Format(format ?? string.Empty, args));
		public static void Debug(string format, params object[] args) => WriteMessage(LOG_LEVEL.Debug, string.Format(format ?? string.Empty, args));
		public static void Info(string format, params object[] args) => WriteMessage(LOG_LEVEL.Info, string.Format(format ?? string.Empty, args));
		public static void Warn(string format, params object[] args) => WriteMessage(LOG_LEVEL.Warn, string.Format(format ?? string.Empty, args));
		public static void Error(string format, params object[] args) => WriteMessage(LOG_LEVEL.Error, string.Format(format ?? string.Empty, args));
		public static void Fatal(string format, params object[] args) => WriteMessage(LOG_LEVEL.Fatal, string.Format(format ?? string.Empty, args));

		public static void SetStream(TextWriter writer)
		{
			Writer = writer;
		}

		private static void WriteMessage(LOG_LEVEL level, string msg)
		{
			if (Level == LOG_LEVEL.Off || level < Level) return;

			msg = $"[{level}-{Timer.Elapsed:hh\\:mm\\:ss}] {msg}";

			// TODO: Turn this into a queue
			Writer?.WriteLine(msg);

			switch (level)
			{
				case LOG_LEVEL.Trace:
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
				case LOG_LEVEL.Debug:
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case LOG_LEVEL.Info:
					Console.ForegroundColor = ConsoleColor.White;
					break;
				case LOG_LEVEL.Warn:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LOG_LEVEL.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LOG_LEVEL.Fatal:
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
			}

			Console.WriteLine(msg);
			Console.ResetColor();
		}
	}
}

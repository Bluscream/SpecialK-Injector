using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;

namespace SKInjector
{
    class Logger
    {
        private static bool verbose = false;
        enum LogLevel {
            Trace, Debug, Info, Warn, Error, Fatal
        }
        private static FileInfo getLogFile(string fileName = "Injector.log") {
            return new FileInfo(Path.Combine(Utils.getOwnPath().DirectoryName, fileName));
        }
        public static void Init() {
            try { Console.Title = "SK Injector - Log"; } catch { }
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            args = args.Select(s => s.ToLowerInvariant()).ToArray();
            verbose = args.ContainsAny("--verbose","-v");
            ClearLog();
        }
        public static void ClearLog() {
            var log = getLogFile();
            if (log.Exists && log.Length > 0) try { File.WriteAllText(log.FullName, string.Empty); } catch { }
            }
        private static ConsoleColor ColorFromLogLevel(LogLevel logLevel) {
            switch (logLevel) {
                case LogLevel.Trace:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.Cyan;
                case LogLevel.Warn:
                    return ConsoleColor.DarkYellow;
                case LogLevel.Error:
                    return ConsoleColor.Red;
                case LogLevel.Fatal:
                    return ConsoleColor.DarkRed;
                default:
                    return ConsoleColor.White;
            }
        }
        public static void Trace(params object[] msg) => log(LogLevel.Trace, msgs: msg);
        public static void Debug(params object[] msg) => log(LogLevel.Debug, msgs: msg);
        public static void Log(params object[] msg) => log(LogLevel.Info, msgs: msg);
        public static void LogLines(params object[] msg) => log(LogLevel.Info, lines: true, msgs: msg);
        public static void Warn(params object[] msg) => log(LogLevel.Warn, msgs: msg);
        public static void Error(params object[] msg) => log(LogLevel.Error, msgs: msg);
        public static void Fatal(params object[] msg) => log(LogLevel.Fatal, msgs: msg);
        private static void log(LogLevel logLevel, bool lines = false, params object[] msgs) // [CallerMemberName] string cName = "Unknown.Unknown", 
        {
            string timestamp = DateTime.UtcNow.ToString("HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            StackFrame frame = new StackFrame(2); var method = frame.GetMethod(); var cName = method.DeclaringType.Name; var mName = method.Name;
            var oldColor = Console.ForegroundColor;
            var newColor = ColorFromLogLevel(logLevel);
            var str = "";
            var seperator = lines ? Environment.NewLine : " ";
            foreach(var msg in msgs) {
                try { str += seperator + (string)msg;
                } catch (Exception) {
                    try {
                        str += seperator + msg.ToJson();
                    } catch (Exception) {
                        try {
                            str += seperator + msg.ToString();
                        } catch (Exception) { }
                    }
                }
            }
            var line = $"[{timestamp}] {logLevel} - {cName}.{mName}: {str}";
            getLogFile().AppendLine(line);
            if (logLevel > LogLevel.Trace || verbose ) {
                try {
                    Console.ForegroundColor = newColor;
                    Console.WriteLine(line);
                    Console.ForegroundColor = oldColor;
                } catch { }
            }
        }
    }
}

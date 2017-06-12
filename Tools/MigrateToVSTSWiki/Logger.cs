using System;

namespace MigrateToVSTSWiki
{
    public enum LogType
    {
        Info,
        Error
    }

    public static class Logger
    {
        public static void Log(string message, LogType type = LogType.Info)
        {
            Console.WriteLine(type.ToString() + ":" + message);
        }
    }
}

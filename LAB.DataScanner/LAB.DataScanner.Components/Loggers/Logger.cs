namespace LAB.DataScanner.Components.Loggers
{
    using System;
    using Serilog;

    public class Logger : IInfoLogger
    {
        public void Error(Exception ex)
        {
            Log.Error(ex.Message);
        }

        public void Info(string message)
        {
            Log.Information(message);
        }

        public void InitLogger(string logFileName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}

namespace LAB.DataScanner.Components.Loggers
{
    using System;

    public interface ILoggerServiceSerilog : Serilog.ILogger
    {
        T RunWithExceptionLogging<T>(Func<T> functionToRun);

        void RunWithExceptionLogging(Action actionToRun);

        T RunWithSilentExceptionLogging<T>(Func<T> functionToRun);

        void RunWithSilentExceptionLogging(Action actionToRun);
    }
}

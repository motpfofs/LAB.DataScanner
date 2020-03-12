namespace LAB.DataScanner.Components.Loggers
{
    using System;

    public interface IInfoLogger : IDisposable
    {
        void Error(Exception ex);
        void InitLogger(string logFileName);
        void Info(string message);
    }
}

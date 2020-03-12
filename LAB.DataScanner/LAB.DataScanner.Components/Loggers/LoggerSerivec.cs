namespace LAB.DataScanner.Components.Loggers
{
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using System;
    using System.Collections.Generic;

    public class LoggerService : ILoggerServiceSerilog
    {
        private readonly string ApplicationName;
        private readonly ILogger logger;

        public LoggerService(ILogger logger, string applicationName)
        {
            this.logger = logger;
            this.ApplicationName = applicationName;
        }

        public ILogger ForContext(ILogEventEnricher enricher)
        {
            return this.logger.ForContext(enricher);
        }

        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            return this.logger.ForContext(enrichers);
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            return this.logger.ForContext(propertyName, value, destructureObjects);
        }

        public ILogger ForContext<TSource>()
        {
            return this.logger.ForContext<TSource>();
        }

        public ILogger ForContext(Type source)
        {
            return this.logger.ForContext(source);
        }

        public void Write(LogEvent logEvent)
        {
            this.logger.Write(logEvent);
        }

        public void Write(LogEventLevel level, string messageTemplate)
        {
            this.logger.Write(level, messageTemplate);
        }

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            this.logger.Write(level, messageTemplate, propertyValue);
        }

        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            this.logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Write<T0, T1, T2>(
            LogEventLevel level,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            this.logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            this.logger.Write(level, messageTemplate, propertyValues);
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            this.logger.Write(level, exception, messageTemplate);
        }

        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            this.logger.Write(level, exception, messageTemplate, propertyValue);
        }

        public void Write<T0, T1>(
            LogEventLevel level,
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1)
        {
            this.logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Write<T0, T1, T2>(
            LogEventLevel level,
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            this.logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            this.logger.Write(level, exception, messageTemplate, propertyValues);
        }

        public bool IsEnabled(LogEventLevel level)
        {
            return this.logger.IsEnabled(level);
        }

        public void Verbose(string messageTemplate)
        {
            this.logger.Verbose(messageTemplate);
        }

        public void Verbose<T>(string messageTemplate, T propertyValue)
        {
            this.logger.Verbose(messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            this.logger.Verbose(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            this.logger.Verbose(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            this.logger.Verbose(messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            this.logger.Verbose(exception, messageTemplate);
        }

        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            this.logger.Verbose(exception, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            this.logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            this.logger.Verbose(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            this.logger.Verbose(exception, messageTemplate, propertyValues);
        }

        public void Debug(string messageTemplate)
        {
            this.logger.Debug(messageTemplate);
        }

        public void Debug<T>(string messageTemplate, T propertyValue)
        {
            this.logger.Debug(messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            this.logger.Debug(messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            this.logger.Debug(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            this.logger.Debug(messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            this.logger.Debug(exception, messageTemplate);
        }

        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            this.logger.Debug(exception, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            this.logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            this.logger.Debug(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            this.logger.Debug(exception, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(message, ApplicationName);
        }

        public void Information<T>(string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(message, ApplicationName, propertyValue);
        }

        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(message, ApplicationName, messageTemplate, propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(exception, message, ApplicationName);
        }

        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(exception, message, ApplicationName, propertyValue);
        }

        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(exception, message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(exception, message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Information(exception, message, ApplicationName, propertyValues);
        }

        public void Warning(string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(message, ApplicationName);
        }

        public void Warning<T>(string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(message, ApplicationName, propertyValue);
        }

        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(message, ApplicationName, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(exception, message, ApplicationName);
        }

        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(exception, message, ApplicationName, propertyValue);
        }

        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(exception, message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(exception, message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Warning(exception, message, ApplicationName, propertyValues);
        }

        public void Error(string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(message, ApplicationName);
        }

        public void Error<T>(string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(message, ApplicationName, propertyValue);
        }

        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(message, ApplicationName, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(exception, message, ApplicationName);
        }

        public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(exception, message, ApplicationName, propertyValue);
        }

        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(exception, message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(exception, message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Error(exception, message, ApplicationName, propertyValues);
        }

        public void Fatal(string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(message, ApplicationName);
        }

        public void Fatal<T>(string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(message, ApplicationName, propertyValue);
        }

        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(message, ApplicationName, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(exception, message, ApplicationName);
        }

        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(exception, message, ApplicationName, propertyValue);
        }

        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(exception, message, ApplicationName, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(
            Exception exception,
            string messageTemplate,
            T0 propertyValue0,
            T1 propertyValue1,
            T2 propertyValue2)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(exception, message, ApplicationName, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            var message = "Service {ApplicationName}. " + messageTemplate;
            this.logger.Fatal(exception, message, ApplicationName, propertyValues);
        }

        public bool BindMessageTemplate(
            string messageTemplate,
            object[] propertyValues,
            out MessageTemplate parsedTemplate,
            out IEnumerable<LogEventProperty> boundProperties)
        {
            return this.logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
        }

        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            return this.logger.BindProperty(propertyName, value, destructureObjects, out property);
        }

        public T RunWithExceptionLogging<T>(Func<T> functionToRun)
        {
            try
            {
                return functionToRun();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Error occurred");
                throw;
            }
        }

        public void RunWithExceptionLogging(Action actionToRun)
        {
            this.RunWithExceptionLogging<object>(
                () =>
                {
                    actionToRun();
                    return null;
                });
        }

        public T RunWithSilentExceptionLogging<T>(Func<T> functionToRun)
        {
            try
            {
                return functionToRun();
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Error occurred");
                return default(T);
            }
        }

        public void RunWithSilentExceptionLogging(Action actionToRun)
        {
            this.RunWithSilentExceptionLogging<object>(
                () =>
                {
                    actionToRun();
                    return null;
                });
        }
    }
}

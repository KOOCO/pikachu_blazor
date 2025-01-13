using Kooco.Pikachu.Interfaces;

using Serilog.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Services
{
    public class SerilogService<T>(ILogger logger) : ILoggerService<T>
    {
        private readonly string _categoryName = typeof(T).Name;

        private object?[] PrependCategoryAndWorkId(object?[]? propertyValues)
        {
            var newPropertyValues = new object?[propertyValues?.Length + 1 ?? 1];
            newPropertyValues[0] = _categoryName;

            propertyValues?.CopyTo(newPropertyValues, 1);
            return newPropertyValues;
        }

        private void Log(LogEventLevel level, Exception? exception, string message,
            object?[]? propertyValues)
        {
            var newPropertyValues = PrependCategoryAndWorkId(propertyValues);
            logger.Write(level, exception, "{CategoryName} | " + message, newPropertyValues);
        }

        public void Error(Exception? exception, string message, params object?[]? propertyValues)
        {
            Log(LogEventLevel.Error, exception, message, propertyValues);
        }

        /*public void Error(Exception? exception, string message, params object?[]? propertyValues)
        {
            Error(exception, string.Empty, message, propertyValues);
        }*/

        public void Info(string message, params object?[]? propertyValues)
        {
            Log(LogEventLevel.Information, null, message, propertyValues);
        }

        public void Warn(string message, params object?[]? propertyValues)
        {
            Log(LogEventLevel.Warning, null, message, propertyValues);
        }

        public void Debug(string message, params object?[]? propertyValues)
        {
            Log(LogEventLevel.Debug, null, message, propertyValues);
        }
    }
}

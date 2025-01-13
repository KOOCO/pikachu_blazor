using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Interfaces
{
    public interface ILoggerService<T>
    {
        //void Error(Exception? exception, string message, params object?[]? propertyValues);
        //void Error(Exception? exception, string workId, string message, params object?[]? propertyValues);

        void Error(Exception? exception, string message, params object?[]? propertyValues);

        void Info(string message, params object?[]? propertyValues);

        // void Info(string tenant, Guid workId, string message, params object?[]? propertyValues);
        void Debug(string message, params object?[]? propertyValues);

        void Warn(string message, params object?[]? propertyValues);
    }
}

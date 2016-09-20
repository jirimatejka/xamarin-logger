using System;
using System.Collections.Generic;

namespace Plugin.SimpleLogger.Abstractions
{
    /// <summary>
    /// Interface for SimpleLogger
    /// </summary>
    public interface ISimpleLogger
    {
        /// <summary>
        /// Logs debug message to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(string message);

        /// <summary>
        /// Logs information message to current log file
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(string message);

        /// <summary>
        /// Logs error message including the exception and stack trace to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        void Error(string message, Exception ex);

        /// <summary>
        /// Logs a warning message to current log file
        /// </summary>
        /// <param name="message">The message.</param>
        void Warning(string message);

        /// <summary>
        /// Gets the content of the current log file
        /// </summary>
        /// <returns>System.String.</returns>
        string GetCurrentLogContent();

        /// <summary>
        /// Gets the content of all log files as string
        /// </summary>
        /// <returns>System.String.</returns>
        string GetAllLogContent();

        /// <summary>
        /// Gets the content of specific log file
        /// </summary>
        /// <param name="logFileName">Name of the log file including extension but without path. Use an item from GetLogFiles method</param>
        /// <returns>System.String.</returns>
        string GetLogContent(string logFileName);

        /// <summary>
        /// Returns list of all registered log files
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        List<string> GetLogFiles();

        /// <summary>
        /// Deletes all log files. 
        /// </summary>
        void PurgeLog();

        /// <summary>
        /// Configures the logger
        /// </summary>
        /// <param name="logFileNameBase">The log file name base.</param>
        /// <param name="maxLogFilesCount">The maximum log files count.</param>
        /// <param name="maxLogFileSizeKb">The maximum log file size kb.</param>
        /// <param name="level">The logging level</param>
        void Configure(string logFileNameBase, int maxLogFilesCount = 3, int maxLogFileSizeKb = 100, LogLevel level = LogLevel.Warning);
    }
}

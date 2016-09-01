using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SimpleLogger.Abstractions
{
    public abstract class SimpleLoggerBase : ISimpleLogger
    {
        private const string DEFAULT_LOG_NAME = "app_events";

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLoggerBase"/> class.
        /// </summary>
        public SimpleLoggerBase()
        {
            logFiles = new List<string>();
            Configure(DEFAULT_LOG_NAME);
        }
        #endregion

        #region Properties

        private readonly object objectLock = new object();

        private string logFileNameBase;
        private int maxLogFilesCount;
        private int maxLogFileSizeKb;

        private LogLevel logLevel;
        /// <summary>
        /// Gets or sets the log level. See LogLevel enumeration for details.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel
        {
            get { return logLevel; }
            set { logLevel = value; }
        }
        private string currentLogFileName
        {
            get { return logFiles.First(); }
        }
        private List<string> logFiles;
        private string logStatusFileName
        {
            get { return String.Format("{0}_status.txt", logFileNameBase); }
        }

        #endregion

        #region Abstract Methods - implemented in platform specific projects

        protected abstract bool AppendToFile(string filename, string message);

        protected abstract bool SaveToFile(string filename, string message);

        protected abstract string LoadFromFile(string filename);

        protected abstract bool DeleteFile(string filename);

        protected abstract string GetNextAvailableFileName(string logFileNameBase);

        protected abstract long GetFileSizeKb(string fileName);

        #endregion


        #region Private Methods

        /// <summary>
        /// Loads the log file config from a file or creates a new set from scratch
        /// </summary>
        private void SetupLogFiles()
        {
            // load status if any            
            if (!loadLogStatus())
            {
                // first logging on this device or logs have been purged
                var newFileName = getNextAvailableFileName();
                logFiles.Add(newFileName);
                saveLogStatus();
            }
        }

        /// <summary>
        /// Saves a message, possibly exception message and stack trace to current log files
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exc">The exc.</param>
        private void LogRecord(string message, Exception exc = null)
        {
            string rec = "";
            if (exc == null)
                rec = String.Format("{0} - {1}", DateTime.UtcNow, message);
            else
                rec = String.Format("{0} - {1}. EXCEPTION: {2}. STACK TRACE: {3}.", DateTime.UtcNow, message, exc.Message, exc.StackTrace.ToString());
            AppendToFile(currentLogFileName, rec);

            // check log rollover
            checkLogRollover();
            saveLogStatus();
        }

        /// <summary>
        /// Finds first available filename based on logFileNameBase_N.log template where N is an integer from 1 to infinity. Name of first non-existing file is returned.
        /// </summary>        
        /// <returns>System.String.</returns>
        private string getNextAvailableFileName()
        {
            return GetNextAvailableFileName(logFileNameBase);
        }

        /// <summary>
        /// Checks if size of current log file exceeds the limit and creates a new one if needed. Also removes the oldest log file if total number of log files exceed the limit. 
        /// </summary>
        private void checkLogRollover()
        {
            var currentFileSize = GetFileSizeKb(currentLogFileName);
            if (currentFileSize > maxLogFileSizeKb)
            {
                if (logFiles.Count == maxLogFilesCount)
                {
                    // delete oldest file
                    if (DeleteFile(logFiles[maxLogFilesCount - 1]))
                        logFiles.RemoveAt(maxLogFilesCount - 1);
                }

                // get new filename
                logFiles.Insert(0, getNextAvailableFileName());
            }
        }

        /// <summary>
        /// Saves the list of log files ordered by age (newest first)
        /// </summary>
        /// <returns><c>true</c> if successfully saved, <c>false</c> otherwise.</returns>
        private bool saveLogStatus()
        {
            string s = "";
            foreach (var f in logFiles)
            {
                if (!String.IsNullOrEmpty(s))
                    s += ";";
                s += f;
            }
            return SaveToFile(logStatusFileName, s);
        }

        /// <summary>
        /// Loads list of log files.
        /// </summary>
        /// <returns><c>true</c> if successfully loaded, <c>false</c> otherwise.</returns>
        private bool loadLogStatus()
        {
            var status = LoadFromFile(logStatusFileName);
            if (!String.IsNullOrEmpty(status))
            {
                string[] tokens = status.Split(';');
                foreach (var token in tokens)
                {
                    logFiles.Add(token);
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Logs debug message to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            if (logLevel > LogLevel.Debug)
                return;

            lock (objectLock)
            {
                LogRecord(message);
            }
        }

        /// <summary>
        /// Logs information message to current log files
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            if (logLevel > LogLevel.Info)
                return;

            lock (objectLock)
            {
                LogRecord(message);
            }
        }

        /// <summary>
        /// Logs error message including the exception and stack trace to current log file.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception to be logged</param>
        public void Error(string message, Exception ex)
        {
            if (logLevel > LogLevel.Error)
                return;

            lock (objectLock)
            {
                LogRecord(message, ex);
            }
        }

        /// <summary>
        /// Logs a warning message to current log file
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warning(string message)
        {
            if (logLevel > LogLevel.Warning)
                return;

            lock (objectLock)
            {
                LogRecord(message);
            }
        }

        /// <summary>
        /// Gets the content of the current log file
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetCurrentLogContent()
        {
            return LoadFromFile(currentLogFileName);
        }

        /// <summary>
        /// Gets the content of all log files as string
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetAllLogContent()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var f in logFiles)
            {
                sb.AppendLine(LoadFromFile(f));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the content of specific log file
        /// </summary>
        /// <param name="logFileName">Name of the log file including extension but without path. Use an item from GetLogFiles method</param>
        /// <returns>System.String.</returns>
        public string GetLogContent(string logFileName)
        {
            return LoadFromFile(logFileName);
        }

        /// <summary>
        /// Returns list of all registered log files
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> GetLogFiles()
        {
            return logFiles;
        }

        /// <summary>
        /// Deletes all log files.
        /// </summary>
        public void PurgeLog()
        {
            foreach (var f in logFiles)
            {
               DeleteFile(f);
            }
            DeleteFile(logStatusFileName);

        }

        /// <summary>
        /// Configures the Logger
        /// </summary>        
        /// <param name="logFileNameBase">The log file name without extension. SimpleLogger will add a suffix to ensure uniqueness of the filename.</param>
        /// <param name="maxLogFilesCount">The maximum number of log files. Oldest files will be automatically removed once the number is reached.</param>
        /// <param name="maxLogFileSizeKb">The maximum size of log file size. A new log file will be created once the size is reached.</param>
        /// <param name="level">The logging level - all, debug, info, warning, error</param>
        /// <exception cref="System.ArgumentNullException">
        /// subFolderName
        /// or
        /// logFileNameBase
        /// </exception>
        public void Configure(string logFileNameBase, int maxLogFilesCount = 3, int maxLogFileSizeKb = 100, LogLevel level = LogLevel.Warning)
        {
            if (maxLogFilesCount < 1)
                throw new Exception("maxLogFilesCount must be greater than zero.");

            if (maxLogFileSizeKb < 1)
                throw new Exception("maxLogFileSizeKb must be greater than zero.");

            if (String.IsNullOrEmpty(logFileNameBase))
            {
                throw new ArgumentNullException("logFileNameBase");
            }
            else if (logFileNameBase.Contains(";"))
                throw new Exception("Semicolon is not allowed in logfile name.");

            this.logFileNameBase = logFileNameBase;
            this.maxLogFilesCount = maxLogFilesCount;
            this.maxLogFileSizeKb = maxLogFileSizeKb;
            this.logLevel = level;

            SetupLogFiles();            
        }


        #endregion
    }

}

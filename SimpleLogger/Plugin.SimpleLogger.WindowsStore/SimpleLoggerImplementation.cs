using Plugin.SimpleLogger.Abstractions;
using System;
using System.Collections.Generic;

namespace Plugin.SimpleLogger
{
    /// <summary>
    /// Implementation for SimpleLogger
    /// </summary>
    public class SimpleLoggerImplementation : SimpleLoggerBase
    {
        protected override bool AppendToFile(string filename, string message)
        {
            throw new NotImplementedException();
        }

        protected override bool DeleteFile(string filename)
        {
            throw new NotImplementedException();
        }

        protected override long GetFileSizeKb(string fileName)
        {
            throw new NotImplementedException();
        }

        protected override string GetNextAvailableFileName(string logFileNameBase)
        {
            throw new NotImplementedException();
        }

        protected override string LoadFromFile(string filename)
        {
            throw new NotImplementedException();
        }

        protected override bool SaveToFile(string filename, string message)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SimpleLogger.Abstractions
{
    public enum LogLevel
    {
        All = 0, // logs all records
        Debug = 1, // logs all records
        Info = 2, // logs info, warning and error records
        Warning = 3, // logs warning and error records
        Error = 4 // logs only error records
    }
}

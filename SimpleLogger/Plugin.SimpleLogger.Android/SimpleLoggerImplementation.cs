using Plugin.SimpleLogger.Abstractions;
using System;
using System.IO;

namespace Plugin.SimpleLogger
{
  /// <summary>
  /// Implementation for Feature
  /// </summary>
  public class SimpleLoggerImplementation : SimpleLoggerBase
  {

        private string accomplishPath(string fileOrSubfolderName)
        {
            var localFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            return Path.Combine(localFolder, fileOrSubfolderName);
        }

        protected override bool AppendToFile(string filename, string message)
        {
            try
            {
                var fullPath = accomplishPath(filename);
                File.AppendAllText(fullPath, message);
                return true;
            }
            catch (System.IO.IOException ex)
            {
                // logging to file failed, we can't do anything better than return false 
                return false;
            }
        }

        protected override bool DeleteFile(string filename)
        {
            var fullPath = accomplishPath(filename);
            try
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                return true;
            }
            catch (IOException ex)
            {
                return false;
            }
        }

        protected override long GetFileSizeKb(string fileName)
        {
            var fullPath = accomplishPath(fileName);
            try
            {
                if (File.Exists(fullPath))
                {
                    var fi = new FileInfo(fullPath);
                    return Convert.ToInt64(Math.Round((double)fi.Length / (double)1024));
                }
                return -1;
            }
            catch (IOException ex)
            {
                return -1;
            }
        }

        protected override string GetNextAvailableFileName(string logFileNameBase)
        {
            string theFileName = "";
            int i = 1;
            while (String.IsNullOrEmpty(theFileName))
            {
                var fn = String.Format("{0}{1}.log", logFileNameBase, i);
                if (File.Exists(accomplishPath(fn)))
                    i++;
                else
                    theFileName = fn;
            }
            return theFileName;
        }

        protected override string LoadFromFile(string filename)
        {
            try
            {
                var fullPath = accomplishPath(filename);
                if (File.Exists(fullPath))
                    return File.ReadAllText(fullPath);
                return "";
            }
            catch (Exception ex)
            {
                // file read failed, return empty string
                return "";
            }
        }

        protected override bool SaveToFile(string filename, string message)
        {
            try
            {
                var fullPath = accomplishPath(filename);
                File.WriteAllText(fullPath, message);
                return true;
            }
            catch (System.IO.IOException ex)
            {
                // logging to file failed, we can't do anything better than return false 
                return false;
            }
        }



    }
}
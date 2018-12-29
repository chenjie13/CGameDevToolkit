using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace CGameDevToolkit.Framework
{
    
    /// <summary>
    /// 文本日志输出
    /// </summary>
    public class FileLogOutput : LogOutput
    {
#if UNITY_EDITOR
        string _devicePersistentPath = Application.dataPath + "/../PersistentPath";
#elif UNITY_STANDALONE_WIN
		string _devicePersistentPath = Application.dataPath + "/PersistentPath";
#elif UNITY_STANDALONE_OSX
		string _devicePersistentPath = Application.dataPath + "/PersistentPath";
#else
		string _devicePersistentPath = Application.persistentDataPath;
#endif

        static string LogPath = "Log";

        private StreamWriter _logWriter;

        public FileLogOutput()
        {
            var now = DateTime.Now;
            var logName = now.ToString("yy-MM-dd_HH-mm-ss");
            var logPath = string.Format("{0}/{1}/{2}.txt", _devicePersistentPath, LogPath, logName);
            if (File.Exists(logPath))
                File.Delete(logPath);
            var logDir = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);
            _logWriter = new StreamWriter(logPath);
            _logWriter.AutoFlush = true;
            
            Start();
        }

        public override void Close()
        {
            base.Close();
            _logWriter.Close();
        }

        protected override void LogImp(LogData logData)
        {
            if (logData.Level == LogLevel.Error)
            {
                _logWriter.WriteLine(
                    "---------------------------------------------------------------------------------------------------------------------");
                _logWriter.WriteLine(DateTime.Now + "\t" + logData.Log + "\n");
                _logWriter.WriteLine(logData.Track);
                _logWriter.WriteLine(
                    "---------------------------------------------------------------------------------------------------------------------");
            }
            else
            {
                _logWriter.WriteLine(DateTime.Now + "\t" + logData.Log);
            }
        }
    }
}

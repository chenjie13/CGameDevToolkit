using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CGameDevToolkit.Framework
{
    /// <summary>
    /// 日志等级，为不同输出配置用
    /// </summary>
    public enum LogLevel
    {
        Min = 0,
        Log = 1,
        Warning = 2,
        Assert = 3,
        Error = 4,
        Max = Error,
    }

    /// <summary>
    /// 日志数据类
    /// </summary>
    public struct LogData
    {
        public string Log;
        public string Track;
        public LogLevel Level;
    }

    /// <summary>
    /// 封装日志模块
    /// </summary>
    public static class CDebug
    {
        /// <summary>
        /// 屏幕输出日志等级，只要大于等于这个级别的日志，都会输出到屏幕
        /// </summary>
        public static LogLevel ScreenLogLevel = LogLevel.Log;

        /// <summary>
        /// 文本输出日志等级，只要大于等于这个级别的日志，都会输出到文本
        /// </summary>
        public static LogLevel FileLogLevel = LogLevel.Error;

        // unity日志和日志输出等级的映射
        private static readonly Dictionary<LogType, LogLevel> _logTypeLevelDict = new Dictionary<LogType, LogLevel>
        {
            {LogType.Log, LogLevel.Log},
            {LogType.Warning, LogLevel.Warning},
            {LogType.Assert, LogLevel.Assert},
            {LogType.Error, LogLevel.Error},
            {LogType.Exception, LogLevel.Error}
        };

        private static ILogOutput _fileLogOutput = new FileLogOutput();
        private static ILogOutput _screenLogOutput = new ScreenLogOutput();

        #region Log Method

        /// <summary>
        /// Unity的Debug.Assert()在发布版本有问题
        /// </summary>
        [StringFormatMethod("info")]
        public static void Assert(bool condition, string info, params object[] args)
        {
            if (condition) return;
            Debug.LogErrorFormat(info, args);
        }

        [StringFormatMethod("info")]
        public static void Log(string info, params object[] args)
        {
            Debug.LogFormat(info, args);
        }

        [StringFormatMethod("info")]
        public static void LogToFile(string info, params object[] args)
        {
            _fileLogOutput.Log(new LogData {Log = string.Format(info, args), Level = LogLevel.Log});
        }

        [StringFormatMethod("info")]
        public static void LogToScreen(string info, params object[] args)
        {
            _screenLogOutput.Log(new LogData {Log = string.Format(info, args), Level = LogLevel.Log});
        }

        [StringFormatMethod("info")]
        public static void Warning(string info, params object[] args)
        {
            Debug.LogWarningFormat(info, args);
        }

        [StringFormatMethod("info")]
        public static void Error(string info, params object[] args)
        {
            Debug.LogErrorFormat(info, args);
        }

        static CDebug()
        {
            Application.logMessageReceivedThreaded += Output;
        }

        #endregion

        public static void Dispose()
        {
            _fileLogOutput.Close();
            _screenLogOutput.Close();
            Application.logMessageReceivedThreaded -= Output;
        }

        // 日志调用回调，主线程和其他线程都会回调这个函数，在其中根据配置输出日志
        static void Output(string log, string track, LogType type)
        {
            LogLevel level = _logTypeLevelDict[type];
            LogData logData = new LogData
            {
                Log = log,
                Track = track,
                Level = level
            };

            if (FileLogLevel >= _logTypeLevelDict[type])
                _fileLogOutput.Log(logData);
            if (ScreenLogLevel >= _logTypeLevelDict[type])
                _screenLogOutput.Log(logData);
        }
    }
}
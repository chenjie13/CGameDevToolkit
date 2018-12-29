using System.Collections.Generic;
using System.Threading;

namespace CGameDevToolkit.Framework
{
    public abstract class LogOutput : ILogOutput
    {
        protected Queue<LogData> _writingLogQueue = new Queue<LogData>();
        protected Queue<LogData> _waitingLogQueue = new Queue<LogData>();
        protected readonly object _logLock = new object();
        protected Thread _logThread;
        protected bool _isRunning;

        // 子类需要调用Start()来启动线程
        protected void Start()
        {
            _isRunning = true;
            _logThread = new Thread(WriteLog);
            _logThread.Start();
        }

        protected virtual void WriteLog()
        {
            while (_isRunning)
            {
                if (_writingLogQueue.Count == 0)
                {
                    lock (_logLock)
                    {
                        while (_waitingLogQueue.Count == 0)
                            Monitor.Wait(_logLock);
                        Queue<LogData> tmpQueue = _writingLogQueue;
                        _writingLogQueue = _waitingLogQueue;
                        _waitingLogQueue = tmpQueue;
                    }
                }
                else
                {
                    while (_writingLogQueue.Count > 0)
                    {
                        LogData log = _writingLogQueue.Dequeue();
                        LogImp(log);
                    }
                }
            }
        }

        public virtual void Log(LogData logData)
        {
            lock (_logLock)
            {
                _waitingLogQueue.Enqueue(logData);
                Monitor.Pulse(_logLock);
            }
        }

        public virtual void Close()
        {
            _isRunning = false;
        }

        // 在LogImp里使用Log会无限循环
        protected abstract void LogImp(LogData logData);
    }
    
    /// <summary>
    /// 日志输出接口
    /// </summary>
    public interface ILogOutput
    {
        /// <summary>
        /// 输出日志数据
        /// </summary>
        /// <param name="logData">日志数据</param>
        void Log(LogData logData);

        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}



using UnityEngine;
using System.Collections.Generic;

namespace CGameDevToolkit.Framework
{
    /// <summary>
    /// 控制台GUI日志输出类
    /// 在电脑上使用 Ctrl + ` 显示/隐藏窗口
    /// 移动平台上 四指点击
    /// </summary>
    public class ScreenLogOutput : LogOutput
    {
        public bool ShowGUI { get; set; }

        List<LogData> _logDatas = new List<LogData>();
        Vector2 _scrollPos;
        bool _toBottom = true;
        bool _collapse;

        const int MARGIN = 20;

        Rect _windowRect = new Rect(MARGIN + Screen.width * 0.5f, MARGIN, Screen.width * 0.5f - (2 * MARGIN),
            Screen.height - (2 * MARGIN));

        static readonly GUIContent _clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent _collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        static readonly GUIContent _scrollToBottomLabel = new GUIContent("ToBottom", "Scroll bar always at bottom");

        public ScreenLogOutput()
        {
            ShowGUI = true;
            ScreenLogger.Instance.Output = this;
            Start();
        }

        public void UpdateGUI()
        {
            if (!ShowGUI)
                return;

            _windowRect = GUILayout.Window(123456, _windowRect, DrawWindow, "Console");
        }


        /// <summary>
        /// A window displaying the logged messages.
        /// </summary>
        void DrawWindow(int windowId)
        {
            if (_toBottom)
            {
                _scrollPos = Vector2.up * _logDatas.Count * 100.0f;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            // Go through each logged entry
            for (int i = 0; i < _logDatas.Count; i++)
            {
                LogData logData = _logDatas[i];
                // If this message is the same as the last one and the collapse feature is chosen, skip it
                if (_collapse && i > 0 && logData.Log == _logDatas[i - 1].Log)
                {
                    continue;
                }

                // Change the text colour according to the log type
                switch (logData.Level)
                {
                    case LogLevel.Assert:
                    case LogLevel.Error:
                        GUI.contentColor = Color.red;
                        break;
                    case LogLevel.Warning:
                        GUI.contentColor = Color.yellow;
                        break;
                    default:
                        GUI.contentColor = Color.white;
                        break;
                }

                if (logData.Level == LogLevel.Error)
                {
                    GUILayout.Label(logData.Log + " || " + logData.Track);
                }
                else
                {
                    GUILayout.Label(logData.Log);
                }
            }

            GUI.contentColor = Color.white;
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            // Clear button
            if (GUILayout.Button(_clearLabel))
            {
                _logDatas.Clear();
            }

            // Collapse toggle
            _collapse = GUILayout.Toggle(_collapse, _collapseLabel, GUILayout.ExpandWidth(false));
            _toBottom = GUILayout.Toggle(_toBottom, _scrollToBottomLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            // Set the window to be draggable by the top title bar
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        protected override void LogImp(LogData logData)
        {
            _logDatas.Add(logData);
        }
    }

    [MonoSingletonPath("[Debug]/ScreenLogger")]
    class ScreenLogger : MonoSingleton<ScreenLogger>
    {
        public ScreenLogOutput Output;

        private void Update()
        {
            if (Output == null) return;

#if UNITY_EDITOR && UNITY_STANDALONE
            if (
                Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.BackQuote))
#elif UNITY_ANDROID && UNITY_IOS
            if (Input.touchCount >= 4 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
            {
                Output.ShowGUI = !Output.ShowGUI;
            }
        }

        private void OnGUI()
        {
            if (Output != null)
            {
                Output.UpdateGUI();
            }
        }
    }
}
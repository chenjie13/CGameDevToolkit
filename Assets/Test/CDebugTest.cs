using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CGameDevToolkit.Framework;
using UnityEngine;

public class CDebugTest : MonoBehaviour
{
	private Thread thread;
	void Start ()
	{
		CDebug.FileLogLevel = LogLevel.Max;
		CDebug.ScreenLogLevel = LogLevel.Max;
		
		Debug.Log("Unity Log");
		Debug.LogWarning("Unity Warning");
		Debug.LogError("Unity error");
		
		TestAllMethod("NormalTest");


		thread = new Thread(ThreadLogTest);
		thread.Start();
	}

	void ThreadLogTest()
	{
		var strBuilder = new StringBuilder();
		strBuilder.Append("long log Test");
		for (int i = 0; i < 10; i++)
		{
			TestAllMethod(i + " large log count test ");
			strBuilder.Append("0000000000000000000000000000000000000000000000000");
		}
		
		CDebug.Log("long log test " + strBuilder.ToString());
	}

	private static void TestAllMethod(string str)
	{
		CDebug.Log("{0} all level log", str);
		CDebug.Warning("{0} all level warning", str);
		CDebug.Error("{0} all level error", str);
		CDebug.Assert(false, "{0} all level assert false", str);
		CDebug.Assert(true, "{0} all level assert true", str);

		CDebug.LogToFile("{0} file only log", str);
		CDebug.LogToScreen("{0} screen only log", str);
	}
}

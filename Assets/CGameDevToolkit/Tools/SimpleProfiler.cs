using UnityEngine;
using UnityEngine.Profiling;

namespace CGameDevToolkit.Tools
{
	/// <summary>
	///	显示FPS和内存信息
	/// </summary>
	public class SimpleProfiler : MonoBehaviour
	{
		[Header("FPS")]
		public bool ShowFps = true;
		// 帧率计算频率
		public float CountRate = 0.5f;

		[Header("Memory")] 
		public bool ShowMemory = true;
		
		public int Fps { get; private set; }
		
		private int _frameCount;
		private float _duration;
		private float _byteToM = 1f / 1024 / 1024;

		void Update()
		{
			if (!ShowFps) return;
			
			++_frameCount;
			_duration += Time.deltaTime;
			if (_duration > CountRate)
			{
				// 计算帧率
				Fps = (int)(_frameCount / _duration);
				_frameCount = 0;
				_duration = 0f;
			}
		}

		private void OnGUI()
		{
			if (ShowFps) GUILayout.Label("fps:" + Fps);

			if (ShowMemory)
			{
				GUILayout.Label(
					string.Format("Alloc Memory : {0}M", Profiler.GetTotalReservedMemoryLong() * _byteToM));
				GUILayout.Label(
					string.Format("Reserved Memory : {0}M", Profiler.GetTotalReservedMemoryLong() * _byteToM));
				GUILayout.Label(
					string.Format("Unused Reserved: {0}M", Profiler.GetTotalUnusedReservedMemoryLong() * _byteToM));
				GUILayout.Label(
					string.Format("Mono Heap : {0}M", Profiler.GetMonoHeapSizeLong() * _byteToM));
				GUILayout.Label(
					string.Format("Mono Used : {0}M", Profiler.GetMonoUsedSizeLong() * _byteToM));
			}
		}
	}
}

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : UnityEngine.MonoBehaviour
{
	// public Text DebugInfoText;
	public Text PerformanceText;

	public bool ShowProfiler;
	public float ProfilingUpdateFrequency = 4;

	private float _updateTime;
	private int _frameCount;

	private void Awake()
	{
		ToggleProfiling();
	}

	public void ToggleProfiling()
	{
		ShowProfiler = !ShowProfiler;

		if (ShowProfiler)
		{
			StartCoroutine(UpdateProfiling());
		}
	}

	IEnumerator UpdateProfiling()
	{
		while (ShowProfiler)
		{
			++_frameCount;
			_updateTime += Time.deltaTime;
			if (1.0f / ProfilingUpdateFrequency < _updateTime)
			{
				var fps = (int) (_frameCount / _updateTime);
				PerformanceText.text = fps.ToString();
				
				_frameCount = 0;
				_updateTime -= 1.0f / ProfilingUpdateFrequency;
			}

			yield return null;
		}

// #if UNITY_DEBUG
// 		sample.Append("FPS: 000\n");
// 		sample.Append("Total: 00000.0 / 00000.0 (000%)\n");
// 		sample.Append("Mono: 00000.0 / 00000.0 (000%)\n");
// 		sample.Append("Texture: 0000.0");
//
// 		SpeedString = new StringBuilder(sample.Length);
// 		
// 		PerformanceText.enabled = true;
//
// 		while (ShowProfiler)
// 		{
// 			++_frameCount;
// 			_updateTime += Time.deltaTime;
//
// 			if (1.0f / ProfilingUpdateFrequency < _updateTime)
// 			{
// 				SpeedString.Clear();
//
// 				// FPS
// 				var fps = (int)(_frameCount / _updateTime);
// 				SpeedString.Append("FPS: ");
// 				SpeedString.Append(fps);
// 				SpeedString.Append("\n");
//
// 				// Memory usage
// 				var textures = Resources.FindObjectsOfTypeAll(typeof (Texture));
// 				long usage = 0;
//
// 				for (int i = 0; i < textures.Length; i++)
// 					usage += UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(textures[i]);
//
// 				// bytes -> Mbytes 로 변환
// 				uint totalUsed = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemory();
// 				uint totalSize = UnityEngine.Profiling.Profiler.GetTotalReservedMemory();
// 				uint monoUsed = UnityEngine.Profiling.Profiler.GetMonoUsedSize();
// 				uint monoSize = UnityEngine.Profiling.Profiler.GetMonoHeapSize();
//
// 				SpeedString.Append("Total: ");
// 				SpeedString.Append((int)(totalUsed / (1024f * 1024f)));
// 				SpeedString.Append(" / ");
// 				SpeedString.Append((int)(totalSize / (1024f * 1024f)));
// 				SpeedString.Append(" (");
// 				SpeedString.Append((int)(100f * totalUsed / totalSize));
// 				SpeedString.Append(" %)");
// 				SpeedString.Append("\n");
//
// 				SpeedString.Append("Mono: ");
// 				SpeedString.Append((int)(monoUsed / (1024f * 1024f)));
// 				SpeedString.Append(" / ");
// 				SpeedString.Append((int)(monoSize / (1024f * 1024f)));
// 				SpeedString.Append(" (");
// 				SpeedString.Append((int)(100f * monoUsed / monoSize));
// 				SpeedString.Append(" %)");
// 				SpeedString.Append("\n");
//
// 				SpeedString.Append("Texture: ");
// 				SpeedString.Append((int)(usage/(1024*1024)));
// 				SpeedString.Append("\n");
//
// 				// SpeedString.SetText(ref PerformanceText);
// 				PerformanceText.text = SpeedString.ToString();
//
// 				_frameCount = 0;
// 				_updateTime -= 1.0f / ProfilingUpdateFrequency;
// 			}
//
// 			yield return null;
// 		}
// #endif
		PerformanceText.enabled = false;
		yield return null;
	}
}
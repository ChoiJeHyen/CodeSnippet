using EasyMobile;
using UnityEngine;

public class ApplicationStartActions
{
	static public Resolution ThisApplicationResolution;
	//첫씬로드되기 직전
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void ApplicationStart()
	{
		Application.targetFrameRate = 120;
		QualitySettings.vSyncCount = 0;
		var StartRes = Screen.currentResolution;
		float ratio = (float) StartRes.height / (float) StartRes.width;
		ThisApplicationResolution = new Resolution();
		ThisApplicationResolution.width = 720;
		ThisApplicationResolution.height = (int) (720 * ratio);
		Screen.SetResolution(ThisApplicationResolution.width, ThisApplicationResolution.height, true);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	public static void AfterSceneLoaded()
	{
		Application.targetFrameRate = 120;
		QualitySettings.vSyncCount = 0;
		if (!RuntimeManager.IsInitialized())
		{
			RuntimeManager.Init();
		}
	}
}
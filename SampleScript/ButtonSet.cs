using EasyMobile;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
	[UnityEngine.CreateAssetMenu(fileName = "ButtonActions", menuName = "ButtonActions", order = 0)]
	public class ButtonSet : UnityEngine.ScriptableObject
	{
		public void MoveScene(string SceneName)
		{
			SceneManager.LoadScene(SceneName);
		}

		public void SetFrameRate(int fps)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = fps;
		}

		public void ShowRewardAD()
		{
			Advertising.RewardedAdCompleted += RewaredOnComplete;
			Advertising.RewardedAdSkipped += RewaredOnSkip;
			
			var isReady = Advertising.IsRewardedAdReady();
			if (isReady)
			{
				Advertising.ShowRewardedAd();
			}
		}

		private void DeleteAddEvent()
		{
			Advertising.RewardedAdCompleted -= RewaredOnComplete;
			Advertising.RewardedAdSkipped -= RewaredOnSkip;
		}

		private void RewaredOnComplete(RewardedAdNetwork network, AdLocation location)
		{
			Debug.Log("Rewarded ad has completed. The user should be rewarded now.");
			DeleteAddEvent();
		}

		private void RewaredOnSkip(RewardedAdNetwork network, AdLocation location)
		{
			Debug.Log("Rewarded ad was skipped. The user should NOT be rewarded.");
			DeleteAddEvent();
		}
	}
}
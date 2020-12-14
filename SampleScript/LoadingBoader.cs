using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LoadingBoader : MonoBehaviour
{
	public RectTransform[] LoadingMasks;

	public RectTransform FullMask;
	public Ease LoadingEase;
	private Vector2 ScreenSize;

	private static LoadingBoader _instance;
	public static LoadingBoader Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Instantiate(Resources.Load<GameObject>(Prefabs.GetPath(Prefabs._Prefab.LoadingCanvas))).GetComponent<LoadingBoader>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		ScreenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
		DontDestroyOnLoad(gameObject);
		_instance = this;
	}

	[Button]
	public void StartLoading()
	{
		foreach (var loadingMask in LoadingMasks)
		{
			loadingMask.DOKill();
			FullMask.DOKill();
			loadingMask.localScale = Vector3.zero;
			FullMask.localScale = Vector3.zero;
			loadingMask.Rotate(Vector3.forward,Random.Range(0,360));
			loadingMask.anchoredPosition = new Vector2(Random.Range(-ScreenSize.x / 2, ScreenSize.x / 2),
				Random.Range(-ScreenSize.y / 2, ScreenSize.y / 2));
			loadingMask.DOScale(15, 0.8f).From(0).SetEase(LoadingEase).SetDelay(Random.Range(0, 0.3f));
			FullMask.DOScale(26, 0.5f).SetDelay(0.5f).SetEase(LoadingEase);
		}
	}

	[Button]
	public void EndLoading()
	{
		foreach (var loadingMask in LoadingMasks)
		{
			loadingMask.DOKill();
			FullMask.DOKill();
			loadingMask.localScale = Vector3.one * 15;
			FullMask.localScale = Vector3.one * 26;
			FullMask.DOScale(0, 0.6f).SetEase(LoadingEase);
			loadingMask.Rotate(Vector3.forward,Random.Range(0,360));
			loadingMask.anchoredPosition = new Vector2(Random.Range(-ScreenSize.x / 2, ScreenSize.x / 2),
				Random.Range(-ScreenSize.y / 2, ScreenSize.y / 2));
			loadingMask.DOScale(0, 0.8f).From(10).SetEase(LoadingEase).SetDelay(Random.Range(0, 0.3f));
		}
	}

	[Button]
	public void MoveScene(string SceneName)
	{
		StartCoroutine(MoveSceneRoutine(SceneName));
	}

	public static void MoveSceneEndImple(Action MoveSceneendFunc)
	{
		if (_instance == null)
		{
			MoveSceneendFunc();
		}
		else
		{
			_instance.MoveSecneEnd += MoveSceneendFunc;
		}
	} 

	private Action MoveSecneEnd;
	IEnumerator MoveSceneRoutine(string SceneName)
	{
		MoveSecneEnd = null;
		StartLoading();
		yield return new WaitForSecondsRealtime(1);
		
		var async = SceneManager.LoadSceneAsync(SceneName);
		while (!async.isDone)
		{
			yield return new WaitForSecondsRealtime(0.2f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		EndLoading();
		yield return new WaitForSecondsRealtime(1);
		if (MoveSecneEnd != null)
		{
			MoveSecneEnd();
		}
	}
}
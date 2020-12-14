using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Febucci.UI;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SayCanvas : MonoBehaviour
{
	private static SayCanvas _instance;

	public static SayCanvas Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Instantiate(Resources.Load<GameObject>(Prefabs.GetPath(Prefabs._Prefab.SayCanvas))).GetComponent<SayCanvas>();
			}

			return _instance;
		}
	}

	public TextAnimatorPlayer TextPlayer;

	public RectTransform Background;

	public SayData Saydata;

	public TextAnimatorPlayer Name;

	private Say currentSay;
	private int currentSayIdx;

	private bool isTypeWriterEnd;

	public GameObject BranchWindow;
	public Button[] BranchButtons;

	private void Awake()
	{
		_instance = this;
		TextPlayer.onTypeWriterEnded += CheckTypeEnd;
		DontDestroyOnLoad(gameObject);
	}

	private void OnDestroy()
	{
		TextPlayer.onTypeWriterEnded -= CheckTypeEnd;
	}

	private void CheckTypeEnd()
	{
		isTypeWriterEnd = true;
	}

	public void AddSayData(SayData sayData)
	{
		Saydata = sayData;
		currentSay = Saydata.SayList.First();
		currentSayIdx = 0;
		TextPlayer.GetComponent<TextMeshProUGUI>().text = "";
		Name.ShowText(sayData.name);
		if (IsShowNow)
		{
			ShowNext();
		}
		else
		{
			StartCoroutine(StartSay());
		}
	}

	private void ShowNext()
	{
		if (currentSay.SayText.Length > currentSayIdx)
		{
			isTypeWriterEnd = false;
			Background.DOKill();
			Background.transform.localScale = Vector3.one;
			Background.transform.DOShakeScale(0.2f,0.1f,5,50);
			TextPlayer.ShowText(currentSay.SayText[currentSayIdx]);
			currentSayIdx++;
		}
		else
		{
			switch (currentSay.LastOption)
			{
			case SayLastFunc.None:
				SetCurrentSay(currentSay.NextIndex);
				break;
			case SayLastFunc.Branch:
				//브랜치 창열자.
				if (BranchWindow.gameObject.activeSelf)
				{
					BranchWindow.transform.DOKill();
					BranchWindow.transform.localScale = Vector3.one;
					BranchWindow.transform.DOShakeScale(0.2f,0.2f,5,50);
					break;
				}
				BranchWindow.gameObject.SetActive(true);
				var idx = 0;
				foreach (var button in BranchButtons)
				{
					if (idx < currentSay.BranchIndex.Length)
					{
						var nextidx = currentSay.BranchIndex[idx];
						button.gameObject.SetActive(true);
						button.GetComponentInChildren<TextAnimatorPlayer>().ShowText(currentSay.BranchSay[idx]);
						button.onClick.RemoveAllListeners();
						button.onClick.AddListener(() =>
						{
							SetCurrentSay(nextidx);
							BranchWindow.transform.DOScale(0, 0.2f).From(1).SetEase(Ease.Flash).OnComplete(() =>
							{
								BranchWindow.gameObject.SetActive(false);
							});
						});
					}
					else
					{
						button.gameObject.SetActive(false);
					}
					idx++;
				}
				BranchWindow.transform.DOScale(1, 0.2f).From(0).SetEase(Ease.Flash);
				break;
			case SayLastFunc.Action:
				if (currentSay.Action != null)
				{
					currentSay.Action.Invoke();
				}
				SetCurrentSay(currentSay.NextIndex);
				break;
			}
		}
	}

	private void SetCurrentSay(int idx)
	{
		if (idx == -1)
		{
			StartCoroutine(Close());
		}
		else
		{
			currentSay = Saydata.SayList[idx];
			currentSayIdx = 0;
			ShowNext();
		}
	}

	public void ShowButton()
	{
		if (isTypeWriterEnd)
		{
			ShowNext();
		}
		else
		{
			TextPlayer.SkipTypewriter();
		}
	}

	private bool IsShowNow = false;
	IEnumerator StartSay()
	{
		IsShowNow = true;
		Background.gameObject.SetActive(true);
		Background.DOKill();
		Background.DOScale(1, 0.2f).From(0).SetEase(Ease.Flash);
		yield return new WaitForSecondsRealtime(0.2f);

		// Background.DOKill();
		// DOTween.Sequence().Append(Background.DOScaleX(1.05f, 1f).SetEase(Ease.Linear))
		//     .Join(Background.DOScaleY(1.1f, 1).SetEase(Ease.Linear))
		//     .SetLoops(-1, LoopType.Yoyo);

		TextPlayer.transform.localScale = Vector3.one;
		TextPlayer.gameObject.SetActive(true);
		ShowNext();
	}

	public Action CloseImpl;

	IEnumerator Close()
	{
		IsShowNow = false;
		Background.DOKill();
		Background.DOScale(0, 0.2f).From(1).SetEase(Ease.Flash);
		TextPlayer.transform.DOScale(0, 0.2f).From(1).SetEase(Ease.Flash);
		yield return new WaitForSecondsRealtime(0.2f);
		Background.gameObject.SetActive(false);
		TextPlayer.gameObject.SetActive(false);
		if (CloseImpl != null)
		{
			CloseImpl();
		}

		CloseImpl = null;
	}
}
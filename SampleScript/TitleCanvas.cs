using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    public Material TitleSkyBoxMat;
    public TextMeshProUGUI TitleText;

    private void Awake()
    {
        TitleText.color = new Color(1f, 1f, 1f, 0f);
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        DOTween.Sequence().
            Append(TitleText.DOFade(1, 1f).SetEase(Ease.Flash).From(0)).
            Append(TitleText.transform.DOShakeScale(2.5f,1,5,60));
        
        yield return new WaitForSeconds(4);
        LoadingBoader.Instance.MoveScene("Main");
    }

}

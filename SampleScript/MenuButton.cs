using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public GameObject MenuBackground;
    void Start()
    {
        
    }

    public void ClickMenuButton()
    {
        MenuBackground.SetActive(true);
        MenuBackground.transform.localScale = Vector3.one;
        MenuBackground.transform.DOKill();
        MenuBackground.transform.DOShakeScale(0.4f);
    }
}

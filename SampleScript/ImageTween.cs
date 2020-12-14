using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ImageTween : MonoBehaviour
{
    public enum TweenSelect
    {
        Move,Scale,Rotate
    }

    public TweenSelect Trans;
    public Vector3 StartVariable;
    public Vector3 EndVariable;
    public float Duration;
    public Ease EaseType;
    
    void Start()
    {
        var image = GetComponent<Image>();
        switch (Trans)
        {
        case TweenSelect.Move:
            transform.position = StartVariable;
            transform.DOMove(EndVariable, Duration).SetEase(EaseType).SetLoops(-1, LoopType.Yoyo);
            break;
        case TweenSelect.Rotate:
            break;
        case TweenSelect.Scale:
            transform.localScale = StartVariable;
            transform.DOScale(EndVariable, Duration).SetEase(EaseType).SetLoops(-1, LoopType.Yoyo);
            break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

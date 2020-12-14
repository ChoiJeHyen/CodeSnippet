using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutTrigger : MonoBehaviour
{
	public GameObject TestGameObject;
	public Text CountTex;
	public int Count = 0;

	public SayData TestSayData;

	private void Awake()
	{
		//차라리 플레이어레졸루션세팅에 DPI를건들자. 이것도 기기마다 너무천차만별임.
		// var StartRes = Screen.currentResolution;
		// float ratio =(float)StartRes.width/ (float)StartRes.height;
	}

	void Start()
	{
		var boxcol = GetComponent<BoxCollider2D>();
		var ratio = (float) Screen.width / (float) Screen.height;

		boxcol.size = new Vector2(40 * ratio, 40);
		SayCanvas.Instance.AddSayData(SayDataDataBase.Instance[SayDataDataBase.FirstSay]);
	}

	public void ScreenWindowModeButton(int mode)
	{
		// var StartRes = Screen.currentResolution;
		// float ratio = (float) StartRes.height / (float) StartRes.width;
		// float targetratio = 1280f / 720f;
		// Screen.SetResolution(720, (int) (720 * ratio), true);
		//
		// switch (mode)
		// {
		// case 0:
		// 	Debug.Log("뭐야??");
		// 	Screen.SetResolution(720, 1280 , FullScreenMode.Windowed);
		// 	break;
		// case 1:
		// 	Debug.Log("뭐야??");
		// 	Screen.SetResolution(720, 1280, FullScreenMode.MaximizedWindow);
		// 	break;
		// case 2:
		// 	Debug.Log("뭐야??");
		// 	Screen.SetResolution(720, 1280, FullScreenMode.ExclusiveFullScreen);
		// 	break;
		// case 3:
		// 	Debug.Log("뭐야??");
		// 	Screen.SetResolution(720, 1280, FullScreenMode.FullScreenWindow);
		// 	break;
		// default:
		// 	break;
		// }
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnMouseUpAsButton()
	{
		Instantiate(TestGameObject, Vector3.zero, Quaternion.identity);
		Count++;
		CountTex.text = Count.ToString();
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var pos = other.transform.position;
		pos.Scale(-Vector3.one);
		other.transform.position = pos;
	}
}
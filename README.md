# CodeSnippet
여러 프로젝트에 재사용 가능한 코드들 정리용도로 만든 곳.

# ScriptableObject Singleton pattern
Unity ScriptableObject를 활용하여 asset Instance를 만들어 싱글톤처럼 사용하는 기법.

``` c#
using UnityEngine;

public class ScriptableObjInstance<T> : ScriptableObject where T : ScriptableObject
{
	private const string DataPath = "폴더이름";

	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance != null)
			{
				return _instance;
			}

			_instance = Resources.Load<T>($"{DataPath}/{typeof(T).Name}");

#if UNITY_EDITOR
			if (_instance == null)
			{
				//파일이 진짜 없는지 한번더 체크.
				//파일이있는데 덮어쓸경우 작업한 데이터가 다 날아갈수있으므로 최대한 보수적으로 체크한다.
				if (!System.IO.File.Exists($"{Application.dataPath}/Resources/{DataPath}/{typeof(T).Name}.asset"))
				{
					_instance = CreateInstance<T>();
					UnityEditor.AssetDatabase.CreateAsset(_instance,
						$"Assets/Resources/{DataPath}/{typeof(T).Name}.asset");
					UnityEditor.AssetDatabase.SaveAssets();
					Debug.LogWarning(typeof(T).Name + " 데이터를 찾지못해서 생성하였습니다.");
				}
				else
				{
					Debug.LogError($"{DataPath}/{typeof(T).Name}.asset 파일이 존재함");
				}
			}
#endif
			return _instance;
		}
	}
}
```
사용성
```c#
public class 싱글턴클레스명 : ScriptableObjInstance<싱글턴클레스명>
{
}
```

#Unity ButtonAttribute
모노비헤이비어 컴포넌트의 특정함수를 인스펙터에서 바로 실행시키고 싶을때 사용
```c#
using System;
using System.Linq;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ButtonAttribute : Attribute
{
	public string Name;
	public ButtonAttribute()
	{
	}
}

#if UNITY_EDITOR
	namespace Internal
	{
		using UnityEditor;
		using System.Reflection;

		[CanEditMultipleObjects]
		[CustomEditor(typeof(UnityEngine.Object), true)]
		public class ButtonAttributeDrawer : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();
				var methods = this.target.GetType()
					.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					.Where(m => m.GetParameters().Length == 0);
				foreach (var methodInfo in methods)
				{
					var b = Attribute.GetCustomAttribute(methodInfo, typeof(ButtonAttribute));
					if (b != null)
					{
						if (GUILayout.Button(methodInfo.Name))
						{
							foreach (var t in this.targets)
							{
								methodInfo.Invoke(t,null);
							}
						}
					}
				}
			}
		}
	}
#endif
```

사용성
```c#
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [Button]
    public void TestButton(){
    
    }
}
```


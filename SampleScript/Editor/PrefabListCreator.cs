using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabListCreator : AssetPostprocessor
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
		string[] movedFromAssetPaths)
	{
		var isDirty = false;

		// 추가
		foreach (string str in importedAssets)
		{
			if (IsResource(str))
			{
				Debug.Log("PrefabListCreator Import: " + str);

				isDirty = true;
			}
		}

		// 제거
		foreach (string str in deletedAssets)
		{
			if (IsResource(str))
			{
				Debug.Log("PrefabListCreator Deleted: " + str);

				isDirty = true;
			}
		}

		// 이동
		for (int i = 0; i < movedAssets.Length; i++)
		{
			if (IsResource(movedAssets[i]) || IsResourcePath(movedFromAssetPaths[i]))
			{
				Debug.Log("PrefabListCreator Moved: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);

				isDirty = true;
			}
		}

		if (isDirty)
		{
			CreatePrefabList();
			CreatePrefabKey();
		}
	}

	private static bool IsResource(string path)
	{
		path = path.Replace("\\", "/");

		return path.Contains("Resources/") && path.Contains(".prefab");
	}

	private static bool IsResourcePath(string path)
	{
		path = path.Replace("\\", "/");

		return path.Contains("Resources/");
	}

	private static string GetFolderPath(string path)
	{
		path = path.Replace("\\", "/");
		path = path.Substring(path.IndexOf("Resources") + 9);
		path = path.Substring(0, path.LastIndexOf("/"));

		return path;
	}

	private static string GetResourcePath(string path)
	{
		path = path.Replace("\\", "/");
		path = path.Substring(path.IndexOf("Resources/") + 10);
		path = path.Substring(0, path.LastIndexOf("."));

		return path;
	}

	/// <summary>
	/// 폴더 내의 prefab들의 list를 생성
	/// </summary>
	private static void CreatePrefabList()
	{
		var fs = new FileStream(Application.dataPath + "/Resources/" + String.Format("Prefabs.txt"), FileMode.Create);
		var writer = new StreamWriter(fs, System.Text.Encoding.Unicode);

		var dir = new DirectoryInfo(Application.dataPath + "/Resources/");
		var fileInfos = dir.GetFiles("*.prefab*", SearchOption.AllDirectories)
			.Where(s => s.FullName.EndsWith(".prefab"));

		var begin = true;

		foreach (var f in fileInfos)
		{
			if (!begin)
			{
				writer.WriteLine();
			}

			writer.Write(GetResourcePath(f.FullName));

			begin = false;
		}

		writer.Flush();
		writer.Close();
		fs.Close();
	}

	public static IEnumerable<FileInfo> GetFileInfos(string path, string searchPattern = "*.*",
		string extention = ".prefab")
	{
		var info = new DirectoryInfo(path);

		return info.GetFiles(searchPattern, SearchOption.AllDirectories).Where(s => s.FullName.EndsWith(extention));
	}

	public static string GetFileName(string path, string extension = ".asset")
	{
		path = path.Replace("\\", "/");
		path = path.Substring(path.LastIndexOf("/") + 1);
		path = path.Replace(extension, "");
		path = path.Replace(" ", "_");

		return path;
	}

	/// <summary>
	/// 폴더 내의 prefab들의 key코드를 생성
	/// </summary>
	private static void CreatePrefabKey()
	{
		var fs = new FileStream(Application.dataPath + "/Script/" + String.Format("Prefabs.cs"), FileMode.Create);
		var w = new StreamWriter(fs, System.Text.Encoding.Unicode);

		var resources = GetFileInfos(Application.dataPath + "/Resources/", "*.prefab", ".prefab");
		//var cResources = GetFileInfos(Application.dataPath + "/CommonAssets/Resources/", "*.prefab*", ".prefab");
		var tab = "    ";

		var fileInfos = resources;//.Concat(resources);

		w.WriteLine("using System;");
		w.WriteLine("using System.Collections.Generic;");
		w.WriteLine("");
		//w.WriteLine("namespace Common");
		//w.WriteLine("{");
		w.WriteLine(tab + "public static class Prefabs");
		w.WriteLine(tab + "{");

		var folderDic = new Dictionary<string, HashSet<string>>();

		foreach (var f in fileInfos)
		{
			var folderPath = GetFolderPath(f.FullName).Replace("/", "_");
			var resourceName = GetFileName(f.FullName, ".prefab").Replace(" ", "_");

			if (!folderDic.ContainsKey(folderPath))
			{
				folderDic.Add(folderPath, new HashSet<string>());
			}

			if (!folderDic[folderPath].Contains(resourceName))
			{
				folderDic[folderPath].Add(resourceName);
			}
			else
			{
				throw new ArgumentException(string.Format("PrefabListCreator: already has this key. {0}/{1}",
					folderPath, resourceName));
			}
		}

		foreach (var pair in folderDic)
		{
			w.WriteLine(tab + tab + "public enum " + pair.Key);
			w.WriteLine(tab + tab + "{");

			foreach (var resourceName in pair.Value)
			{
				w.WriteLine(tab + tab + tab + resourceName + ",");
			}

			w.WriteLine(tab + tab + "}");
			w.WriteLine();
		}

		w.WriteLine(
			tab + tab + "private static readonly Dictionary<string, string> _datas = new Dictionary<string, string>()");
		w.WriteLine(tab + tab + "{");

		foreach (var f in fileInfos)
		{
			w.WriteLine(tab + tab + tab + "{ \"" + GetFileName(f.FullName, ".prefab").Replace(" ", "_") +
			            "\",  \"" + GetResourcePath(f.FullName) + "\" },");
		}

		w.WriteLine(tab + tab + "};");
		w.WriteLine();
		w.WriteLine(tab + tab + "public static string GetPath(string key)");
		w.WriteLine(tab + tab + "{");
		w.WriteLine(tab + tab + tab + "if (_datas.ContainsKey(key))");
		w.WriteLine(tab + tab + tab + "{");
		w.WriteLine(tab + tab + tab + tab + "return _datas[key];");
		w.WriteLine(tab + tab + tab + "}");
		w.WriteLine(tab + tab + tab +
		            "throw new KeyNotFoundException(string.Format(\"PrefabDatas: there is no such Key {0}\", key));");
		w.WriteLine(tab + tab + "}");
		w.WriteLine();

		foreach (var pair in folderDic)
		{
			w.WriteLine(tab + tab + "public static string GetPath(" + pair.Key + " key)");
			w.WriteLine(tab + tab + "{");
			w.WriteLine(tab + tab + tab + "return GetPath(key.ToString());");
			w.WriteLine(tab + tab + "}");
			w.WriteLine();
		}

		w.WriteLine(tab + "}");
		//w.WriteLine("}");

		w.Flush();
		w.Close();
		fs.Close();
	}
}
using System;
using System.Collections.Generic;

    public static class Prefabs
    {
        public enum _Prefab
        {
            BackGroundTrigger,
            LoadingCanvas,
            SayCanvas,
        }

        public enum _Square
        {
            Square,
        }

        private static readonly Dictionary<string, string> _datas = new Dictionary<string, string>()
        {
            { "BackGroundTrigger",  "Prefab/BackGroundTrigger" },
            { "LoadingCanvas",  "Prefab/LoadingCanvas" },
            { "SayCanvas",  "Prefab/SayCanvas" },
            { "Square",  "Square/Square" },
        };

        public static string GetPath(string key)
        {
            if (_datas.ContainsKey(key))
            {
                return _datas[key];
            }
            throw new KeyNotFoundException(string.Format("PrefabDatas: there is no such Key {0}", key));
        }

        public static string GetPath(_Prefab key)
        {
            return GetPath(key.ToString());
        }

        public static string GetPath(_Square key)
        {
            return GetPath(key.ToString());
        }

    }

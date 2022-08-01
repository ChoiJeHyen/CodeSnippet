using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Tables;

static public class LocalizationRuntimeEntryPusher
{
    public static void SetLocalizationText(this Text textComp, string setText) {
        if (!textComp.TryGetComponent<LocalizeStringEvent>(out var localizeEvent)) {
            localizeEvent = textComp.gameObject.AddComponent<LocalizeStringEvent>();
            localizeEvent.OnUpdateString.AddListener((str) => { textComp.text = str; });
        }
        string key = "DynamicText/" + setText;
#if UNITY_EDITOR
        var localitable = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection("LocalizationTable");
        //키가 없을 경우에만 추가.
        if (!localitable.SharedData.Contains(key)) {
            var tables = localitable.Tables;
            foreach (var table in tables) {
                var sTable = table.asset as StringTable;
                if (sTable.LocaleIdentifier == "ko") {
                    sTable.AddEntry(key, setText);
                }
                //다른언어임을 표시하는 프리텍스트 붙혀서 일단 기본으로 체워주도록.
                else {
                    sTable.AddEntry(key, "(" + table.asset.LocaleIdentifier + ")" + setText);
                }
            }

            foreach (var item in tables) {
                UnityEditor.EditorUtility.SetDirty(item.asset);
            }
            UnityEditor.EditorUtility.SetDirty(localitable);
            UnityEditor.EditorUtility.SetDirty(localitable.SharedData);
        }
#endif
        localizeEvent.StringReference.SetReference("LocalizationTable", key);
        textComp.text = setText;
        localizeEvent.RefreshString();
    }
}

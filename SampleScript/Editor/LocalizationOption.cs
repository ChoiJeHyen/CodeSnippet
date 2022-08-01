using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Events;

public static class LocalizationOption
{
    public static string GetPath(this Component component) {
        if (component.transform.parent == null)
            return component.transform.name;
        return component.transform.parent.GetPath() + "/" + component.transform.name;
    }

    [MenuItem("Localization/ClearLocalizationTable")]
    public static void ClearLocalizationTable() {
        LocalizationEditorSettings.GetStringTableCollection("LocalizationTable").ClearAllEntries();
    }

    [MenuItem("GameObject/AddLocalizationComponentInChlid", false,-10)]
    public static void AddLocalizationComponentInChlid() {
        Debug.Log(Selection.activeGameObject.name);
        var stringTable = LocalizationEditorSettings.GetStringTableCollection("LocalizationTable").GetTable("ko") as StringTable;
        var tables = LocalizationEditorSettings.GetStringTableCollection("LocalizationTable").Tables;

        foreach (var item in Selection.activeGameObject.GetComponentsInChildren<Text>(true)) {
            //기본 텍스트가 없는경우에는 스킵
            if (item.text == string.Empty)
                continue;

            //숫자일경우 스킵.
            if (double.TryParse(item.text, out var result))
                continue;

            string key = item.GetPath();

            //키값이 존재하지않을경우 키 생성.
            //우연히 하이라키에 같은 이름이 생긴다면 덮어쓰니까 이름바꾸는방식으로 대응해서 수정해야함.
            foreach (var table in tables) {
                var sTable = table.asset as StringTable;
                if (sTable.LocaleIdentifier == "ko") {
                    sTable.AddEntry(key, item.text);
                }
                //다른언어임을 표시하는 프리텍스트 붙혀서 일단 기본으로 체워주도록.
                else {
                    sTable.AddEntry(key, "(" + table.asset.LocaleIdentifier + ")" + item.text);
                }
            }

            //로컬라제이션 이벤트가 없을경우 추가.
            if (!item.TryGetComponent<LocalizeStringEvent>(out var localCompo)) {
                localCompo = item.gameObject.AddComponent<LocalizeStringEvent>();
            }

            //로컬라제이션 테이블 키에 값 추가.
            localCompo.StringReference.SetReference("LocalizationTable", key);

            //업데이트 유니티 이벤트 제거
            var eventCount = localCompo.OnUpdateString.GetPersistentEventCount();
            for (int i = 0; i < eventCount; i++) {
                UnityEventTools.RemovePersistentListener(localCompo.OnUpdateString, 0);
            }

            //업데이트 이벤트 추가.
            var str_execute = System.Delegate.CreateDelegate(typeof(UnityAction<string>), item, "set_text") as UnityAction<string>;
            UnityEventTools.AddPersistentListener<string>(localCompo.OnUpdateString, str_execute);
            EditorUtility.SetDirty(localCompo);
        }

        foreach (var item in tables) {
            EditorUtility.SetDirty(item.asset);
        }
        EditorUtility.SetDirty(stringTable);
        EditorUtility.SetDirty(stringTable.SharedData);
    }
}

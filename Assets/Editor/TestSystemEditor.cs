using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(_TestSystem))]
public class TestSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "selectedItem");
        EditorGUILayout.HelpBox("공통 TMP 드롭다운과 각 대상의 +10 / +1 Button을 위 필드에 연결하세요. 버튼 On Click은 자동 등록됩니다. UI Canvas Group에는 개발자 패널을 연결하고, 이 스크립트와 패널 오브젝트는 활성 상태로 유지하세요. F11로 표시를 전환합니다.", MessageType.Info);

        var database = serializedObject.FindProperty("itemDatabase").objectReferenceValue as ItemDatabaseSO;
        SerializedProperty selected = serializedObject.FindProperty("selectedItem");
        var items = new List<ItemDataSO> { null };
        var labels = new List<string> { "아이템 선택" };
        if (database != null && database.Items != null)
        {
            foreach (ItemDataSO item in database.Items)
            {
                if (item == null) continue;
                items.Add(item);
                labels.Add($"{item.ItemName} ({item.ItemId})");
            }
        }

        int current = Mathf.Max(0, items.IndexOf(selected.objectReferenceValue as ItemDataSO));
        int index = EditorGUILayout.Popup("추가할 아이템", current, labels.ToArray());
        selected.objectReferenceValue = items[index];
        bool changed = serializedObject.ApplyModifiedProperties();

        var testSystem = (_TestSystem)target;
        if (changed && Application.isPlaying) testSystem.RefreshItemDropdown();

        EditorGUILayout.HelpBox("Play 모드에서 지급합니다. 플레이어와 통합 전송기를 지정하지 않으면 씬에서 자동으로 찾습니다. 판매대는 공유 인벤토리를 사용합니다.", MessageType.Info);
        using (new EditorGUI.DisabledScope(!Application.isPlaying || selected.objectReferenceValue == null))
        {
            DrawButtons("플레이어", testSystem.OnClickPlayerItem1, testSystem.OnClickPlayerItem10);
            DrawButtons("통합 전송기", testSystem.OnClickTransmitterItem1, testSystem.OnClickTransmitterItem10);
            DrawButtons("판매대", testSystem.OnClickSalesCounterItem1, testSystem.OnClickSalesCounterItem10);
        }

        if (!string.IsNullOrEmpty(testSystem.LastItemResult))
            EditorGUILayout.HelpBox(testSystem.LastItemResult, MessageType.Info);
    }

    private static void DrawButtons(string label, System.Action addOne, System.Action addTen)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(100));
            if (GUILayout.Button("+1")) addOne();
            if (GUILayout.Button("+10")) addTen();
        }
    }
}

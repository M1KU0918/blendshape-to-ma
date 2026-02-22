// BlendShapeToMAShapeChanger.cs
// Modular Avatar が導入済みであることが前提です

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public class BlendShapeToMAShapeChangerWindow : EditorWindow
{
    private SkinnedMeshRenderer targetRenderer;
    private Component targetShapeChanger;

    private bool onlyNonZero = true;
    private bool clearBeforeApply = false;

    private static Type _shapeChangerType;
    private static Type ShapeChangerType
    {
        get
        {
            if (_shapeChangerType != null) return _shapeChangerType;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType("nadena.dev.modular_avatar.core.ModularAvatarShapeChanger");
                if (t != null) { _shapeChangerType = t; break; }
            }
            return _shapeChangerType;
        }
    }

    [MenuItem("Tools/BlendShape to MA ShapeChanger")]
    public static void ShowWindow()
    {
        GetWindow<BlendShapeToMAShapeChangerWindow>("BlendShape to MA");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("BlendShape to MA Shape Changer", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (ShapeChangerType == null)
        {
            EditorGUILayout.HelpBox("Modular Avatar が見つかりません。プロジェクトに MA を導入してください。", MessageType.Error);
            return;
        }

        targetRenderer = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(
            "Skinned Mesh Renderer", targetRenderer, typeof(SkinnedMeshRenderer), true);

        targetShapeChanger = (Component)EditorGUILayout.ObjectField(
            "MA Shape Changer", targetShapeChanger, ShapeChangerType, true);

        EditorGUILayout.Space();
        onlyNonZero = EditorGUILayout.Toggle("0 以外のみ登録", onlyNonZero);
        clearBeforeApply = EditorGUILayout.Toggle("適用前にリストをクリア", clearBeforeApply);
        EditorGUILayout.Space();

        if (targetRenderer != null && targetRenderer.sharedMesh != null)
        {
            int total = targetRenderer.sharedMesh.blendShapeCount;
            int willAdd = 0;
            for (int i = 0; i < total; i++)
                if (!onlyNonZero || targetRenderer.GetBlendShapeWeight(i) != 0f) willAdd++;
            EditorGUILayout.LabelField("BlendShape 総数: " + total);
            EditorGUILayout.LabelField("登録予定: " + willAdd + " 件");
        }

        EditorGUILayout.Space();

        bool canApply = targetRenderer != null
                        && targetRenderer.sharedMesh != null
                        && targetShapeChanger != null;

        GUI.enabled = canApply;
        if (GUILayout.Button("MA Shape Changer に登録", GUILayout.Height(40)))
            Apply();
        GUI.enabled = true;

        if (!canApply)
            EditorGUILayout.HelpBox(
                "Skinned Mesh Renderer と MA Shape Changer の両方を設定してください。",
                MessageType.Info);
    }

    private void Apply()
    {
        var so = new SerializedObject(targetShapeChanger);
        so.Update();

        var shapesProp = so.FindProperty("m_shapes");
        if (shapesProp == null || !shapesProp.isArray)
        {
            Debug.LogError("[BlendShape to MA] m_shapes が見つかりませんでした。MAのバージョンを確認してください。");
            return;
        }

        Mesh mesh = targetRenderer.sharedMesh;

        if (clearBeforeApply)
            shapesProp.ClearArray();

        int added = 0;
        for (int i = 0; i < mesh.blendShapeCount; i++)
        {
            string shapeName = mesh.GetBlendShapeName(i);
            float weight = targetRenderer.GetBlendShapeWeight(i);

            if (onlyNonZero && weight == 0f) continue;

            int existingIdx = FindShapeIndex(shapesProp, shapeName);
            if (existingIdx >= 0)
            {
                shapesProp.GetArrayElementAtIndex(existingIdx)
                    .FindPropertyRelative("Value").floatValue = weight;
            }
            else
            {
                shapesProp.InsertArrayElementAtIndex(shapesProp.arraySize);
                var elem = shapesProp.GetArrayElementAtIndex(shapesProp.arraySize - 1);

                var objProp = elem.FindPropertyRelative("Object");
                if (objProp != null) objProp.objectReferenceValue = targetRenderer;

                elem.FindPropertyRelative("ShapeName").stringValue = shapeName;
                elem.FindPropertyRelative("ChangeType").enumValueIndex = 0;
                elem.FindPropertyRelative("Value").floatValue = weight;
            }
            added++;
        }

        so.ApplyModifiedProperties();
        Debug.Log("[BlendShape to MA] " + added + " 件の BlendShape を登録しました。");
    }

    private int FindShapeIndex(SerializedProperty arr, string name)
    {
        for (int i = 0; i < arr.arraySize; i++)
        {
            var p = arr.GetArrayElementAtIndex(i).FindPropertyRelative("ShapeName");
            if (p != null && p.stringValue == name) return i;
        }
        return -1;
    }
}
#endif

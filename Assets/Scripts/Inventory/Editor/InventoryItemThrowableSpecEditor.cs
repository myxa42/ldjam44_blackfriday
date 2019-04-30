
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(InventoryItemThrowableSpec))]
[CanEditMultipleObjects]
public sealed class InventoryItemThrowableSpecEditor : InventoryItemSpecEditor
{
    SerializedProperty mThrowableProperty;
    Throwable.Visual mCurrentVisual;

    protected override string Category => "Throwable";

    protected override void OnEnable()
    {
        base.OnEnable();

        mThrowableProperty = serializedObject.FindProperty("Throwable");

        AddProperty(serializedObject.FindProperty("Damage"));
        AddProperty(serializedObject.FindProperty("CritProbability"));
    }

    Throwable.Visual GetCurrentVisual()
    {
        return (Throwable.Visual)Enum.GetValues(typeof(Throwable.Visual)).GetValue(mThrowableProperty.enumValueIndex);
    }

    protected override void DrawInspector()
    {
        EditorGUILayout.PropertyField(mThrowableProperty);
    }

    protected override bool ShouldRefreshPreviewObject()
    {
        return (GetCurrentVisual() != mCurrentVisual);
    }

    protected override GameObject CreatePreviewObject()
    {
        GameObject go = null;
        try {
            mCurrentVisual = GetCurrentVisual();
            var prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Throwable.prefab", typeof(GameObject));
            go = (GameObject)Instantiate(prefab);
            go.GetComponent<Throwable>().SetVisual(mCurrentVisual);
        } catch (Exception e) {
            Debug.LogException(e);
        }
        return go;
    }
}

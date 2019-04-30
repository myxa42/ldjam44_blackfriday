
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(InventoryItemConsumableSpec))]
[CanEditMultipleObjects]
public sealed class InventoryItemConsumableSpecEditor : InventoryItemSpecEditor
{
    SerializedProperty mConsumableProperty;
    Consumable.Visual mCurrentVisual;

    protected override string Category => "Consumable";

    protected override void OnEnable()
    {
        base.OnEnable();

        mConsumableProperty = serializedObject.FindProperty("Consumable");

        AddProperty(serializedObject.FindProperty("HealthBonusPercent"));
    }

    Consumable.Visual GetCurrentVisual()
    {
        return (Consumable.Visual)Enum.GetValues(typeof(Consumable.Visual)).GetValue(mConsumableProperty.enumValueIndex);
    }

    protected override void DrawInspector()
    {
        EditorGUILayout.PropertyField(mConsumableProperty);
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
            var prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Consumable.prefab", typeof(GameObject));
            go = (GameObject)Instantiate(prefab);
            go.GetComponent<Consumable>().SetVisual(mCurrentVisual);
        } catch (Exception e) {
            Debug.LogException(e);
        }
        return go;
    }
}

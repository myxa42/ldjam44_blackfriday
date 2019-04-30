
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Globalization;

[CustomEditor(typeof(InventoryItemWeaponSpec))]
[CanEditMultipleObjects]
public sealed class InventoryItemWeaponSpecEditor : InventoryItemSpecEditor
{
    SerializedProperty mWeaponProperty;
    SerializedProperty mBaseDamageProperty;
    SerializedProperty mBaseCritProperty;
    SerializedProperty mSpecialProperty;
    SerializedProperty mSpecialProbabilityProperty;
    SerializedProperty mDamage1Property;
    SerializedProperty mCrit1Property;
    SerializedProperty mDamage2Property;
    SerializedProperty mCrit2Property;
    SerializedProperty mDamage3Property;
    SerializedProperty mCrit3Property;
    SerializedProperty mUpgradeCost1Property;
    SerializedProperty mSpecialCost1Property;
    SerializedProperty mUpgradeCost2Property;
    SerializedProperty mSpecialCost2Property;
    SerializedProperty mUpgradeCost3Property;
    SerializedProperty mSpecialCost3Property;
    Weapon.Visual mCurrentVisual;

    protected override string Category => "Weapon";

    protected override void OnEnable()
    {
        base.OnEnable();

        AddProperty(mWeaponProperty = serializedObject.FindProperty("Weapon"));
        AddProperty(mBaseDamageProperty = serializedObject.FindProperty("BaseDamage"));
        AddProperty(mBaseCritProperty = serializedObject.FindProperty("BaseCritProbability"));
        AddProperty(mSpecialProperty = serializedObject.FindProperty("Special"));
        AddProperty(mSpecialProbabilityProperty = serializedObject.FindProperty("SpecialProbability"));
        AddProperty(mDamage1Property = serializedObject.FindProperty("Damage.Array.data[0]"), "Damage1");
        AddProperty(mCrit1Property = serializedObject.FindProperty("CritProbability.Array.data[0]"), "CritProbability1");
        AddProperty(mDamage2Property = serializedObject.FindProperty("Damage.Array.data[1]"), "Damage2");
        AddProperty(mCrit2Property = serializedObject.FindProperty("CritProbability.Array.data[1]"), "CritProbability2");
        AddProperty(mDamage3Property = serializedObject.FindProperty("Damage.Array.data[2]"), "Damage3");
        AddProperty(mCrit3Property = serializedObject.FindProperty("CritProbability.Array.data[2]"), "CritProbability3");
        AddProperty(mUpgradeCost1Property = serializedObject.FindProperty("UpgradeCost.Array.data[0]"), "UpgradeCost1");
        AddProperty(mUpgradeCost2Property = serializedObject.FindProperty("UpgradeCost.Array.data[1]"), "UpgradeCost2");
        AddProperty(mUpgradeCost3Property = serializedObject.FindProperty("UpgradeCost.Array.data[2]"), "UpgradeCost3");
        AddProperty(mSpecialCost1Property = serializedObject.FindProperty("SpecialUpgradeCost.Array.data[0]"), "SpecialCost1");
        AddProperty(mSpecialCost2Property = serializedObject.FindProperty("SpecialUpgradeCost.Array.data[1]"), "SpecialCost2");
        AddProperty(mSpecialCost3Property = serializedObject.FindProperty("SpecialUpgradeCost.Array.data[2]"), "SpecialCost3");
    }

    Weapon.Visual GetCurrentVisual()
    {
        return (Weapon.Visual)Enum.GetValues(typeof(Weapon.Visual)).GetValue(mWeaponProperty.enumValueIndex);
    }

    static int IndexOf<T>(T value)
    {
        var values = Enum.GetValues(typeof(T));
        int index = 0;
        foreach (var it in values) {
            if (EqualityComparer<T>.Default.Equals((T)Convert.ChangeType(it, typeof(T)), value))
                return index;
            ++index;
        }
        throw new Exception($"\"{value}\" is not a member of \"{typeof(T).FullName}\".");
    }

    protected override void DrawInspector()
    {
        using (new GUILayout.HorizontalScope()) {
            GUILayout.Label("Paste:");

            if (GUILayout.Button("Stats")) {
                string[] items = EditorGUIUtility.systemCopyBuffer.Split('\t');

                mBaseDamageProperty.intValue = Convert.ToInt32(items[2], CultureInfo.InvariantCulture);
                mBaseCritProperty.intValue = Convert.ToInt32(items[3], CultureInfo.InvariantCulture);
                mDamage1Property.intValue = Convert.ToInt32(items[6], CultureInfo.InvariantCulture);
                mCrit1Property.intValue = Convert.ToInt32(items[7], CultureInfo.InvariantCulture);
                mDamage2Property.intValue = Convert.ToInt32(items[8], CultureInfo.InvariantCulture);
                mCrit2Property.intValue = Convert.ToInt32(items[9], CultureInfo.InvariantCulture);
                mDamage3Property.intValue = Convert.ToInt32(items[10], CultureInfo.InvariantCulture);
                mCrit3Property.intValue = Convert.ToInt32(items[11], CultureInfo.InvariantCulture);

                if (items[5].Trim().Length == 0)
                    mSpecialProbabilityProperty.intValue = 0;
                else
                    mSpecialProbabilityProperty.intValue = Convert.ToInt32(items[5], CultureInfo.InvariantCulture);

                switch (items[4].Trim()) {
                    case "": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.None); break;
                    case "разъярить": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.Infuriate); break;
                    case "отравить": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.Poison); break;
                    case "оконфузить": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.Confuse); break;
                    case "оглушить": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.Stun); break;
                    case "ошеломить": mSpecialProperty.enumValueIndex = IndexOf(Weapon.Special.Scare); break;
                    default: throw new Exception($"Unsupported special: \"{items[4]}\".");
                }
            }

            if (GUILayout.Button("Upgrade")) {
                string[] items = EditorGUIUtility.systemCopyBuffer.Split('\t');
                mUpgradeCost1Property.intValue = Convert.ToInt32(items[6], CultureInfo.InvariantCulture);
                mUpgradeCost2Property.intValue = Convert.ToInt32(items[8], CultureInfo.InvariantCulture);
                mUpgradeCost3Property.intValue = Convert.ToInt32(items[10], CultureInfo.InvariantCulture);
            }

            if (GUILayout.Button("Special")) {
                string[] items = EditorGUIUtility.systemCopyBuffer.Split('\t');
                mSpecialCost1Property.intValue = Convert.ToInt32(items[6], CultureInfo.InvariantCulture);
                mSpecialCost2Property.intValue = Convert.ToInt32(items[8], CultureInfo.InvariantCulture);
                mSpecialCost3Property.intValue = Convert.ToInt32(items[10], CultureInfo.InvariantCulture);
            }
        }
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
            var prefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Weapon.prefab", typeof(GameObject));
            go = (GameObject)Instantiate(prefab);
            go.GetComponent<Weapon>().SetVisual(mCurrentVisual);
        } catch (Exception e) {
            Debug.LogException(e);
        }
        return go;
    }
}


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameController))]
[CanEditMultipleObjects]
public sealed class GameControllerEditor : Editor
{
    private List<InventoryItemSpec> mItems;
    private string[] mItemNames;
    private int mAddHealth;
    private int mAddExperience;
    private int mAddMoney;
    private int mSelectedItem;
    private int mSelectedLevel;

    void OnEnable()
    {
        mItems = new List<InventoryItemSpec>();
        var itemNames = new List<string>();
        foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(InventoryItemSpec).Name}")) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var spec = AssetDatabase.LoadAssetAtPath<InventoryItemSpec>(path);
            mItems.Add(spec);

            string type = "";
            if (spec is InventoryItemWeaponSpec)
                type = "Weapon";
            else if (spec is InventoryItemConsumableSpec)
                type = "Food";
            else if (spec is InventoryItemThrowableSpec)
                type = "Throw";

            itemNames.Add($"[{type}] {spec.name}");
        }
        mItemNames = itemNames.ToArray();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!serializedObject.isEditingMultipleObjects && EditorApplication.isPlaying)
            DebugAddToInventory();
    }

    void DebugAddToInventory()
    {
        EditorGUILayout.HelpBox("Stats", MessageType.None, true);

        using (new GUILayout.HorizontalScope()) {
            mAddHealth = EditorGUILayout.IntField("Add health:", mAddHealth);
            if (GUILayout.Button("Add"))
                ((GameController)serializedObject.targetObject).AdjustHealth(mAddHealth);
        }

        using (new GUILayout.HorizontalScope()) {
            mAddExperience = EditorGUILayout.IntField("Add exp:", mAddExperience);
            if (GUILayout.Button("Add"))
                ((GameController)serializedObject.targetObject).AdjustExperience(mAddExperience);
        }

        using (new GUILayout.HorizontalScope()) {
            mAddMoney = EditorGUILayout.IntField("Add money:", mAddMoney);
            if (GUILayout.Button("Add"))
                ((GameController)serializedObject.targetObject).AdjustMoney(mAddMoney);
        }

        using (new GUILayout.HorizontalScope()) {
            GUILayout.Label($"Player Level: {((GameController)serializedObject.targetObject).PlayerLevel}");
            if (GUILayout.Button("Level Up"))
                ((GameController)serializedObject.targetObject).LevelUp();
        }

        EditorGUILayout.HelpBox("Add to inventory", MessageType.None, true);

        mSelectedItem = EditorGUILayout.Popup("Item", mSelectedItem, mItemNames);

        var selectedItem = mItems[mSelectedItem];
        var selectedWeapon = selectedItem as InventoryItemWeaponSpec;
        mSelectedLevel = (selectedWeapon == null ? 0 : EditorGUILayout.IntSlider("Level", mSelectedLevel, 0, 3));

        if (GUILayout.Button("Add")) {
            var controller = (GameController)serializedObject.targetObject;
            var item = (selectedWeapon != null ? selectedWeapon.WithLevel(mSelectedLevel) : selectedItem);
            controller.Inventory.AddItem(item);
        }

        using (new GUILayout.HorizontalScope()) {
            GUILayout.Label("Add all:");

            if (GUILayout.Button("Weapons")) {
                var controller = (GameController)serializedObject.targetObject;
                foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(InventoryItemWeaponSpec).Name}")) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var spec = AssetDatabase.LoadAssetAtPath<InventoryItemWeaponSpec>(path);
                    for (int i = 0; i <= InventoryItemWeaponSpec.NumLevels; i++)
                        controller.Inventory.AddItem(spec.WithLevel(i));
                }
            }

            if (GUILayout.Button("Throwables")) {
                var controller = (GameController)serializedObject.targetObject;
                foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(InventoryItemThrowableSpec).Name}")) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var spec = AssetDatabase.LoadAssetAtPath<InventoryItemThrowableSpec>(path);
                    controller.Inventory.AddItem(spec);
                }
            }

            if (GUILayout.Button("Food")) {
                var controller = (GameController)serializedObject.targetObject;
                foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(InventoryItemConsumableSpec).Name}")) {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    var spec = AssetDatabase.LoadAssetAtPath<InventoryItemConsumableSpec>(path);
                    controller.Inventory.AddItem(spec);
                }
            }
        }
    }
}

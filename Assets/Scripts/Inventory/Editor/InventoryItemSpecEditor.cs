
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public abstract class InventoryItemSpecEditor : Editor
{
    const int IconSize = 256;
    static readonly int PreviewControlID = "IconPreview".GetHashCode();

    struct PropInfo
    {
        public SerializedProperty Property;
        public GUIContent Label;
    }

    List<PropInfo> mProperties;
    SerializedProperty mIconPreviewBrightness;
    SerializedProperty mIconPreviewOffset;
    SerializedProperty mIconPreviewAngles;
    SerializedProperty mIconPreviewPosition;
    SerializedProperty mIconPreviewRotation;
    SerializedProperty mIconPreviewScale;
    SerializedProperty mIcon;
    GameObject mPreviewGameObject;
    PreviewRenderUtility mPreviewRenderUtility;
    Texture2D mWhiteTexture;
    GUIStyle mGUIStyle = new GUIStyle();
    float mCurrentPreviewBrightness;
    Vector3 mCurrentPreviewOffset;
    Vector3 mCurrentPreviewAngles;
    Vector3 mCurrentPreviewPosition;
    Vector3 mCurrentPreviewRotation;
    Vector3 mCurrentPreviewScale;
    bool mShouldRefresh = true;

    protected abstract string Category { get; }

    protected virtual void OnEnable()
    {
        mIconPreviewBrightness = serializedObject.FindProperty("IconPreviewBrightness");
        mIconPreviewOffset = serializedObject.FindProperty("IconPreviewOffset");
        mIconPreviewAngles = serializedObject.FindProperty("IconPreviewAngles");
        mIconPreviewPosition = serializedObject.FindProperty("IconPreviewPosition");
        mIconPreviewRotation = serializedObject.FindProperty("IconPreviewRotation");
        mIconPreviewScale = serializedObject.FindProperty("IconPreviewScale");
        mIcon = serializedObject.FindProperty("Icon");

        mProperties = new List<PropInfo>();
        AddProperty(serializedObject.FindProperty("NameEN"));
        AddProperty(serializedObject.FindProperty("NameES"));
        AddProperty(serializedObject.FindProperty("NameRU"));
        AddProperty(serializedObject.FindProperty("ShopHealthCost"));
        AddProperty(serializedObject.FindProperty("ShopMoneyCost"));

        mWhiteTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        mWhiteTexture.SetPixel(0,0, Color.white);
        mWhiteTexture.Apply();
        mGUIStyle.normal.background = mWhiteTexture;
    }

    protected virtual void OnDisable()
    {
        if (mWhiteTexture != null) {
            DestroyImmediate(mWhiteTexture);
            mWhiteTexture = null;
        }

        DestroyPreview();
    }

    protected void AddProperty(SerializedProperty property, string label = null)
    {
        GUIContent content = (label == null ? null : new GUIContent(label));
        mProperties.Add(new PropInfo{ Property = property, Label = content });
    }

    protected void DestroyPreview()
    {
        if (mPreviewRenderUtility != null) {
            try {
                mPreviewRenderUtility.Cleanup();
            } catch (Exception e) {
                Debug.LogException(e);
            }
            mPreviewRenderUtility = null;
        }

        if (mPreviewGameObject != null) {
            try {
                DestroyImmediate(mPreviewGameObject);
            } catch (Exception e) {
                Debug.LogException(e);
            }
            mPreviewGameObject = null;
        }
    }

    protected abstract bool ShouldRefreshPreviewObject();
    protected abstract GameObject CreatePreviewObject();

    void DrawPreview(bool save = false)
    {
        float iconPreviewBrightness = mIconPreviewBrightness.floatValue;
        Vector3 iconPreviewOffset = mIconPreviewOffset.vector3Value;
        Vector3 iconPreviewAngles = mIconPreviewAngles.vector3Value;
        Vector3 iconPreviewPosition = mIconPreviewPosition.vector3Value;
        Vector3 iconPreviewRotation = mIconPreviewRotation.vector3Value;
        Vector3 iconPreviewScale = mIconPreviewScale.vector3Value;

        if (mShouldRefresh
            || mCurrentPreviewBrightness != iconPreviewBrightness
            || mCurrentPreviewOffset != iconPreviewOffset
            || mCurrentPreviewAngles != iconPreviewAngles
            || mCurrentPreviewPosition != iconPreviewPosition
            || mCurrentPreviewRotation != iconPreviewRotation
            || mCurrentPreviewScale != iconPreviewScale
            || save
            || ShouldRefreshPreviewObject()) {
            DestroyPreview();

            mShouldRefresh = false;
            mCurrentPreviewBrightness = iconPreviewBrightness;
            mCurrentPreviewOffset = iconPreviewOffset;
            mCurrentPreviewAngles = iconPreviewAngles;
            mCurrentPreviewPosition = iconPreviewPosition;
            mCurrentPreviewRotation = iconPreviewRotation;
            mCurrentPreviewScale = iconPreviewScale;

            mPreviewGameObject = CreatePreviewObject();
            if (mPreviewGameObject != null)
                mPreviewGameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        if (mPreviewGameObject == null && !save)
            return;

        if (mPreviewRenderUtility == null) {
            mPreviewRenderUtility = new PreviewRenderUtility();
            mPreviewRenderUtility.camera.fieldOfView = 30.0f;
            mPreviewRenderUtility.camera.allowHDR = false;
            mPreviewRenderUtility.camera.allowMSAA = false;
            mPreviewRenderUtility.ambientColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            mPreviewRenderUtility.lights[0].intensity = iconPreviewBrightness;
            mPreviewRenderUtility.lights[0].transform.position = iconPreviewOffset;
            mPreviewRenderUtility.lights[0].transform.eulerAngles = iconPreviewAngles;
            mPreviewRenderUtility.lights[1].intensity = iconPreviewBrightness;
            mPreviewRenderUtility.camera.transform.position = iconPreviewOffset;
            mPreviewRenderUtility.camera.transform.eulerAngles = iconPreviewAngles;
            mPreviewRenderUtility.camera.nearClipPlane = 0.1f;
            mPreviewRenderUtility.camera.farClipPlane = 10.0f;
            mPreviewRenderUtility.AddSingleGO(mPreviewGameObject);
        }

        mPreviewGameObject.transform.position = iconPreviewPosition;
        mPreviewGameObject.transform.eulerAngles = iconPreviewRotation;
        mPreviewGameObject.transform.localScale = iconPreviewScale;

        Rect previewRect = EditorGUILayout.GetControlRect(false, 200.0f);
        int previewID = GUIUtility.GetControlID(PreviewControlID, FocusType.Passive, previewRect);

        Event e = Event.current;
        EventType eventType = e.GetTypeForControl(previewID);
        bool repaint = eventType == EventType.Repaint;

        if (repaint || save) {
            Texture texture = null;
            mPreviewRenderUtility.BeginPreview(new Rect(0, 0, IconSize, IconSize), mGUIStyle);
            try {
                mPreviewRenderUtility.Render(false);
            } finally {
                texture = mPreviewRenderUtility.EndPreview();
            }

            if (save) {
                UnityEngine.Object obj = serializedObject.targetObject;
                string path = $"Sprites/Inventory/{Category}/{obj.name}.png";

                var rt = (RenderTexture)texture;
                int width = rt.width;
                int height = rt.height;

                RenderTexture.active = rt;
                try {
                    var tex2D = new Texture2D(width, height, TextureFormat.RGB24, false);
                    try {
                        tex2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                        File.WriteAllBytes($"{Application.dataPath}/{path}", tex2D.EncodeToPNG());
                    } finally {
                        DestroyImmediate(tex2D);
                    }
                } finally {
                    RenderTexture.active = null;
                }

                AssetDatabase.Refresh();

                path = $"Assets/{path}";
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                importer.textureType = TextureImporterType.Sprite;
                importer.sRGBTexture = false;
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.None;
                importer.spritePixelsPerUnit = 100.0f;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = IconSize;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                mIcon.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            }

            if (repaint)
                GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit, false);
        }
    }

    protected abstract void DrawInspector();

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        foreach (var it in mProperties) {
            if (it.Label == null)
                EditorGUILayout.PropertyField(it.Property, true);
            else
                EditorGUILayout.PropertyField(it.Property, it.Label, true);
        }

        DrawInspector();

        EditorGUILayout.PropertyField(mIconPreviewBrightness);
        EditorGUILayout.PropertyField(mIconPreviewOffset);
        EditorGUILayout.PropertyField(mIconPreviewAngles);
        EditorGUILayout.PropertyField(mIconPreviewPosition);
        EditorGUILayout.PropertyField(mIconPreviewRotation);
        EditorGUILayout.PropertyField(mIconPreviewScale);
        EditorGUILayout.PropertyField(mIcon);

        if (!serializedObject.isEditingMultipleObjects) {
            bool save = GUILayout.Button("Save");
            DrawPreview(save);
        }

        serializedObject.ApplyModifiedProperties();
    }
}

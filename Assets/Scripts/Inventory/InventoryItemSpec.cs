
using UnityEngine;

public abstract class InventoryItemSpec : ScriptableObject
{
    public string NameEN;
    public string NameES;
    public string NameRU;
    public Sprite Icon;

    public int ShopMoneyCost;
    public int ShopHealthCost;

  #if UNITY_EDITOR
    public float IconPreviewBrightness = 1.4f;
    public Vector3 IconPreviewOffset = new Vector3(-0.3f, 0.0f, -2.0f);
    public Vector3 IconPreviewAngles = new Vector3(0.0f, 0.0f, 45.0f);
    public Vector3 IconPreviewPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 IconPreviewRotation = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3 IconPreviewScale = new Vector3(1.0f, 1.0f, 1.0f);
  #endif

    public string GetLocalizedName()
    {
        switch (Language.Current.Id) {
            case LanguageId.English: return NameEN;
            case LanguageId.Spanish: return NameES;
            case LanguageId.Russian: return NameRU;
        }

        return NameEN;
    }
}

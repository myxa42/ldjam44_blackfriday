
using UnityEngine;

public enum POIType
{
    Start,
    Fight,
    Shop,
    Prize,
    EndOfLevel,
};

public sealed class POI : MonoBehaviour
{
    public POIType Type;
    public string NextLevel;

    public Vector3 Position { get; private set; }

    void Awake()
    {
        Position = transform.position;
    }
}

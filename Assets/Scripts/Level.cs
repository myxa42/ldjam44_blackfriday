
using UnityEngine;

public sealed class Level : MonoBehaviour
{
    public Vector3 OriginalPosition { get; private set; }
    public POI[] POIs;

    void Awake()
    {
        OriginalPosition = transform.position;
    }

    public void SetToPOI(POI poi)
    {
        var t = transform;
        var position = t.position;
        position.x = OriginalPosition.x - poi.Position.x;
        t.position = position;
    }

    public bool MoveTowardsPOI(POI poi, float speed)
    {
        bool done;

        var t = transform;
        var position = t.position;

        float targetX = OriginalPosition.x - poi.Position.x;
        float deltaX = targetX - position.x;

        if (Mathf.Abs(deltaX) <= speed) {
            position.x = targetX;
            done = true;
        } else {
            position.x += (deltaX > 0.0f ? speed : -speed);
            done = false;
        }

        t.position = position;
        return done;
    }
}

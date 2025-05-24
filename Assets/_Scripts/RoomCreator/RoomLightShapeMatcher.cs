#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomLightShapeMatcher {

    public void MatchLightShape(Light2D light, PolygonCollider2D roomCollider) {

        if (light.lightType != Light2D.LightType.Freeform) {
            Debug.LogError("Light type is not Freeform!");
        }

        Vector2[] points = roomCollider.GetPath(0);
        light.SetShapePath(points.Select(p => (Vector3)p).ToArray());


        EditorUtility.SetDirty(light);
    }

    private Vector2 GetLightPoint(Vector2 previousPoint, Vector2 currentPoint, Vector2 nextPoint) {

        float offset = 0.25f;

        bool outerTopRightCorner = currentPoint.x > previousPoint.x && currentPoint.y > nextPoint.y;
        bool outerTopLeftCorner = currentPoint.y > previousPoint.y && currentPoint.x < nextPoint.x;
        bool outerBotRightCorner = currentPoint.y < previousPoint.y && currentPoint.x > nextPoint.x;
        bool outerBotLeftCorner = currentPoint.x < previousPoint.x && currentPoint.y < nextPoint.y;

        bool innerTopRightCorner = currentPoint.y < previousPoint.y && currentPoint.x < nextPoint.x;
        bool innerTopLeftCorner = currentPoint.x > previousPoint.x && currentPoint.y < nextPoint.y;
        //bool innerBotRightCorner = currentPoint.y < previousPoint.y && currentPoint.x > nextPoint.x;
        //bool innerBotLeftCorner = currentPoint.x < previousPoint.x && currentPoint.y < nextPoint.y;

        if (outerTopRightCorner || innerTopRightCorner) {
            return new Vector2(currentPoint.x - offset, currentPoint.y - offset);
        }

        if (outerTopLeftCorner || innerTopLeftCorner) {
            return new Vector2(currentPoint.x + offset, currentPoint.y - offset);
        }

        //if (outerBotRightCorner || innerBotRightCorner) {
        //    return new Vector2(currentPoint.x - offset, currentPoint.y + offset);
        //}

        //if (outerBotLeftCorner || innerBotLeftCorner) {
        //    return new Vector2(currentPoint.x + offset, currentPoint.y + offset);
        //}

        Debug.LogWarning($"Could not determine corner of point: {currentPoint}!");
        return Vector2.zero;
    }
}
#endif

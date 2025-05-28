#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomLightShapeMatcher {

    public void MatchLightShape(Light2D light, PolygonCollider2D roomCollider) {

        if (light.lightType != Light2D.LightType.Freeform) {
            Debug.LogError("Light type is not Freeform!");
            return;
        }

        Undo.RecordObject(light, "Match Light Shape");

        Vector2[] colliderPoints = roomCollider.GetPath(0);
        Vector3[] lightPoints = new Vector3[colliderPoints.Length];

        for (int pointIndex = 0; pointIndex < colliderPoints.Length; pointIndex++) {

            int previousPointIndex = (pointIndex + colliderPoints.Length - 1) % colliderPoints.Length;
            int nextPointIndex = (pointIndex + 1) % colliderPoints.Length;

            lightPoints[pointIndex] = GetLightPoint(colliderPoints[previousPointIndex], colliderPoints[pointIndex], colliderPoints[nextPointIndex], roomCollider.name);
        }

        light.SetShapePath(lightPoints);

        EditorUtility.SetDirty(light);
    }

    private Vector2 GetLightPoint(Vector2 previousPoint, Vector2 currentPoint, Vector2 nextPoint, string roomName) {

        float offset = 0.25f;

        bool outerTopRightCorner = currentPoint.x > previousPoint.x && currentPoint.y > nextPoint.y;
        bool outerTopLeftCorner = currentPoint.y > previousPoint.y && currentPoint.x < nextPoint.x;
        bool outerBotRightCorner = currentPoint.y < previousPoint.y && currentPoint.x > nextPoint.x;
        bool outerBotLeftCorner = currentPoint.x < previousPoint.x && currentPoint.y < nextPoint.y;

        bool innerTopRightCorner = currentPoint.y < previousPoint.y && currentPoint.x < nextPoint.x;
        bool innerTopLeftCorner = currentPoint.x > previousPoint.x && currentPoint.y < nextPoint.y;
        bool innerBotRightCorner = currentPoint.x < previousPoint.x && currentPoint.y > nextPoint.y;
        bool innerBotLeftCorner = currentPoint.y > previousPoint.y && currentPoint.x > nextPoint.x;

        if (outerTopRightCorner || innerTopRightCorner) {
            return new Vector2(currentPoint.x - offset, currentPoint.y - offset);
        }

        if (outerTopLeftCorner || innerTopLeftCorner) {
            return new Vector2(currentPoint.x + offset, currentPoint.y - offset);
        }

        if (outerBotRightCorner || innerBotRightCorner) {
            return new Vector2(currentPoint.x - offset, currentPoint.y + offset);
        }

        if (outerBotLeftCorner || innerBotLeftCorner) {
            return new Vector2(currentPoint.x + offset, currentPoint.y + offset);
        }

        Debug.LogWarning($"Could not determine corner of point: {currentPoint} in {roomName}!");
        return Vector2.zero;
    }
}
#endif

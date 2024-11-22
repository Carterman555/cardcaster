#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
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
}
#endif

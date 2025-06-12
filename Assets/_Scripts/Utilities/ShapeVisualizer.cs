using System.Collections.Generic;
using UnityEngine;

public static class ShapeVisualizer {
    private static List<ShapeData> activeShapes = new List<ShapeData>();
    private static bool isInitialized = false;

    private class ShapeData {
        public System.Action drawAction;
        public float endTime;
        public bool isPermanent;

        public ShapeData(System.Action drawAction, float duration) {
            this.drawAction = drawAction;
            this.isPermanent = duration <= 0f;
            this.endTime = isPermanent ? 0f : Time.time + duration;
        }
    }

    // Initialize the visualizer (call this once, preferably in a MonoBehaviour's Start or Awake)
    public static void Initialize() {
        if (!isInitialized) {
            // Create a GameObject to handle the OnDrawGizmos callback
            GameObject visualizerObject = new GameObject("ShapeVisualizer");
            visualizerObject.AddComponent<ShapeVisualizerComponent>();
            Object.DontDestroyOnLoad(visualizerObject);
            isInitialized = true;
        }
    }

    // Rectangle visualization
    public static void DrawRect(Vector3 point, Vector2 size, float angle = 0f, float duration = 0f, Color? color = null) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;

            // Calculate corners of the rectangle
            Vector3[] corners = GetRectCorners(point, size, angle);

            // Draw the rectangle
            for (int i = 0; i < 4; i++) {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Filled rectangle visualization
    public static void DrawRectFilled(Vector3 point, Vector2 size, float angle = 0f, float duration = 0f, Color? color = null) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;

            // Save current matrix
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply transformation
            Gizmos.matrix = Matrix4x4.TRS(point, Quaternion.AngleAxis(angle, Vector3.forward), Vector3.one);

            // Draw filled cube (which appears as a rectangle in 2D)
            Gizmos.DrawCube(Vector3.zero, new Vector3(size.x, size.y, 0.01f));

            // Restore matrix
            Gizmos.matrix = oldMatrix;
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Circle visualization
    public static void DrawCircle(Vector3 point, float radius, float duration = 0f, Color? color = null, int segments = 32) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;

            Vector3 prevPoint = point + new Vector3(radius, 0, 0);
            for (int i = 1; i <= segments; i++) {
                float angle = (float)i / segments * 2f * Mathf.PI;
                Vector3 newPoint = point + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Filled circle visualization
    public static void DrawCircleFilled(Vector3 point, float radius, float duration = 0f, Color? color = null) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;
            Gizmos.DrawSphere(point, radius);
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Capsule visualization
    public static void DrawCapsule(Vector3 point, Vector2 size, CapsuleDirection2D direction = CapsuleDirection2D.Vertical, float angle = 0f, float duration = 0f, Color? color = null, int segments = 16) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;
            DrawCapsuleGizmo(point, size, direction, angle, segments);
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Filled capsule visualization
    public static void DrawCapsuleFilled(Vector3 point, Vector2 size, CapsuleDirection2D direction = CapsuleDirection2D.Vertical, float angle = 0f, float duration = 0f, Color? color = null) {
        Initialize();
        Color drawColor = color ?? Color.white;

        System.Action drawAction = () => {
            Gizmos.color = drawColor;

            // Save current matrix
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply transformation
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (direction == CapsuleDirection2D.Horizontal)
                rotation *= Quaternion.AngleAxis(90f, Vector3.forward);

            Gizmos.matrix = Matrix4x4.TRS(point, rotation, Vector3.one);

            // Draw capsule using Unity's built-in method
            float height = direction == CapsuleDirection2D.Vertical ? size.y : size.x;
            float radius = direction == CapsuleDirection2D.Vertical ? size.x * 0.5f : size.y * 0.5f;

            // Draw main cylinder
            Gizmos.DrawCube(Vector3.zero, new Vector3(radius * 2f, height - radius * 2f, radius * 2f));

            // Draw top and bottom spheres
            Gizmos.DrawSphere(new Vector3(0, (height - radius * 2f) * 0.5f, 0), radius);
            Gizmos.DrawSphere(new Vector3(0, -(height - radius * 2f) * 0.5f, 0), radius);

            // Restore matrix
            Gizmos.matrix = oldMatrix;
        };

        activeShapes.Add(new ShapeData(drawAction, duration));
    }

    // Clear all shapes
    public static void Clear() {
        activeShapes.Clear();
    }

    // Clear only temporary shapes (keep permanent ones)
    public static void ClearTemporary() {
        activeShapes.RemoveAll(shape => !shape.isPermanent);
    }

    // Clear only permanent shapes
    public static void ClearPermanent() {
        activeShapes.RemoveAll(shape => shape.isPermanent);
    }

    // Internal method to draw all active shapes
    internal static void DrawAllShapes() {
        // Remove expired shapes
        activeShapes.RemoveAll(shape => !shape.isPermanent && Time.time > shape.endTime);

        // Draw all active shapes
        foreach (var shape in activeShapes) {
            shape.drawAction.Invoke();
        }
    }

    // Helper method to get rectangle corners
    private static Vector3[] GetRectCorners(Vector3 center, Vector2 size, float angle) {
        Vector3[] corners = new Vector3[4];
        Vector2 halfSize = size * 0.5f;

        // Local corners
        Vector2[] localCorners = new Vector2[]
        {
            new Vector2(-halfSize.x, -halfSize.y),
            new Vector2(halfSize.x, -halfSize.y),
            new Vector2(halfSize.x, halfSize.y),
            new Vector2(-halfSize.x, halfSize.y)
        };

        // Apply rotation and translation
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);

        for (int i = 0; i < 4; i++) {
            Vector2 rotated = new Vector2(
                localCorners[i].x * cos - localCorners[i].y * sin,
                localCorners[i].x * sin + localCorners[i].y * cos
            );
            corners[i] = center + new Vector3(rotated.x, rotated.y, 0);
        }

        return corners;
    }

    // Helper method to draw capsule wireframe
    private static void DrawCapsuleGizmo(Vector3 point, Vector2 size, CapsuleDirection2D direction, float angle, int segments) {
        float width = direction == CapsuleDirection2D.Vertical ? size.x : size.y;
        float height = direction == CapsuleDirection2D.Vertical ? size.y : size.x;
        float radius = width * 0.5f;
        float cylinderHeight = height - width;

        if (cylinderHeight < 0) cylinderHeight = 0;

        Vector3 offset = direction == CapsuleDirection2D.Vertical ?
            new Vector3(0, cylinderHeight * 0.5f, 0) :
            new Vector3(cylinderHeight * 0.5f, 0, 0);

        // Apply rotation
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        Vector3 rotatedOffset = new Vector3(
            offset.x * cos - offset.y * sin,
            offset.x * sin + offset.y * cos,
            0
        );

        Vector3 topCenter = point + rotatedOffset;
        Vector3 bottomCenter = point - rotatedOffset;

        // Draw semicircles
        DrawSemiCircle(topCenter, radius, direction, angle, true, segments / 2);
        DrawSemiCircle(bottomCenter, radius, direction, angle, false, segments / 2);

        // Draw connecting lines
        Vector3 perpOffset = direction == CapsuleDirection2D.Vertical ?
            new Vector3(radius, 0, 0) :
            new Vector3(0, radius, 0);

        Vector3 rotatedPerpOffset = new Vector3(
            perpOffset.x * cos - perpOffset.y * sin,
            perpOffset.x * sin + perpOffset.y * cos,
            0
        );

        Gizmos.DrawLine(topCenter + rotatedPerpOffset, bottomCenter + rotatedPerpOffset);
        Gizmos.DrawLine(topCenter - rotatedPerpOffset, bottomCenter - rotatedPerpOffset);
    }

    // Helper method to draw semicircle
    private static void DrawSemiCircle(Vector3 center, float radius, CapsuleDirection2D direction, float angle, bool isTop, int segments) {
        float startAngle = direction == CapsuleDirection2D.Vertical ?
            (isTop ? 0f : Mathf.PI) :
            (isTop ? -Mathf.PI * 0.5f : Mathf.PI * 0.5f);

        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++) {
            float currentAngle = startAngle + (i / (float)segments) * Mathf.PI;
            Vector3 localPoint = new Vector3(Mathf.Cos(currentAngle) * radius, Mathf.Sin(currentAngle) * radius, 0);

            // Apply rotation
            Vector3 rotatedPoint = new Vector3(
                localPoint.x * cos - localPoint.y * sin,
                localPoint.x * sin + localPoint.y * cos,
                0
            );

            Vector3 worldPoint = center + rotatedPoint;

            if (i > 0) {
                Gizmos.DrawLine(prevPoint, worldPoint);
            }
            prevPoint = worldPoint;
        }
    }
}
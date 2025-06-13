using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// by claude 4
[CreateAssetMenu(fileName = "ShootPattern", menuName = "Shoot Pattern")]
public class ScriptableShootPattern : ScriptableObject {
    [SerializeField] private Vector2[] positions;
    public Vector2[] Positions => positions;

    [Header("Shape Generators")]
    [SerializeField] private bool useGenerator = false;
    [SerializeField] private ShapeType shapeType = ShapeType.Circle;

    [Header("Circle Settings")]
    [SerializeField] private int circleAmount = 8;
    [SerializeField] private float circleRadius = 5f;

    [Header("Regular Polygon Settings")]
    [SerializeField] private int polygonSides = 6;
    [SerializeField] private float polygonSideLength = 3f;
    [SerializeField] private int pointsPerSide = 5;
    [SerializeField] private float polygonAngle = 0f;

    public enum ShapeType {
        Circle,
        RegularPolygon
    }

    [ContextMenu("Generate Shape")]
    public void GenerateShape() {
        if (!useGenerator) return;

        switch (shapeType) {
            case ShapeType.Circle:
                GenerateCircle();
                break;
            case ShapeType.RegularPolygon:
                GenerateRegularPolygon();
                break;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void GenerateCircle() {
        positions = new Vector2[circleAmount];

        for (int i = 0; i < circleAmount; i++) {
            float angle = (2f * Mathf.PI * i) / circleAmount;
            float x = Mathf.Cos(angle) * circleRadius;
            float y = Mathf.Sin(angle) * circleRadius;
            positions[i] = new Vector2(x, y);
        }
    }

    private void GenerateRegularPolygon() {
        List<Vector2> positionList = new List<Vector2>();

        // Calculate radius from side length for regular polygon
        float angle = 2f * Mathf.PI / polygonSides;
        float radius = polygonSideLength / (2f * Mathf.Sin(angle / 2f));

        // Convert rotation angle to radians
        float rotationRadians = polygonAngle * Mathf.Deg2Rad;

        // Generate vertices of the polygon
        Vector2[] vertices = new Vector2[polygonSides];
        for (int i = 0; i < polygonSides; i++) {
            float vertexAngle = (2f * Mathf.PI * i) / polygonSides + rotationRadians;
            vertices[i] = new Vector2(
                Mathf.Cos(vertexAngle) * radius,
                Mathf.Sin(vertexAngle) * radius
            );
        }

        // Distribute points along each side
        for (int side = 0; side < polygonSides; side++) {
            Vector2 startVertex = vertices[side];
            Vector2 endVertex = vertices[(side + 1) % polygonSides];

            // Add points along this side (excluding the end vertex to avoid duplicates)
            for (int point = 0; point < pointsPerSide; point++) {
                float t = (float)point / pointsPerSide;
                Vector2 position = Vector2.Lerp(startVertex, endVertex, t);
                positionList.Add(position);
            }
        }

        positions = positionList.ToArray();
    }

    private void OnValidate() {
        // Clamp values to reasonable ranges
        circleAmount = Mathf.Max(1, circleAmount);
        circleRadius = Mathf.Max(0.1f, circleRadius);
        polygonSides = Mathf.Max(3, polygonSides);
        polygonSideLength = Mathf.Max(0.1f, polygonSideLength);
        pointsPerSide = Mathf.Max(1, pointsPerSide);

        // Auto-generate when values change in inspector
        if (useGenerator && Application.isEditor) {
            GenerateShape();
        }
    }
}
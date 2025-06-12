using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component class to handle Gizmo drawing
public class ShapeVisualizerComponent : MonoBehaviour {
    void OnDrawGizmos() {
        ShapeVisualizer.DrawAllShapes();
    }
}

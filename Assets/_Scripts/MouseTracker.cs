using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class MouseTracker : StaticInstance<MouseTracker> {


    void Update() {

        Profiler.BeginSample("Sample 1");
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Profiler.EndSample();

        Profiler.BeginSample("Sample 2");
        UpdateHoveredObjects();
        Profiler.EndSample();
    }

    public Vector2 ToMouseDirection(Vector2 origin) {
        Vector2 toMouseDirection = ((Vector2)transform.position - origin).normalized;
        return toMouseDirection;
    }


    private GameObject[] hoveredObjects;

    private void UpdateHoveredObjects() {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        hoveredObjects = hits.Select(hit => hit.collider.gameObject).ToArray();
    }

    public bool IsMouseOver(GameObject gameObject) {
        return hoveredObjects.Contains(gameObject);
    }
}

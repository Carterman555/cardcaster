using System.Linq;
using UnityEngine;

public class MouseTracker : StaticInstance<MouseTracker> {


    void Update() {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        UpdateHoveredObjects();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : StaticInstance<MouseTracker> {

    void Update() {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2 ToMouseDirection(Vector2 origin) {
        Vector2 toMouseDirection = ((Vector2)transform.position - origin).normalized;
        return toMouseDirection;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : StaticInstance<MouseTracker> {

    void Update() {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearTrailOnEnable : MonoBehaviour {

    private TrailRenderer trailRenderer;

    private void Awake() {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable() {
        trailRenderer.Clear();
    }
}

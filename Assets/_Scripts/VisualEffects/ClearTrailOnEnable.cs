using System.Collections;
using UnityEngine;

public class ClearTrailOnEnable : MonoBehaviour {

    private TrailRenderer trailRenderer;

    [SerializeField] private bool useEmissionDelay;
    [ConditionalHide("useEmissionDelay")][SerializeField] private float emissionDelay;

    private void Awake() {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void OnEnable() {
        trailRenderer.Clear();

        if (useEmissionDelay) {
            trailRenderer.emitting = false;
            Invoke(nameof(StartEmitting), emissionDelay);
        }
    }

    private void StartEmitting() {
        trailRenderer.emitting = true;
    }
}

using System;
using UnityEngine;

public class LineSight : MonoBehaviour {

    public event Action OnEnterSight;
    public event Action OnExitSight;

    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float castRadius;

    private Transform target;

    private bool inSight;

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void Update() {
        if (InSight() && !inSight) {
            inSight = true;
            OnEnterSight?.Invoke();
        }
        else if (!InSight() && inSight) {
            inSight = false;
            OnExitSight?.Invoke();
        }
    }

    public bool InSight() {

        if (target == null) {
            Debug.LogWarning("Target not set in line sight!");
            return false;
        }

        Vector2 toTarget = target.position - transform.position;
        float distance = 100f;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, castRadius, toTarget, distance, sightLayerMask);
        return hit.transform == target;
    }
}

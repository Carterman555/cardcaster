using System;
using System.Collections;
using UnityEngine;

public class LineSight : MonoBehaviour {

    public event Action OnEnterSight;
    public event Action OnExitSight;

    [SerializeField] private LayerMask sightLayerMask;
    [SerializeField] private float castRadius;

    [SerializeField] private float checkSightInterval = 0.5f;

    private Transform target;

    private bool previouslyInSight;

    public bool TargetInSight { get; private set; }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    private void OnEnable() {
        StartCoroutine(CheckSight());
    }

    private IEnumerator CheckSight() {
        while (enabled) {

            bool inSight = false;
            if (target != null) {
                inSight = InSight();
            }

            if (inSight && !TargetInSight) {
                TargetInSight = true;
                OnEnterSight?.Invoke();
            }
            else if (!inSight && TargetInSight) {
                TargetInSight = false;
                OnExitSight?.Invoke();
            }

            yield return new WaitForSeconds(checkSightInterval);
        }
    }

    private bool InSight() {

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

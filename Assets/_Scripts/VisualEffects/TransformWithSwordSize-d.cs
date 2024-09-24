using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformWithSwordSize : MonoBehaviour {

    [SerializeField] private bool moveWithSwordSize;
    private Vector2 originalLocalPosition;

    [SerializeField] private bool scaleWithSwordSize;
    private Vector3 originalScale;

    private void Awake() {
        originalLocalPosition = transform.localPosition;
        originalScale = transform.localScale;
    }

    public void SetOriginalLocalPos(Vector2 localPos) {
        originalLocalPosition = localPos;
    }

    private void Update() {

        float swordSize = StatsManager.Instance.GetPlayerStats().SwordSize;

        if (moveWithSwordSize) {
            transform.localPosition = originalLocalPosition * swordSize;
        }

        if (scaleWithSwordSize) {
            transform.localScale = originalScale * swordSize;
        }
    }
}

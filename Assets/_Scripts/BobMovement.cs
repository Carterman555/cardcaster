using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobMovement : MonoBehaviour {

    [SerializeField] private Vector2 amount;
    [SerializeField] private float duration;

    private Tween bobTween;

    private void OnEnable() {
        StartBobbing();
    }
    private void OnDisable() {
        StopBobbing();
    }

    public void StartBobbing() {
        Vector2 targetPosition = (Vector2)transform.position + amount;

        bobTween = transform.DOMove(targetPosition, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopBobbing() {
        bobTween.Kill();
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour {

    [SerializeField] private float distance;
    [SerializeField] private float recoilAngleOffset;

    private Tween tween;

    public void RecoilWeapon() {

        tween?.Kill();

        Vector2 originalPos = transform.localPosition;

        float recoilDuration = 0.1f;
        float recoilAngle = recoilAngleOffset;
        Vector3 recoilDirection = recoilAngle.RotationToDirection().normalized;
        tween = transform.DOLocalMove(transform.localPosition + recoilDirection * distance, recoilDuration).SetEase(Ease.OutSine).OnComplete(() => {
            float returnDuration = 0.5f;
            tween = transform.DOLocalMove(originalPos, returnDuration).SetEase(Ease.InOutSine);
        });
    }

    private void OnDrawGizmos() {

        float recoilAngle = recoilAngleOffset;
        Vector3 recoilDirection = recoilAngle.RotationToDirection().normalized;
        recoilDirection = recoilDirection * distance;
        Gizmos.DrawLine(transform.position, transform.position + recoilDirection);
    }

}

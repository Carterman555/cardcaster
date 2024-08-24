using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour {

    [SerializeField] private float distance;

    private Tween tween;

    public void RecoilWeapon() {

        tween?.Kill();

        Vector2 originalPos = transform.localPosition;

        float recoilDuration = 0.1f;
        Vector3 recoilDirection = transform.localRotation * -Vector2.up;
        tween = transform.DOLocalMove(transform.localPosition + recoilDirection * distance, recoilDuration).SetEase(Ease.OutSine).OnComplete(() => {
            float returnDuration = 0.5f;
            tween = transform.DOLocalMove(originalPos, returnDuration).SetEase(Ease.InOutSine);
        });
    }

}

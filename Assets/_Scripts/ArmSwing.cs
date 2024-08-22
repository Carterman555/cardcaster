using DG.Tweening;
using UnityEngine;

public class ArmSwing : MonoBehaviour {

    [SerializeField] private float speed;
    [SerializeField] private float degrees;

    private float originalRotation;

    private Tween rotationTween;

    private void Awake() {
        originalRotation = transform.eulerAngles.z;
    }

    private void Start() {
        StartRotation();
    }

    public void StartRotation() {

        float swingBackDuration = 1f / speed;
        transform.DOLocalRotate(new Vector3(0, 0, originalRotation - degrees * 0.5f), swingBackDuration).SetEase(Ease.InOutSine).OnComplete(() => {
            float duration = 1f / speed;
            rotationTween = transform.DOLocalRotate(new Vector3(0, 0, degrees), duration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutSine)   // Smooth easing
                .SetLoops(-1, LoopType.Yoyo);  // Loop indefinitely, Yoyo means back and forth
        });
    }

    public void StopRotation() {
        // Stop the rotation tween
        if (rotationTween != null) {
            rotationTween.Kill();
            rotationTween = null;

            float duration = (1f / speed) * 0.5f;
            transform.DOLocalRotate(new Vector3(0, 0, originalRotation), duration).SetEase(Ease.InOutSine);
        }
    }

}

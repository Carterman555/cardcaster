using DG.Tweening;
using UnityEngine;

public class GrowOnEnable : MonoBehaviour {

    [SerializeField] private float duration = 0.3f;

    private void OnEnable() {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration);
    }
}

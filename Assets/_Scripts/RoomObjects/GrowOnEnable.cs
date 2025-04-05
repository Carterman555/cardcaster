using DG.Tweening;
using UnityEngine;

public class GrowOnEnable : MonoBehaviour
{
    private void OnEnable() {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration: 0.3f);
    }
}

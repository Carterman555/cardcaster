using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowOnEnable : MonoBehaviour
{
    private void OnEnable() {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, duration: 0.3f);
    }
}

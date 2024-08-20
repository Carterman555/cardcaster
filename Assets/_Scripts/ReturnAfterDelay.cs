using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnAfterDelay : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnEnable() {
        StartCoroutine(DelayedReturn());
    }

    private IEnumerator DelayedReturn() {
        yield return new WaitForSeconds(delay);
        gameObject.ReturnToPool();
    }
}

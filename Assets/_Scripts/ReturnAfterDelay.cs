using System.Collections;
using UnityEngine;

public class ReturnAfterDelay : MonoBehaviour
{
    [SerializeField] private float delay;

    [SerializeField] private bool shrink;

    private void OnEnable() {
        StartCoroutine(DelayedReturn());
    }

    private IEnumerator DelayedReturn() {
        yield return new WaitForSeconds(delay);
        
        if (shrink) {
            gameObject.transform.ShrinkThenDestroy();
        }
        else {
            gameObject.ReturnToPool();
        }
    }
}

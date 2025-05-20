using System;
using UnityEngine;

public class TriggerEventInvoker : MonoBehaviour {

    public event Action<Collider2D> OnTriggerEnter_Col;
    public event Action OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision) {
        OnTriggerEnter_Col?.Invoke(collision);
        OnTriggerEnter?.Invoke();
    }

    public event Action<Collider2D> OnTriggerExit_Col;
    public event Action OnTriggerExit;

    private void OnTriggerExit2D(Collider2D collision) {
        OnTriggerExit_Col?.Invoke(collision);
        OnTriggerExit?.Invoke();
    }
}

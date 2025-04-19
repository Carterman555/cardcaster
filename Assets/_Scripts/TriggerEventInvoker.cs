using System;
using UnityEngine;

public class TriggerEventInvoker : MonoBehaviour {

    public event Action<Collider2D> OnTriggerEnter_Col;
    public event Action OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision) {
        OnTriggerEnter_Col?.Invoke(collision);
        OnTriggerEnter?.Invoke();
    }
}

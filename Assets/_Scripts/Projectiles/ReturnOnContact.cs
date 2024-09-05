using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnContact : MonoBehaviour, IDelayedReturn {

    public event Action OnStartReturn;

    [SerializeField] private LayerMask layerMask;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (layerMask.ContainsLayer(collision.gameObject.layer)) {
            transform.ShrinkThenDestroy();
            OnStartReturn?.Invoke();
        }
    }
}

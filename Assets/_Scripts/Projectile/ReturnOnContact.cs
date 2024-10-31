using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnContact : MonoBehaviour {

    [SerializeField] private LayerMask layerMask;

    // so this script can be disabled
    private void OnEnable() { }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (layerMask.ContainsLayer(collision.gameObject.layer) && enabled) {
            gameObject.ReturnToPool();
        }
    }
}

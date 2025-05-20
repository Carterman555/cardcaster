using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class ExplodeOnContact : MonoBehaviour {

    private ExplodeBehavior explodeBehavior;

    public GameObject ExcludedObject { get; set; }

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject == ExcludedObject) {
            return;
        }

        explodeBehavior.Explode();
        gameObject.ReturnToPool();
    }
}

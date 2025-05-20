using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class ExplodeOnContact : MonoBehaviour {

    private ExplodeBehavior explodeBehavior;

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        explodeBehavior.Explode();
    }

}

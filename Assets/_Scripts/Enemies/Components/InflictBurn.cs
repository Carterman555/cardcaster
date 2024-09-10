using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class InflictBurn : MonoBehaviour {

    private Health health;

    private float duration;

    private void Awake() {
        health = GetComponent<Health>();
    }

    public void Setup(float duration) {
        this.duration = duration;
    }

    private void Update() {

        // remove this component when duration is up
        duration -= Time.deltaTime;
        if (duration < 0) {
            Destroy(this);
        }

        // deal damage
        float damagePerSecond = 0.5f;
        health.Damage(damagePerSecond * Time.deltaTime);
    }
}

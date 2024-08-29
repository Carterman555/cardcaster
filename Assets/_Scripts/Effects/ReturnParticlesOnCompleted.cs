using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnParticlesOnCompleted : MonoBehaviour {

    private ParticleSystem particles;

    private void Awake() {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (!particles.IsAlive()) {
            gameObject.ReturnToPool();
        }
    }
}

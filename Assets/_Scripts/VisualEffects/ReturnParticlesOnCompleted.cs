using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnParticlesOnCompleted : MonoBehaviour {

    private ParticleSystem particles;

    [SerializeField] private bool returnOther;
    [ConditionalHide("returnOther")]
    [SerializeField] private GameObject returnTarget;

    private void Awake() {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (!particles.IsAlive()) {
            if (!returnOther) {
                gameObject.TryReturnToPool();
            }
            else {
                returnTarget.TryReturnToPool();
            }
        }
    }
}

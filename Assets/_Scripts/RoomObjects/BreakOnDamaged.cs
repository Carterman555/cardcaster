using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnDamaged : MonoBehaviour, IDamagable {

    private Animator anim;

    [SerializeField] private ParticleSystem breakParticlesPrefab;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void Damage(float damage) {
        anim.SetTrigger("break");

        ParticleSystem particles = breakParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        particles.Play();
    }
}

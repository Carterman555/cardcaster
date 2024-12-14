using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnDamaged : MonoBehaviour, IDamagable {

    public event Action<float, bool> OnDamaged_Damage_Shared;

    private Animator anim;

    [SerializeField] private ParticleSystem breakParticlesPrefab;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    public void Damage(float damage, bool shared = false) {
        anim.SetTrigger("break");

        ParticleSystem particles = breakParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        particles.Play();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BreakBarrel);

        OnDamaged_Damage_Shared?.Invoke(damage, shared);
    }
}

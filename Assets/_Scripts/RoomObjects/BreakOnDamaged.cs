using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakOnDamaged : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    public bool Dead { get; private set; }

    private Animator anim;

    [SerializeField] private ParticleSystem breakParticlesPrefab;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void OnEnable() {
        Dead = false;
    }

    public void Damage(float damage, bool shared = false) {
        Dead = true;

        anim.SetTrigger("break");

        ParticleSystem particles = breakParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        particles.Play();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BreakBarrel);

        OnDamaged?.Invoke();
        OnDamaged_Damage_Shared?.Invoke(damage, shared);
    }
}

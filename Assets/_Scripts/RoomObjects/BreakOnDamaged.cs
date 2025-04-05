using System;
using UnityEngine;

public class BreakOnDamaged : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;

    public bool Dead { get; private set; }

    private Animator anim;

    [SerializeField] private ParticleSystem breakParticlesPrefab;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void OnEnable() {
        Dead = false;
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {
        Dead = true;

        anim.SetTrigger("break");

        ParticleSystem particles = breakParticlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
        particles.Play();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BreakBarrel);

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);
    }
}

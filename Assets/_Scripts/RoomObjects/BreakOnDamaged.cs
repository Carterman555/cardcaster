using System;
using UnityEngine;

public class BreakOnDamaged : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;

    public bool Dead { get; private set; }

    private Animator anim;

    [SerializeField] private ParticleSystem breakParticles;
    [SerializeField] private AudioClips breakSfx;

    private void Awake() {
        anim = GetComponent<Animator>();
    }

    private void OnEnable() {
        Dead = false;
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {
        Dead = true;

        //... spawned so particles don't get destroyed when gameobject does
        ParticleSystem spawnedParticles = breakParticles.Spawn(breakParticles.transform.position, Containers.Instance.Effects);
        spawnedParticles.Play();

        if (anim != null) {
            anim.SetTrigger("break");
        }
        else {
            gameObject.TryReturnToPool();
        }

        AudioManager.Instance.PlaySound(breakSfx);

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);
    }
}

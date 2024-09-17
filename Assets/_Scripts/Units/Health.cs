using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable {

    public event Action OnDeath;

    [SerializeField] private UnityEvent damagedEventTrigger;

    private float maxHealth;
    private float health;

    private bool dead;
    private bool invincible;

    [SerializeField] private bool hasDeathParticles;
    [ConditionalHide("hasDeathParticles")]
    [SerializeField] private ParticleSystem deathParticles;
    [ConditionalHide("hasDeathParticles")]
    [SerializeField] private Color deathParticlesColor;

    public bool IsDead() {
        return dead;
    }

    public bool IsInvincible() {
        return invincible;
    }

    public void SetInvincible(bool invincible) {
        this.invincible = invincible;
    }

    private void Awake() {
        Stats stats = GetComponent<IHasStats>().GetStats();
        maxHealth = stats.MaxHealth;
        health = stats.MaxHealth;
    }

    private void OnEnable() {
        dead = false;
        invincible = false;
        health = maxHealth;
    }

    public void Damage(float damage) {

        if (dead || invincible) {
            return;
        }

        health -= damage;

        damagedEventTrigger?.Invoke();

        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        dead = true;

        if (hasDeathParticles) {
            CreateParticles(deathParticles, deathParticlesColor);
        }

        OnDeath?.Invoke();

        gameObject.ReturnToPool();
    }

    private void CreateParticles(ParticleSystem particleSystem, Color color) {
        ParticleSystem particles = particleSystem.Spawn(transform.position, Containers.Instance.Effects);
        var main = particles.main;
        main.startColor = color;
    }
}

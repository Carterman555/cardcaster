using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable {

    public event Action OnDeath;
    public event Action<float> OnHealthChanged_HealthProportion;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    [SerializeField] private UnityEvent damagedEventTrigger;

    private float maxHealth;
    private float health;

    private bool dead;

    public bool IsDead() {
        return dead;
    }

    public bool IsInvincible() {
        bool isInvincible = TryGetComponent(out Invincibility invincibility);
        return isInvincible;
    }

    private void Awake() {
        Stats stats = GetComponent<IHasStats>().GetStats();
        maxHealth = stats.MaxHealth;
        health = stats.MaxHealth;
    }

    private void OnEnable() {
        dead = false;
        health = maxHealth;

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }

    public void Damage(float damage, bool shared = false) {

        if (dead || IsInvincible()) {
            return;
        }

        health -= damage;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DamageEnemy);

        OnHealthChanged_HealthProportion?.Invoke(health/maxHealth);
        damagedEventTrigger?.Invoke();
        OnDamaged_Damage_Shared?.Invoke(damage, shared);

        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        dead = true;

        OnDeath?.Invoke();
        gameObject.ReturnToPool();
    }

    public void Heal(float amount) {

        if (dead) {
            return;
        }

        health = Mathf.MoveTowards(health, maxHealth, amount);

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }
}

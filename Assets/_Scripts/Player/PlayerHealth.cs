using QFSW.QC;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamagable {

    public UnityEvent DeathEventTrigger;
    public event Action OnDeathAnimComplete; // only invokes for player right now from the deathfeedbacks

    public UnityEvent DamagedEventTrigger;
    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;
    public event Action<float> OnHealthChanged_HealthProportion;

    public float HealthProportion => health / maxHealth;
    public bool Dead { get; private set; }
    public float CurrentHealth {
        get {
            return health;
        }
        set {
            health = value;
            OnHealthChanged_HealthProportion?.Invoke(HealthProportion);
        }
    }

    private float maxHealth;
    private float health;

    private void Awake() {
        maxHealth = StatsManager.PlayerStats.MaxHealth;
        health = maxHealth;
    }

    private void OnEnable() {
        Dead = false;
        health = maxHealth;

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);

        StatsManager.OnStatsChanged += TrySetMaxHealth;
    }

    

    public void Damage(float damage, bool shared = false, bool crit = false) {

        if (Dead || IsInvincible()) {
            return;
        }

        health -= damage;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Damaged);

        OnHealthChanged_HealthProportion?.Invoke(health/maxHealth);
        DamagedEventTrigger?.Invoke();

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);

        if (health <= 0) {
            Die();
        }
    }

    [ContextMenu("Die")]
    [Command]
    public void Die() {

        if (Dead) {
            return;
        }

        Dead = true;

        DeathEventTrigger?.Invoke();
    }

    public void InvokeDeathAnimComplete() {
        OnDeathAnimComplete?.Invoke();
    }

    public void Heal(float amount) {

        if (Dead) {
            return;
        }

        health = Mathf.MoveTowards(health, maxHealth, amount);

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }

    public bool IsInvincible() {
        return TryGetComponent(out Invincibility invincibility);
    }

    private void TrySetMaxHealth(PlayerStatType type) {
        if (type == PlayerStatType.MaxHealth) {
            float newMaxHealth = StatsManager.PlayerStats.MaxHealth;
            float changeInMaxHealth = newMaxHealth - maxHealth;
            maxHealth = newMaxHealth;
            health = Mathf.MoveTowards(health, maxHealth, changeInMaxHealth);

            OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
        }
    }
}

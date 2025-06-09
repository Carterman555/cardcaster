using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour, IDamagable {

    public static event Action<EnemyHealth> OnAnyDeath;
    public UnityEvent DeathEventTrigger;

    public UnityEvent DamagedEventTrigger;
    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;
    public event Action<float> OnHealthChanged_HealthProportion;

    [SerializeField] private bool returnOnDeath = true;

    [SerializeField] private bool playSfxOnDeath;
    [SerializeField, ConditionalHide("playSfxOnDeath")] AudioClips deathSfx;

    public float MaxHealth { get; private set; }
    public float Health { get; private set; }
    public float HealthProportion => Health / MaxHealth;

    public bool Dead { get; private set; }

    [SerializeField] private bool increaseHealthPerLevel;
    [ConditionalHide("increaseHealthPerLevel")]
    [SerializeField] private float perLevelProportionToIncrease;

    public bool IsInvincible() {
        return TryGetComponent(out Invincibility invincibility);
    }

    private void OnEnable() {
        Dead = false;

        EnemyStats stats;
        if (TryGetComponent(out IHasEnemyStats hasEnemyStats)) {
            stats = hasEnemyStats.EnemyStats;
        }
        else {
            Debug.LogError("Enemy health does not have IHasEnemyStats behavior!");
            return;
        }

        MaxHealth = stats.MaxHealth;
        if (increaseHealthPerLevel) {
            float proportionIncrease = perLevelProportionToIncrease * (GameSceneManager.Instance.Level - 1);
            MaxHealth += stats.MaxHealth * proportionIncrease;
        }

        SetHealth(MaxHealth);
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {

        if (Dead) {
            return;
        }

        if (IsInvincible()) {
            AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.InvincibleDamaged);
            return;
        }

        Health -= damage;

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.Damaged);

        OnHealthChanged_HealthProportion?.Invoke(HealthProportion);
        DamagedEventTrigger?.Invoke();

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);

        if (Health <= 0) {
            Die();
        }
    }

    [ContextMenu("Die")]
    public void Die() {

        if (Dead) {
            return;
        }

        Dead = true;

        if (playSfxOnDeath) {
            AudioManager.Instance.PlaySingleSound(deathSfx);
        }

        DeathEventTrigger?.Invoke();
        OnAnyDeath?.Invoke(this);

        if (returnOnDeath) {
            gameObject.ReturnToPool();
        }
    }

    // played by boss animation method invoker
    public void ReturnToPool() {
        gameObject.ReturnToPool();
    }

    public void Heal(float amount) {

        if (Dead) {
            return;
        }

        SetHealth(Mathf.MoveTowards(Health, MaxHealth, amount));
    }

    public void SetMaxHealth(float value) {
        MaxHealth = value;

        if (Health > MaxHealth) {
            Health = MaxHealth;
        }

        OnHealthChanged_HealthProportion?.Invoke(HealthProportion);
    }

    public void SetHealth(float value) {
        Health = value;
        OnHealthChanged_HealthProportion?.Invoke(HealthProportion);
    }
}

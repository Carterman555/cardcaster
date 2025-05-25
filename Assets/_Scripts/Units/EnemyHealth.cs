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

    private float maxHealth;
    private float health;
    public float CurrentHealth {
        get {
            return health;
        }
        set {
            health = value;
            OnHealthChanged_HealthProportion?.Invoke(GetHealthProportion());
        }
    }

    public bool Dead { get; private set; }

    [SerializeField] private bool increaseHealthPerLevel;
    [ConditionalHide("increaseHealthPerLevel")]
    [SerializeField] private float perLevelProportionToIncrease;

    public bool IsInvincible() {
        return TryGetComponent(out Invincibility invincibility);
    }

    public float GetHealthProportion() {
        return health / maxHealth;
    }

    private void Awake() {
        EnemyStats stats = GetComponent<IHasEnemyStats>().EnemyStats;

        maxHealth = stats.MaxHealth;
        if (increaseHealthPerLevel) {
            float proportionIncrease = perLevelProportionToIncrease * (GameSceneManager.Instance.Level - 1);
            maxHealth = stats.MaxHealth + (stats.MaxHealth * proportionIncrease);
        }

        health = maxHealth;
    }

    private void OnEnable() {
        Dead = false;
        health = maxHealth;

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {

        if (Dead || IsInvincible()) {
            return;
        }

        health -= damage;

        AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.Damaged);

        OnHealthChanged_HealthProportion?.Invoke(health/maxHealth);
        DamagedEventTrigger?.Invoke();

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);

        if (health <= 0) {
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

        health = Mathf.MoveTowards(health, maxHealth, amount);

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }
}

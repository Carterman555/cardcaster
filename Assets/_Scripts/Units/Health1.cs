using QFSW.QC;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable {

    public static event Action<Health> OnAnyDeath;

    public event Action OnDeath;
    public event Action OnDeathAnimComplete; // only invokes for player right now from the deathfeedbacks
    public event Action<float> OnHealthChanged_HealthProportion;

    public event Action OnDamaged;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    [SerializeField] private bool returnOnDeath = true;

    [SerializeField] private UnityEvent deathEventTrigger;
    [SerializeField] private UnityEvent damagedEventTrigger;

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
        Stats stats = GetComponent<IHasEnemyStats>().EnemyStats;

        maxHealth = stats.MaxHealth;
        if (increaseHealthPerLevel) {
            float proportionIncrease = perLevelProportionToIncrease * (GameSceneManager.Instance.GetLevel() - 1);
            maxHealth = stats.MaxHealth + (stats.MaxHealth * proportionIncrease);
        }

        health = maxHealth;
    }

    private void OnEnable() {
        Dead = false;
        health = maxHealth;

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }

    public void Damage(float damage, bool shared = false) {

        if (Dead || IsInvincible()) {
            return;
        }

        health -= damage;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Damaged);

        OnHealthChanged_HealthProportion?.Invoke(health/maxHealth);
        damagedEventTrigger?.Invoke();

        OnDamaged?.Invoke();
        OnDamaged_Damage_Shared?.Invoke(damage, shared);

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

        deathEventTrigger?.Invoke();
        OnDeath?.Invoke();
        OnAnyDeath?.Invoke(this);

        if (returnOnDeath) {
            gameObject.ReturnToPool();
        }
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
}

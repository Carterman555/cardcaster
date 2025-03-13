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

    private bool dead;

    [SerializeField] private bool increaseHealthPerLevel;
    [ConditionalHide("increaseHealthPerLevel")]
    [SerializeField] private float perLevelProportionToIncrease;

    public bool IsDead() {
        return dead;
    }

    public bool IsInvincible() {
        bool isInvincible = TryGetComponent(out Invincibility invincibility);
        return isInvincible;
    }

    public float GetHealthProportion() {
        return health / maxHealth;
    }

    public float GetHealth() {
        return health;
    }

    private void Awake() {
        Stats stats = GetComponent<IHasStats>().GetStats();

        maxHealth = stats.MaxHealth;
        if (increaseHealthPerLevel) {
            float proportionIncrease = perLevelProportionToIncrease * (GameSceneManager.Instance.GetLevel() - 1);
            maxHealth = stats.MaxHealth + (stats.MaxHealth * proportionIncrease);
        }

        health = maxHealth;
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

        if (dead) {
            return;
        }

        dead = true;

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

        if (dead) {
            return;
        }

        health = Mathf.MoveTowards(health, maxHealth, amount);

        OnHealthChanged_HealthProportion?.Invoke(health / maxHealth);
    }
}

using MoreMountains.Feedbacks;
using QFSW.QC;
using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour, IDamagable {

    public UnityEvent DeathEventTrigger;
    public event Action OnDeathAnimComplete; // only invokes for player right now from the deathfeedbacks

    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;
    public event Action<float> OnHealthChanged_HealthProportion;

    public float HealthProportion => health / StatsManager.PlayerStats.MaxHealth;
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

    private static float health;
    private float lastMaxHealth;

    [SerializeField] private MMF_Player damagedFeedbacks;
    [SerializeField] private ParticleSystem healEffect;


    private void OnEnable() {
        StatsManager.OnStatsChanged += UpdateHealthOnMaxHealthChange;
        GameSceneManager.OnStartGameLoadingCompleted += OnStartGame;

        lastMaxHealth = StatsManager.PlayerStats.MaxHealth;
    }

    private void OnDisable() {
        StatsManager.OnStatsChanged -= UpdateHealthOnMaxHealthChange;
        GameSceneManager.OnStartGameLoadingCompleted -= OnStartGame;
    }

    private void OnStartGame() {
        CurrentHealth = StatsManager.PlayerStats.MaxHealth;

        Dead = false;
    }

    [Command]
    public void Damage(float damage, bool shared = false, bool crit = false) {

        if (Dead || IsInvincible()) {
            return;
        }

        CurrentHealth -= damage;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Damaged);

        damagedFeedbacks.PlayFeedbacks();

        if (CurrentHealth <= 0) {
            Die();
        }

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);
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

    [Command]
    public void Heal(float amount) {

        if (Dead) {
            return;
        }

        CurrentHealth = Mathf.MoveTowards(health, StatsManager.PlayerStats.MaxHealth, amount);

        healEffect.Play();

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.PlayerHeal);
    }

    public bool IsInvincible() {
        return TryGetComponent(out Invincibility invincibility);
    }


    private void UpdateHealthOnMaxHealthChange(PlayerStatType type) {
        if (type == PlayerStatType.MaxHealth) {
            // keep the same health proportion
            float previousHealthProportion = CurrentHealth / lastMaxHealth;
            CurrentHealth = StatsManager.PlayerStats.MaxHealth * previousHealthProportion;
            lastMaxHealth = StatsManager.PlayerStats.MaxHealth;
        }
    }
}

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

    [SerializeField] private ParticleSystem healEffect;

    private void OnEnable() {
        StatsManager.OnStatsChanged += TryIncreaseHealth;
        GameSceneManager.OnStartGameLoadingCompleted += OnStartGame;
        print("sub");

        lastMaxHealth = StatsManager.PlayerStats.MaxHealth;
    }

    private void OnDisable() {
        StatsManager.OnStatsChanged -= TryIncreaseHealth;
        GameSceneManager.OnStartGameLoadingCompleted -= OnStartGame;
        print("unsub");
    }

    private void OnStartGame() {
        CurrentHealth = StatsManager.PlayerStats.MaxHealth;

        Dead = false;
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {

        if (Dead || IsInvincible()) {
            return;
        }

        CurrentHealth -= damage;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Damaged);

        DamagedEventTrigger?.Invoke();

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);

        if (CurrentHealth <= 0) {
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

    private float lastMaxHealth;

    private void TryIncreaseHealth(PlayerStatType type) {
        if (type == PlayerStatType.MaxHealth) {
            float changeInMaxHealth = StatsManager.PlayerStats.MaxHealth - lastMaxHealth;
            CurrentHealth = Mathf.MoveTowards(CurrentHealth, StatsManager.PlayerStats.MaxHealth, changeInMaxHealth);

            lastMaxHealth = StatsManager.PlayerStats.MaxHealth;
        }
    }

    
}

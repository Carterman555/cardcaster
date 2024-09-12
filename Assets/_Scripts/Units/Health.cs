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
        //transform.ShrinkThenDestroy();

        gameObject.ReturnToPool();

        OnDeath?.Invoke();
    }
}

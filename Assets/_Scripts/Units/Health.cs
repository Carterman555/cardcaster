using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable {

    public event Action OnDeath;

    private float maxHealth;
    private float health;

    private bool dead;

    public bool IsDead() {
        return dead;
    }

    private void Awake() {
        Stats stats = GetComponent<IHasStats>().GetStats();
        maxHealth = stats.MaxHealth;
        health = stats.MaxHealth;
    }

    private void OnEnable() {
        dead = false;
        health = maxHealth;
    }

    public void Damage(float damage) {

        if (dead) {
            return;
        }

        health -= damage;

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

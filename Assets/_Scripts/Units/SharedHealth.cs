using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedHealth : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;

    public bool Dead => health.Dead;

    private EnemyHealth health;

    public void SetHealth(EnemyHealth health) {
        this.health = health;
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {
        health.Damage(damage, shared: true);

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);
    }
}

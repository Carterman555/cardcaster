using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedHealth : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    public bool Dead => health.Dead;

    private EnemyHealth health;

    public void SetHealth(EnemyHealth health) {
        this.health = health;
    }

    public void Damage(float damage, bool shared = false) {
        health.Damage(damage, shared: true);

        OnDamaged?.Invoke();
        OnDamaged_Damage_Shared?.Invoke(damage, shared);
    }
}

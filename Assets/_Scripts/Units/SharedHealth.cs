using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedHealth : MonoBehaviour, IDamagable {

    public event Action<float, bool> OnDamaged_Damage_Shared;

    private Health health;

    public void SetHealth(Health health) {
        this.health = health;
    }

    public void Damage(float damage, bool shared = false) {
        health.Damage(damage, shared: true);

        OnDamaged_Damage_Shared?.Invoke(damage, shared);
    }
}

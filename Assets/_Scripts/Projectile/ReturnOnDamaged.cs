using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnOnDamaged : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    public void Damage(float damage, bool shared = false) {
        gameObject.ReturnToPool();

        OnDamaged?.Invoke();
        OnDamaged_Damage_Shared?.Invoke(damage, shared);
    }
}

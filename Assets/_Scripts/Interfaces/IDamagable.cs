using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool> OnDamaged_Damage_Shared;

    public bool Dead { get; }

    public void Damage(float damage, bool shared = false);
}

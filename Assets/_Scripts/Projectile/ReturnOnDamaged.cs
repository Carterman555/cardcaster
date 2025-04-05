using System;
using UnityEngine;

public class ReturnOnDamaged : MonoBehaviour, IDamagable {

    public event Action OnDamaged;
    public event Action<float, bool, bool> OnDamagedDetailed;

    public bool Dead { get; private set; }

    private void OnEnable() {
        Dead = false;
    }

    public void Damage(float damage, bool shared = false, bool crit = false) {
        Dead = true;

        gameObject.ReturnToPool();

        OnDamaged?.Invoke();
        OnDamagedDetailed?.Invoke(damage, shared, crit);
    }
}

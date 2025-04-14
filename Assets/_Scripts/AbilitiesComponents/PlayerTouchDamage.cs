using System;
using UnityEngine;

public class PlayerTouchDamage : MonoBehaviour, ITargetAttacker {

    public event Action<GameObject> OnDamage_Target;
    public event Action OnAttack;

    private float damageMult = 1;

    public void SetDamageMult(float damageMult) {
        this.damageMult = damageMult;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == GameLayers.EnemyLayer && collision.TryGetComponent(out IDamagable damagable) && !damagable.Dead) {
            PlayerMeleeAttack.Instance.ExternalAttack(collision.gameObject, transform.position, damageMult);

            OnAttack?.Invoke();
            OnDamage_Target?.Invoke(collision.gameObject);
        }
    }
}

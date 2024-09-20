using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchDamage : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent(out IDamagable damagable)) {
            PlayerMeleeAttack.Instance.ExternalAttack(collision.gameObject, collision.transform.position);
        }
    }
}

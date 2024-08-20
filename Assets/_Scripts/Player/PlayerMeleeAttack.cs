using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour {

    public static event Action OnAttack;

    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private Transform slashPrefab;
    [SerializeField] private LayerMask enemyLayerMask;

    [SerializeField] private ScriptablePlayer scriptablePlayer;

    private void Update() {
        //if (attackInput.action.triggered) {
        if (Input.GetMouseButtonDown(0)) {
            Attack();
        }
    }

    private void Attack() {
        Vector2 toMouseDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        CreateEffect(toMouseDirection);
        DamageEnemies(toMouseDirection);

        OnAttack?.Invoke();
    }

    private void CreateEffect(Vector2 toMouseDirection) {
        slashPrefab.Spawn(transform.position, toMouseDirection.DirectionToRotation(), Containers.Instance.Effects);

    }

    private void DamageEnemies(Vector2 toMouseDirection) {
        Vector2 attackCenter = (Vector2)transform.position + (toMouseDirection * scriptablePlayer.Stats.SwordSize);

        Collider2D[] cols = Physics2D.OverlapCircleAll(attackCenter, scriptablePlayer.Stats.SwordSize, enemyLayerMask);
        foreach (Collider2D col in cols) {
            //if (col.TryGetComponent(out Health health)) {
            //    health.Damage(scriptablePlayer.Stats.Damage);
            //}
            //if (col.TryGetComponent(out Knockback knockback)) {
            //    Vector2 toEnemyDirection = col.transform.position - transform.position;
            //    knockback.ApplyKnockback(toEnemyDirection, scriptablePlayer.Stats.KnockbackStrength);
            //}
        }
    }
}

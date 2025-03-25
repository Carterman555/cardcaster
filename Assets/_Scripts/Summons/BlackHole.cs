using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class BlackHole : MonoBehaviour, IAbilityStatsSetup, ITargetAttacker {

    public event Action OnAttack;
    public event Action<GameObject> OnDamage_Target;

    private SuckBehaviour suckBehaviour;

    [SerializeField] private TriggerContactTracker contactTracker;

    private float damage;
    private float duration;

    private bool dying;

    private void Awake() {
        suckBehaviour = GetComponent<SuckBehaviour>();
    }

    private void OnEnable() {
        dying = false;

        StartCoroutine(DealDamage());
    }

    public void SetAbilityStats(AbilityStats stats) {
        damage = stats.Damage;
        duration = stats.Duration;

        suckBehaviour.Setup(stats.AreaSize);
    }

    private void Update() {

        if (dying) {
            return;
        }

        duration -= Time.deltaTime;
        if (duration < 0) {
            dying = true;
            transform.ShrinkThenDestroy();
        }
    }

    private IEnumerator DealDamage() {
        while (enabled && !dying) {

            yield return new WaitForSeconds(1f);

            float attackRadius = contactTracker.GetComponent<CircleCollider2D>().radius;

            Collider2D[] damagedCols = DamageDealer.DealCircleDamage(GameLayers.EnemyLayerMask, transform.position, attackRadius, damage, 0);

            foreach (Collider2D col in damagedCols) {
                OnDamage_Target?.Invoke(col.gameObject);
            }

            OnAttack?.Invoke();
        }
    }
}

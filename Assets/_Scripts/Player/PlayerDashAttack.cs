using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerDashAttack : MonoBehaviour {

    private PlayerMovement playerMovement;
    private PlayerMeleeAttack playerMeleeAttack;

    [SerializeField] private TriggerContactTracker dashDamageTrigger;

    [SerializeField] private float windowToAttack = 0.5f;
    private float dashTimer;

    public bool InDashAttackWindow => dashTimer > 0;
    private PlayerStats Stats => StatsManager.Instance.GetPlayerStats();

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    private void OnEnable() {
        playerMovement.OnDash.AddListener(OnDash);
    }

    private void OnDisable() {
        playerMovement.OnDash.RemoveListener(OnDash);
    }

    private void Update() {
        dashTimer -= Time.deltaTime;

        float angle = playerMeleeAttack.GetAttackDirection().DirectionToRotation().eulerAngles.z;
        dashDamageTrigger.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void OnDash() {
        dashTimer = windowToAttack;
    }

    public Collider2D[] DashAttack() {

        foreach (GameObject target in dashDamageTrigger.GetContacts()) {
            DamageDealer.TryDealDamage(target, transform.position, Stats.Damage, Stats.KnockbackStrength);
        }

        return dashDamageTrigger.GetContacts().Select(g => g.GetComponent<Collider2D>()).ToArray();
    }
}

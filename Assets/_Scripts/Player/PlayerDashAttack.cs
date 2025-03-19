using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashAttack : MonoBehaviour {

    private PlayerMovement playerMovement;
    private PlayerMeleeAttack playerMeleeAttack;

    [SerializeField] private float windowToAttack = 0.5f;
    private float dashTimer;

    public bool InDashAttackWindow => dashTimer > 0;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    private void OnEnable() {
        playerMovement.OnDash.AddListener(OnDash);
        playerMeleeAttack.OnAttack += OnMeleeAttack;
    }

    private void OnDisable() {
        playerMovement.OnDash.RemoveListener(OnDash);
        playerMeleeAttack.OnAttack -= OnMeleeAttack;
    }

    private void OnDash() {
        dashTimer = windowToAttack;
    }

    public void DashAttack() {
        print("dash attack");
    }

    private void Update() {
        dashTimer -= Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeBehavior : MonoBehaviour {

    private float chargeTimer;

    private IHasEnemyStats hasEnemyStats;
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private float initialSpeed;
    [SerializeField] private float deceleration;

    public enum ChargeState { OnCooldown, ChargingUp, Moving }
    public ChargeState CurrentState { get; private set; }

    private void Awake() {
        hasEnemyStats = GetComponent<IHasEnemyStats>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable() {
        if (gameObject.activeSelf && CurrentState != ChargeState.OnCooldown) {
            Debug.LogWarning("Disabled charge behavior while still in " + CurrentState + " which might cause issues!");
        }
    }

    private void Update() {
        chargeTimer += Time.deltaTime;
        if (chargeTimer > hasEnemyStats.EnemyStats.AttackCooldown && CurrentState == ChargeState.OnCooldown) {
            chargeTimer = 0;
            CurrentState = ChargeState.ChargingUp;
            
            anim.SetTrigger("startCharging");
        }
    }

    private void FixedUpdate() {
        if (CurrentState == ChargeState.Moving) {
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, Time.fixedDeltaTime * deceleration);

            if (rb.velocity == Vector2.zero) {
                CurrentState = ChargeState.OnCooldown;
                anim.SetTrigger("stopCharging");
            }
        }
    }

    // played by AnimationMethodInvoker
    public void Charge() {
        CurrentState = ChargeState.Moving;

        Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
        rb.velocity = toPlayerDirection * initialSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameLayers.ObstacleLayerMask.ContainsLayer(collision.gameObject.layer)) {
            rb.velocity = Vector2.zero;
        }
    }
}

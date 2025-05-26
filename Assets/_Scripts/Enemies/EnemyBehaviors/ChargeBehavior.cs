using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeBehavior : MonoBehaviour {

    public event Action<bool> OnCharge; // bool: played by anim

    private IHasEnemyStats hasEnemyStats;
    [SerializeField] private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private float initialSpeed;
    [SerializeField] private float deceleration;

    public enum ChargeState { OnCooldown, ChargingUp, Launching }
    public ChargeState CurrentState { get; private set; }

    private float chargeTimer;

    [SerializeField] private bool hasSfx;
    [SerializeField, ConditionalHide("hasSfx")] private AudioClips chargeUpSfx;
    [SerializeField, ConditionalHide("hasSfx")] private AudioClips launchSfx;
    private GameObject chargeUpAudioSource;

    private void Awake() {
        hasEnemyStats = GetComponent<IHasEnemyStats>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        CurrentState = ChargeState.OnCooldown;
        chargeTimer = 0;
    }

    private void OnDisable() {
        if (chargeUpAudioSource != null) {
            chargeUpAudioSource.ReturnToPool();
            chargeUpAudioSource = null;
        }
    }

    private void Update() {

        if (CurrentState == ChargeState.OnCooldown) {
            chargeTimer += Time.deltaTime;

            if (chargeTimer > hasEnemyStats.EnemyStats.AttackCooldown) {
                chargeTimer = 0;
                CurrentState = ChargeState.ChargingUp;

                anim.SetTrigger("startCharging");

                if (hasSfx) {
                    chargeUpAudioSource = AudioManager.Instance.PlaySingleSound(chargeUpSfx);
                }
            }
        }

        if (CurrentState == ChargeState.Launching) {
            if (rb.velocity == Vector2.zero) {
                CurrentState = ChargeState.OnCooldown;
                anim.SetTrigger("stopCharging");
            }
        }
    }

    private void FixedUpdate() {
        if (CurrentState == ChargeState.Launching) {
            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, Time.fixedDeltaTime * deceleration);

            
        }
    }

    // played by AnimationMethodInvoker
    public void Charge() {
        CurrentState = ChargeState.Launching;

        Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
        rb.velocity = toPlayerDirection * initialSpeed;

        if (hasSfx) {
            AudioManager.Instance.PlaySingleSound(launchSfx);
            chargeUpAudioSource = null;
        }

        OnCharge?.Invoke(true);
    }

    public void Charge(Vector2 direction) {
        CurrentState = ChargeState.Launching;

        rb.velocity = direction.normalized * initialSpeed;

        if (hasSfx) {
            AudioManager.Instance.PlaySingleSound(launchSfx);
            chargeUpAudioSource = null;
        }

        OnCharge?.Invoke(false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (GameLayers.ObstacleLayerMask.ContainsLayer(collision.gameObject.layer)) {
            rb.velocity = Vector2.zero;
        }
    }
}

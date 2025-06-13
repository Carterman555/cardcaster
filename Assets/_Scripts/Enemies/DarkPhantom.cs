using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DarkPhantom : Enemy {

    private ChasePlayerBehavior moveBehavior;
    private StraightShootBehavior shootBehavior;

    [Header("Teleport")]
    private RandomTeleportBehavior teleportBehavior;
    [SerializeField] private float teleportDistanceFromPlayer = 3f;
    [SerializeField] private RandomFloat nearPlayerTeleportTime;
    private float nearPlayerTeleportTimer;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();
        teleportBehavior = GetComponent<RandomTeleportBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();
        shootBehavior.OnShootAnim += StopMoving;

        moveBehavior.enabled = true;
        
        nearPlayerTeleportTime.Randomize();
    }

    protected override void OnDisable() {
        base.OnDisable();
        shootBehavior.OnShootAnim -= StopMoving;
    }

    protected override void Update() {
        base.Update();

        if (TryGetComponent(out StopMovement stopMovement)) {
            return;
        }

        if (playerWithinRange) {
            nearPlayerTeleportTimer += Time.deltaTime;
            if (nearPlayerTeleportTimer > nearPlayerTeleportTime.Value) {
                nearPlayerTeleportTimer = 0;
                nearPlayerTeleportTime.Randomize();

                teleportBehavior.VisualTeleport(PlayerMovement.Instance.CenterPos, teleportDistanceFromPlayer);
            }
        }
        else {
            nearPlayerTeleportTimer = 0;
        }
    }

    // stop moving when shooting
    private void StopMoving() => StartCoroutine(StopMovingCor());
    private IEnumerator StopMovingCor() {

        moveBehavior.enabled = false;

        float stopDuration = 0.4f;
        yield return new WaitForSeconds(stopDuration);

        moveBehavior.enabled = true;
    }
}

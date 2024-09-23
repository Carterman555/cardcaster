using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class DarkPhantom : Enemy {
    private ChasePlayerBehavior moveBehavior;

    [Header("Attack")]
    private StraightShootBehavior shootBehavior;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private Transform shootPoint;

    [Header("Teleport")]
    private RandomTeleportBehavior teleportBehavior;
    [SerializeField] private float teleportDistanceFromPlayer = 3f;
    [SerializeField] private RandomFloat nearPlayerTeleportTime;
    private float nearPlayerTeleportTimer;

    [SerializeField] private SpriteRenderer visual;
    private float originalFade;

    protected override void Awake() {
        base.Awake();

        InitializeBehaviors();

        originalFade = visual.color.a;
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.Start();

        nearPlayerTeleportTime.Randomize();

        visual.Fade(originalFade);

        shootBehavior.OnShootAnim += StopMoving;
    }

    protected override void OnDisable() {
        base.OnDisable();

        shootBehavior.OnShootAnim -= StopMoving;
    }

    private void InitializeBehaviors() {
        moveBehavior = new(this);
        enemyBehaviors.Add(moveBehavior);

        shootBehavior = new(this, projectilePrefab, shootPoint);
        enemyBehaviors.Add(shootBehavior);
        shootBehavior.StartShooting(PlayerMovement.Instance.transform);

        PolygonCollider2D teleportBounds = Room.GetCurrentRoom().GetComponent<PolygonCollider2D>();
        teleportBehavior = new(gameObject, this, teleportBounds);
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

                Teleport();
            }
        }
        else {
            nearPlayerTeleportTimer = 0;
        }
    }

    private void Teleport() {

        float duration = 0.3f;
        visual.DOFade(0, duration).SetEase(Ease.InSine).OnComplete(() => {
            teleportBehavior.Teleport(PlayerMovement.Instance.transform.position, teleportDistanceFromPlayer);

            visual.DOFade(originalFade, duration).SetEase(Ease.InSine);
        });

    }

    // stop moving when shooting
    private void StopMoving() => StartCoroutine(StopMovingCor());
    private IEnumerator StopMovingCor() {

        print("stop");

        moveBehavior.Stop();

        float stopDuration = 0.4f;
        yield return new WaitForSeconds(stopDuration);

        moveBehavior.Start();
    }
}

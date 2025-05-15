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

    [Header("Fade")]
    [SerializeField] private SpriteRenderer visual;
    private float originalFade;

    protected override void Awake() {
        base.Awake();

        moveBehavior = GetComponent<ChasePlayerBehavior>();
        shootBehavior = GetComponent<StraightShootBehavior>();
        teleportBehavior = GetComponent<RandomTeleportBehavior>();

        originalFade = visual.color.a;
    }

    protected override void OnEnable() {
        base.OnEnable();

        moveBehavior.enabled = true;
        visual.Fade(originalFade);

        nearPlayerTeleportTime.Randomize();

        shootBehavior.OnShootAnim += StopMoving;
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

        if (PlayerWithinRange) {
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);

        float duration = 0.3f;
        visual.DOFade(0, duration).SetEase(Ease.InSine).OnComplete(() => {
            teleportBehavior.Teleport(PlayerMovement.Instance.CenterPos, teleportDistanceFromPlayer);

            visual.DOFade(originalFade, duration).SetEase(Ease.InSine);
        });
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

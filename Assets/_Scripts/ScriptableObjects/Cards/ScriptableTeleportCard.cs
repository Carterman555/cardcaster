using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// this needs to check if the position the player is trying to teleport to is in the room and not in a collider.
/// if one of those is the case, then it needs to send a raycast from the mouse position to the player position until
/// there is an open spot to teleport. Then teleport there
/// </summary>
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Teleport Card")]
public class ScriptableTeleportCard : ScriptableAbilityCardBase {
    public float raycastStep = 0.1f;
    public LayerMask obstacleLayer;

    [SerializeField] private Transform teleportPosVisualPrefab;
    private Transform teleportPosVisual;

    public override void OnStartDraggingCard(Transform cardTransform) {
        base.OnStartDraggingCard(cardTransform);

        teleportPosVisual = teleportPosVisualPrefab.Spawn(Containers.Instance.Effects);
    }

    protected override void DraggingUpdate(Vector2 cardposition) {
        base.DraggingUpdate(cardposition);

        teleportPosVisual.position = RaycastToFindPosition(cardposition);
    }

    protected override void Play(Vector2 position) {
        base.Play(position);

        teleportPosVisual.gameObject.ReturnToPool();

        // fade out instantly, then fade in with delay
        float duration = 0.5f;
        FadeEffect fadeEffect = PlayerVisual.Instance.AddFadeEffect(0, 0f);
        PlayerVisual.Instance.RemoveFadeEffect(fadeEffect, duration);

        CreateVisualClone(PlayerMovement.Instance.transform.position);

        if (IsValidTeleportPos(position)) {
            TeleportPlayer(position);
        }
        else {
            Vector2 validPosition = RaycastToFindPosition(position);
            TeleportPlayer(validPosition);
        }

        base.Stop();
    }

    private Vector2 RaycastToFindPosition(Vector2 targetPosition) {
        Vector2 playerPosition = PlayerMovement.Instance.transform.position;
        Vector2 toPlayerDirection = (playerPosition - targetPosition).normalized;

        Vector2 checkPos = targetPosition;

        int emergencyCounter = 0;
        while (Vector2.Distance(checkPos, playerPosition) > raycastStep) {
            emergencyCounter++;
            if (emergencyCounter > 500) {
                Debug.LogError("Teleport Emergency Break!");
                return playerPosition;
            }

            checkPos += toPlayerDirection * raycastStep;
            if (IsValidTeleportPos(checkPos)) {
                return checkPos;
            }
        }

        // If no valid point is found, return the player's current position
        return playerPosition;
    }

    public bool IsValidTeleportPos(Vector2 pos) {
        bool validPos = new RoomPositionHelper()
            .SetObstacleAvoidance(1f)
            .IsValidPosition(pos);

        return validPos;
    }

    private void TeleportPlayer(Vector2 position) {
        PlayerMovement.Instance.transform.position = position;
    }

    [SerializeField] private SpriteRenderer visualClonePrefab;

    private void CreateVisualClone(Vector3 clonePosition) {
        SpriteRenderer visualClone = visualClonePrefab.Spawn(clonePosition, Containers.Instance.Effects);

        //... face the same way as the player
        visualClone.transform.eulerAngles = PlayerMovement.Instance.transform.eulerAngles;

        // fade out
        visualClone.Fade(1f);
        float duration = 0.5f;
        visualClone.DOFade(0f, duration).OnComplete(() => {
            visualClone.gameObject.ReturnToPool();
        });
    }
}
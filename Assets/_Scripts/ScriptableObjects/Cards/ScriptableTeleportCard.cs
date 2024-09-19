using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this needs to check if the position the player is trying to teleport to is in the room and not in a collider.
/// if one of those is the case, then it needs to send a raycast from the mouse position to the player position until
/// there is an open spot to teleport. Then teleport there
/// </summary>
[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Teleport Card")]
public class ScriptableTeleportCard : ScriptableCardBase {
    public float raycastStep = 0.1f;
    public LayerMask obstacleLayer;

    public override void Play(Vector2 position) {
        base.Play(position);

        float duration = 0.5f;
        PlayerVisual.Instance.SetFadeEffect
        PlayerVisual.Instance.DOFade(1f, duration);
        CreateVisualClone(PlayerMovement.Instance.transform.position);

        if (IsPointValidForTeleport(position)) {
            TeleportPlayer(position);
        }
        else {
            Vector2 validPosition = RaycastToFindPosition(position);
            TeleportPlayer(validPosition);
        }
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
            if (IsPointValidForTeleport(checkPos)) {
                return checkPos;
            }
        }

        // If no valid point is found, return the player's current position
        return playerPosition;
    }

    private bool IsPointValidForTeleport(Vector2 point) {
        float obstacleAvoidanceRadius = 1f;
        return IsPointInRoom(point) && !Physics2D.OverlapCircle(point, obstacleAvoidanceRadius, obstacleLayer);
    }

    private bool IsPointInRoom(Vector2 point) {
        return Room.GetCurrentRoom().GetComponent<PolygonCollider2D>().OverlapPoint(point);
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
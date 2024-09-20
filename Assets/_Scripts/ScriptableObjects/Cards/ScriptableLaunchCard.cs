using DG.Tweening;
using Mono.CSharp;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// this needs to check if the position the player is trying to teleport to is in the room and not in a collider.
/// if one of those is the case, then it needs to send a raycast from the mouse position to the player position until
/// there is an open spot to teleport. Then teleport there
/// </summary>
[CreateAssetMenu(fileName = "LaunchCard", menuName = "Cards/Launch Card")]
public class ScriptableLaunchCard : ScriptableCardBase {
    public float raycastStep = 0.1f;
    public LayerMask obstacleLayer;

    [SerializeField] private float pathWidth = 3;
    [SerializeField] private Transform pathVisualPrefab;
    private Transform pathVisual;

    public override void OnStartDraggingCard() {
        base.OnStartDraggingCard();

        pathVisual = pathVisualPrefab.Spawn(PlayerMovement.Instance.transform.position, PlayerVisual.Instance.transform);
    }

    protected override void DraggingUpdate() {
        base.DraggingUpdate();

        Vector2 toMouseDirection = MouseTracker.Instance.ToMouseDirection(pathVisual.transform.position);

        //... point path towards mouse
        pathVisual.up = toMouseDirection;

        // scale path towards end of room
        float checkDistance = 100f;
        RaycastHit2D hit = Physics2D.BoxCast(pathVisual.position, new Vector2(pathWidth, 1f), pathVisual.eulerAngles.z, toMouseDirection, checkDistance, obstacleLayer);

        if (hit.collider == null) {
            Debug.LogError("Could Not Find Wall!");
        }
        else {
            pathVisual.localScale = new Vector3(pathWidth, hit.distance);
        }
    }

    public override void Play(Vector2 position) {
        base.Play(position);

        pathVisual.gameObject.ReturnToPool();
    }
}
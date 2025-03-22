using DG.Tweening;
using MoreMountains.Tools;
using System;
using UnityEngine;
using UnityEngine.AI;

public class CircleMoveBehavior : MonoBehaviour, IChangesFacing, IEnemyMovement {

    [SerializeField] private float moveRadius;

    private float angle;
    private Vector2 center;

    private IHasEnemyStats hasStats;
    private NavMeshAgent agent;
    private Knockback knockback;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        knockback = GetComponent<Knockback>();
    }

    private void OnEnable() {
        angle = 0f;

        agent.isStopped = false;

        center = transform.position;

        // face right
        facingRight = true;
        transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
        OnChangedFacing?.Invoke(facingRight);
    }

    private void OnDisable() {
        // if disabled by enemy script, not from dying
        if (!GetComponent<Health>().Dead && !Helpers.GameStopping()) {
            agent.isStopped = true;
        }
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        agent.speed = hasStats.EnemyStats.MoveSpeed;

        float mult = 1 / moveRadius;
        angle += hasStats.EnemyStats.MoveSpeed * mult * Time.deltaTime; // Increment angle based on speed
        float x = center.x + moveRadius * Mathf.Cos(angle);
        float y = center.y + moveRadius * Mathf.Sin(angle);
        Vector3 nextPosition = new Vector3(x, y);
        agent.SetDestination(nextPosition);

        bool faceRight = nextPosition.x > transform.position.x;
        HandleDirectionFacing(faceRight);
    }

    public event Action<bool> OnChangedFacing;

    private bool facingRight;

    // TODO - refactor cause duplicate code as FacePlayerBehaviour
    private void HandleDirectionFacing(bool faceRight) {
        if (!facingRight && faceRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
            facingRight = true;
            OnChangedFacing?.Invoke(facingRight);
        }
        else if (facingRight && !faceRight) {
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
            facingRight = false;
            OnChangedFacing?.Invoke(facingRight);
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled;
    }
}

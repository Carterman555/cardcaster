using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Knockback))]
public class BounceMoveBehaviour : MonoBehaviour, IEffectable, IEnemyMovement {

    public event Action OnBounce;

    private Rigidbody2D rb;
    private Knockback knockback;
    private IHasEnemyStats hasStats;

    private Vector2 velocity;

    [SerializeField] private TriggerContactTracker bounceTrigger;

    [SerializeField] private Transform centerPoint;

    [SerializeField] private bool hasBounceVariation;
    [SerializeField, ConditionalHide("hasBounceVariation")] private float bounceVariation;

    [SerializeField] private bool twoWayFacing = true;
    [SerializeField, ConditionalHideReversed("twoWayFacing")] private float facingAngleOffset;
    
    [SerializeField] private bool hasFacingTarget;
    [SerializeField, ConditionalHide("hasFacingTarget")] private Transform facingTarget;
    private Transform FacingTransform => hasFacingTarget ? facingTarget : transform;

    [SerializeField] private bool hasMoveSFX;
    [ConditionalHide("hasMoveSFX")][SerializeField] private AudioClips moveSFX;

    [SerializeField] private bool hasBounceSFX;
    [ConditionalHide("hasBounceSFX")] [SerializeField] private AudioClips bounceSFX;

    private Coroutine moveSFXCoroutine;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<Knockback>();
        hasStats = GetComponent<IHasEnemyStats>();
    }

    private void OnEnable() {
        RandomizeDirection();

        if (hasMoveSFX) {
            moveSFXCoroutine = StartCoroutine(MoveSFX());
        }
    }
    private void OnDisable() {
        // reset direction facing
        if (!twoWayFacing) {
            FacingTransform.rotation = Quaternion.identity;
        }

        rb.velocity = Vector2.zero;

        if (moveSFXCoroutine != null) {
            StopCoroutine(moveSFXCoroutine);
        }
    }

    private void RandomizeDirection() {
        velocity = Vector2.up * hasStats.EnemyStats.MoveSpeed;

        float randomDegrees = UnityEngine.Random.Range(0f, 360f);
        velocity.RotateDirection(randomDegrees);

        UpdateFacing(velocity);
    }

    public void SetDirection(Vector2 direction) {
        velocity = direction * hasStats.EnemyStats.MoveSpeed;
        UpdateFacing(velocity);
    }

    private void Update() {

        if (!IsMoving()) {
            return;
        }

        if (bounceTrigger.HasContact()) {
            Collider2D contactCollider = bounceTrigger.GetFirstContact().GetComponent<Collider2D>();
            Vector2 direction = contactCollider.ClosestPoint(centerPoint.position) - (Vector2)centerPoint.position;
            Bounce(direction);
        }

        rb.velocity = velocity;
    }

    public void Bounce(Vector2 direction) {

        // Calculate the reflection vector
        Vector2 reflectDir = Vector2.Reflect(velocity, direction.normalized); // Reflect based on current velocity

        // A safe guard because sometimes the bounce glitches and enemies bounce twice at the same time, causing the
        // enemy to bounce back into the wall.
        bool verticalBounce = Mathf.Abs(direction.y) > Mathf.Abs(direction.x);
        if (verticalBounce) {
            bool bounceIntoWall = Mathf.Sign(direction.y) == Mathf.Sign(reflectDir.y);
            if (bounceIntoWall) {
                return;
            }
        }
        else {
            bool bounceIntoWall = Mathf.Sign(direction.x) == Mathf.Sign(reflectDir.x);
            if (bounceIntoWall) {
                return;
            }
        }

        // Set the new velocity
        velocity = reflectDir.normalized * velocity.magnitude; // Preserve the speed

        UpdateFacing(velocity);

        if (hasBounceSFX) {
            AudioManager.Instance.PlaySingleSound(bounceSFX);
        }

        OnBounce?.Invoke();
    }

    public void ForceBounce(Vector2 direction) {
        // Calculate the reflection vector
        Vector2 reflectDir = Vector2.Reflect(velocity, direction.normalized); // Reflect based on current velocity

        // Set the new velocity
        velocity = reflectDir.normalized * velocity.magnitude; // Preserve the speed

        UpdateFacing(velocity);

        if (hasBounceSFX) {
            AudioManager.Instance.PlaySingleSound(bounceSFX);
        }

        OnBounce?.Invoke();
    }

    private void UpdateFacing(Vector2 velocity) {

        if (twoWayFacing) {
            // if going right
            if (velocity.x > 0f) {
                FaceRight();
            }
            // if going left
            else if (velocity.x < 0f) {
                FaceLeft();
            }
        }
        else {
            Vector2 faceDirection = velocity.normalized;
            faceDirection.RotateDirection(facingAngleOffset);
            FacingTransform.up = faceDirection;
        }
    }

    private void FaceRight() {
        FacingTransform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z));
    }

    private void FaceLeft() {
        FacingTransform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z));
    }

    private IEnumerator MoveSFX() {
        while (true) {
            yield return new WaitForSeconds(0.5f + UnityEngine.Random.Range(-0.05f, 0.05f));
            AudioManager.Instance.PlaySingleSound(moveSFX);
        }
    }

    public void OnAddEffect(UnitEffect unitEffect) {
        if (unitEffect is StopMovement) {
            rb.velocity = Vector2.zero;
        }
    }

    public bool IsMoving() {
        bool hasStopEffect = TryGetComponent(out StopMovement stopMovement);
        return !hasStopEffect && !knockback.IsApplyingKnockback() && enabled;
    }
}

using UnityEngine;

public class PlayerDashAttack : MonoBehaviour {

    private PlayerMovement playerMovement;
    private PlayerMeleeAttack playerMeleeAttack;

    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private Vector2 attackSize;
    [SerializeField] private float windowToAttack = 0.5f;
    private float dashTimer;

    private Vector2 dashStartPos;
    private Vector2 dashDirection;

    [SerializeField] private Transform slashPrefab;

    private PlayerStats Stats => StatsManager.PlayerStats;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerMeleeAttack = GetComponent<PlayerMeleeAttack>();
    }

    private void OnEnable() {
        playerMovement.OnDash_Direction += OnStartDash;
    }

    private void OnDisable() {
        playerMovement.OnDash_Direction -= OnStartDash;
    }

    private void OnStartDash(Vector2 dashDirection) {
        dashStartPos = transform.position;
        this.dashDirection = dashDirection;
        dashTimer = windowToAttack;
    }

    public Collider2D[] DashAttack() {

        Vector2 attackDirection = playerMeleeAttack.GetAttackDirection();

        float angle = attackDirection.DirectionToRotation().eulerAngles.z;
        Vector2 pos = (Vector2)playerMovement.CenterPos + (playerMeleeAttack.GetAttackDirection() * attackSize.x * 0.5f);

        Collider2D[] cols = Physics2D.OverlapCapsuleAll(pos, attackSize, CapsuleDirection2D.Horizontal, angle, targetLayerMask);

        foreach (Collider2D col in cols) {
            if (col.TryGetComponent(out DropEssenceOnDeath dropEssenceOnDeath)) {
                dropEssenceOnDeath.DropMult = ScriptableEssenceHarvestCard.TotalDropMult;
            }
        }

        DamageDealer.DealCapsuleDamage(
            targetLayerMask,
            playerMovement.CenterPos,
            pos, attackSize, angle,
            Stats.DashAttackDamage, Stats.KnockbackStrength, canCrit: true);


        slashPrefab.Spawn(playerMovement.CenterPos, attackDirection.DirectionToRotation(), Containers.Instance.Effects);

        return cols;
    }

    // has to wait until almost done dashing though also (that logic is handled in player melee attack)
    public bool CanDashAttack(Vector2 attackDirection) {
        bool completedOverHalfOfDash = Vector2.Distance(transform.position, dashStartPos) > Stats.DashDistance * 0.5f;
        bool withinDashWindow = dashTimer > 0;
        bool directionsMatch = DirectionsWithinRange(dashDirection, attackDirection, maxAngle: 60f);
        return completedOverHalfOfDash && withinDashWindow && directionsMatch;
    }

    private bool DirectionsWithinRange(Vector3 dir1, Vector3 dir2, float maxAngle) {
        dir1.Normalize();
        dir2.Normalize();

        float dot = Vector3.Dot(dir1, dir2);
        float threshold = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        return dot >= threshold;
    }

    private void Update() {
        dashTimer -= Time.deltaTime;
    }
}

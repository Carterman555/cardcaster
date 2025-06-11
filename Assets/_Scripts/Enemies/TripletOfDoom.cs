using DG.Tweening;
using MoreMountains.Tools;
using UnityEngine;

public class TripletOfDoom : Enemy {

    private WanderMovementBehavior wanderMovement;
    private ChasePlayerBehavior chasePlayerMovement;

    [SerializeField] private float distanceToFlee;

    protected override void Awake() {
        base.Awake();

        wanderMovement = GetComponent<WanderMovementBehavior>();
        chasePlayerMovement = GetComponent<ChasePlayerBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        shootTimer = 0;
    }

    protected override void Update() {
        base.Update();

        HandleMovement();
        HandleAttack();
    }

    #region Movement

    protected override void OnPlayerEnteredRange(Collider2D playerCol) {
        base.OnPlayerEnteredRange(playerCol);

        Wander();
    }

    protected override void OnPlayerExitedRange(Collider2D playerCol) {
        base.OnPlayerExitedRange(playerCol);

        if (!health.Dead) {
            ChasePlayer();
        }
    }


    private void HandleMovement() {
        if (playerWithinRange) {
            float playerDistanceSquared = Vector2.SqrMagnitude(PlayerMovement.Instance.CenterPos - transform.position);
            float distanceToFleeSquared = distanceToFlee * distanceToFlee;

            bool playerWithinFleeDistance = playerDistanceSquared < distanceToFleeSquared;
            wanderMovement.FleePlayer = playerWithinFleeDistance;
        }
    }

    private void Wander() {
        wanderMovement.enabled = true;
        chasePlayerMovement.enabled = false;
    }

    private void ChasePlayer() {
        wanderMovement.enabled = false;
        chasePlayerMovement.enabled = true;
    }

    #endregion

    #region Shoot Exploding Skulls

    [Header("Shoot Exploding Skulls")]
    [SerializeField] private HeatSeekMovement skullsProjectilePrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private AudioClips shootSfx;

    private float shootTimer;

    private void HandleAttack() {

        shootTimer += Time.deltaTime;
        if (shootTimer > GetEnemyStats().AttackCooldown) {
            shootTimer = 0;

            anim.SetTrigger("attack");

            AudioManager.Instance.PlaySound(shootSfx);
        }
    }

    // played by AnimationMethodInvoker
    public void ShootSkulls() {
        HeatSeekMovement projectile = skullsProjectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);
        projectile.Setup(PlayerMovement.Instance.CenterTransform);

        ExplosionTarget playerExplosionTarget = new() {
            LayerMask = GameLayers.PlayerLayerMask,
            ExplosionRadius = 3f,
            Damage = GetEnemyStats().Damage,
            KnockbackStrength = GetEnemyStats().KnockbackStrength
        };

        ExplodeBehavior[] explodeBehaviors = projectile.GetComponentsInChildren<ExplodeBehavior>();
        foreach (ExplodeBehavior explodeBehavior in explodeBehaviors) {
            explodeBehavior.AddedExplosionTargets.Add(playerExplosionTarget);
        }

        // so doesn't explode itself
        ExplodeOnContact[] explodeOnContacts = projectile.GetComponentsInChildren<ExplodeOnContact>();
        foreach (ExplodeOnContact explodeOnContact in explodeOnContacts) {
            explodeOnContact.ExcludedObject = gameObject;
        }

        // so all explode projectiles spawn close together then spread out
        MMAutoRotate[] rotators = projectile.GetComponentsInChildren<MMAutoRotate>();
        foreach (MMAutoRotate rotator in rotators) {
            rotator.OrbitRadius = 0f;

            float orbitRadius = 0.7f;
            DOTween.To(() => rotator.OrbitRadius, r => rotator.OrbitRadius = r, orbitRadius, duration: 0.3f);
        }
    }

    #endregion
}

using UnityEngine;

public class SpiralShootBehaviour : BaseShootPatternBehaviour {

    [SerializeField] protected StraightMovement projectilePrefab;

    [SerializeField] private int shootDirectionAmount = 2;
    [SerializeField] private float spiralSpeed = 20f;
    [SerializeField] private float distanceFromShootPoint;

    private TimedActionBehavior timedActionBehavior;

    private Vector2 shootDirection;

    protected override void Awake() {
        base.Awake();

        timedActionBehavior = new TimedActionBehavior(
            hasStats.GetEnemyStats().AttackCooldown,
            () => ShootMultipleProjectiles());
    }

    private void OnEnable() {
        timedActionBehavior.Start();

        //... set to random direction
        shootDirection = Random.insideUnitCircle.normalized;
    }
    private void OnDisable() {
        timedActionBehavior.Stop();
    }

    private void Update() {
        timedActionBehavior.UpdateLogic();

        // rotate shoot direction to make projectiles shoot out in a spiral
        shootDirection.RotateDirection(spiralSpeed * Time.deltaTime);
    }

    private Vector2 currentShootDirection;

    private void ShootMultipleProjectiles() {
        currentShootDirection = shootDirection;
        float angleBetweenProjectiles = 360f / shootDirectionAmount;

        for (int bulletIndex = 0; bulletIndex < shootDirectionAmount; bulletIndex++) {
            currentShootDirection.RotateDirection(angleBetweenProjectiles);
            ShootProjectile();
        }
    }

    public override void ShootProjectile() {

        Vector3 spawnPos = shootPoint.position + (Vector3)currentShootDirection * distanceFromShootPoint;
        StraightMovement projectile = projectilePrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
        projectile.GetComponent<DamageOnContact>().Setup(Damage, hasStats.GetEnemyStats().KnockbackStrength);

        projectile.Setup(currentShootDirection);

        base.ShootProjectile();
    }

}

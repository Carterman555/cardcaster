using UnityEngine;

public class StraightShooter : MonoBehaviour {

    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private Transform shootFromPoint;

    [SerializeField] private float shootCooldown;
    private float shootTimer;

    [SerializeField] private bool useOverrideSpeed;
    [ConditionalHide("useOverrideSpeed")][SerializeField] private float overrideSpeedValue;

    [SerializeField] private float damage;
    [SerializeField] private float knockbackStrength;

    private void Update() {
        shootTimer += Time.deltaTime;
        if (shootTimer > shootCooldown) {
            StraightMovement projectile = projectilePrefab.Spawn(projectileSpawnPoint.position, Containers.Instance.Projectiles);

            Vector2 direction = (projectileSpawnPoint.position - shootFromPoint.position).normalized;

            if (useOverrideSpeed) {
                projectile.Setup(direction, overrideSpeedValue);
            }
            else {
                projectile.Setup(direction);
            }

            projectile.GetComponent<DamageOnContact>().Setup(damage, knockbackStrength);

            AudioManager.Instance.PlaySingleSound(AudioManager.Instance.AudioClips.BasicEnemyShoot);

            shootTimer = 0;
        }
    }
}

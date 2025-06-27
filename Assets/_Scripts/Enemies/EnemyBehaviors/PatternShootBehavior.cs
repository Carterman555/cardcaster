using DG.Tweening;
using UnityEngine;

public class PatternShootBehavior : MonoBehaviour {

    private IHasEnemyStats hasStats;
    private float attackTimer;

    [SerializeField] private Animator anim;

    [SerializeField] private Transform shootPoint;
    [SerializeField] private StraightMovement projectilePrefab;
    [SerializeField] private ScriptableShootPattern[] shootPatterns;

    [SerializeField] private float projectileSpeed = 8f;

    [SerializeField] private bool hasSfx;
    [SerializeField, ConditionalHide("hasSfx")] private AudioClips shootSfx;

    private void Awake() {
        hasStats = GetComponent<IHasEnemyStats>();
    }

    private void OnEnable() {
        attackTimer = 0;
    }

    private void Update() {

        attackTimer += Time.deltaTime;
        if (attackTimer > hasStats.GetEnemyStats().AttackCooldown) {
            attackTimer = 0;

            anim.SetTrigger("attack");
        }
    }

    // played by anim
    public void Shoot() {
        
        Vector2[] shootPositions = shootPatterns.RandomItem().Positions;
        foreach (Vector2 shootPosition in shootPositions) {
            StraightMovement projectile = projectilePrefab.Spawn(shootPoint.position, Containers.Instance.Projectiles);

            float spreadDistance = 3f;
            float spreadDuration = 0.5f;

            Vector2 spreadPosition = (Vector2)shootPoint.position + shootPosition;
            Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - shootPoint.position).normalized;
            Vector2 targetSpreadPosition = spreadPosition + (toPlayerDirection * spreadDistance);

            projectile.transform.up = toPlayerDirection;
            projectile.GetComponent<Rigidbody2D>().DOMove(targetSpreadPosition, spreadDuration).SetEase(Ease.Linear).OnComplete(() => {
                projectile.Setup(toPlayerDirection, projectileSpeed);
            });

            EnemyStats stats = hasStats.GetEnemyStats();
            projectile.GetComponent<DamageOnContact>().Setup(stats.Damage, stats.KnockbackStrength);
        }

        if (hasSfx) {
            AudioManager.Instance.PlaySound(shootSfx);
        }
    }
}

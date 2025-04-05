using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class Bomb : MonoBehaviour, IAbilityStatsSetup {

    private ExplodeBehavior explodeBehavior;

    [SerializeField] private Animator anim;

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    public void SetAbilityStats(AbilityStats stats) {
        explodeBehavior.SetDamage(stats.Damage);
        explodeBehavior.SetExplosionRadius(stats.AreaSize);
        explodeBehavior.SetKnockbackStrength(stats.KnockbackStrength);

        float litAnimationDuration = 0.8f;
        float animSpeed = litAnimationDuration / stats.Cooldown;
        anim.speed = animSpeed;
    }

    public void ExplodeBomb() {
        explodeBehavior.Explode();
    }
}

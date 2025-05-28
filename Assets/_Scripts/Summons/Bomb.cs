using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ExplodeBehavior))]
public class Bomb : MonoBehaviour, IAbilityStatsSetup {

    private ExplodeBehavior explodeBehavior;

    [SerializeField] private Animator anim;

    [SerializeField] private ParticleSystem explodeParticles;
    [SerializeField] private ParticleSystem explodeGlowParticles;

    // spawn new particles so doesn't get disabled when bomb does
    private ParticleSystem spawnedExplodeParticles;

    private float areaSize;

    private void Awake() {
        explodeBehavior = GetComponent<ExplodeBehavior>();
    }

    public void SetAbilityStats(AbilityStats stats) {
        areaSize = stats.AreaSize;

        ExplosionTarget explosionTarget = new ExplosionTarget() {
            LayerMask = GameLayers.PlayerTargetLayerMask,
            ExplosionRadius = stats.AreaSize,
            Damage = stats.Damage,
            KnockbackStrength = stats.KnockbackStrength,
        };

        explodeBehavior.AddedExplosionTargets.Add(explosionTarget);

        float litAnimationDuration = 0.8f;
        float animSpeed = litAnimationDuration / stats.Cooldown;
        anim.speed = animSpeed;
    }

    public void ExplodeBomb() {
        explodeBehavior.Explode();

        spawnedExplodeParticles = explodeParticles.Spawn(transform.position, Containers.Instance.Effects);
        Transform[] explodeParticlesChildren = spawnedExplodeParticles.GetComponentsInChildren<Transform>(true);
        Transform spawnedGlowTransform = explodeParticlesChildren.FirstOrDefault(t => t.name == explodeGlowParticles.name);
        ParticleSystem spawnGlowEffect = spawnedGlowTransform.GetComponent<ParticleSystem>();

        // the bigger the explosion, the bigger the glow particle effect
        var glowMain = spawnGlowEffect.main;
        float sizeMult = 2.5f;
        glowMain.startSize = areaSize * sizeMult;

        spawnedExplodeParticles.Play();

        gameObject.ReturnToPool();
    }
}

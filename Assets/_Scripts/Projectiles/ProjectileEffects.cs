using UnityEngine;

public class ProjectileEffects : MonoBehaviour {
    [SerializeField] private bool hasSpawnParticles;
    [ConditionalHide("hasSpawnParticles")]
    [SerializeField] private ParticleSystem spawnParticles;
    [ConditionalHide("hasSpawnParticles")]
    [SerializeField] private Color spawnParticlesColor;

    [SerializeField] private bool hasHitParticles;
    [ConditionalHide("hasHitParticles")]
    [SerializeField] private ParticleSystem hitTargetParticles;
    [ConditionalHide("hasHitParticles")]
    [SerializeField] private Color hitTargetParticlesColor;

    [SerializeField] private bool hasDestroyParticles;
    [ConditionalHide("hasDestroyParticles")]
    [SerializeField] private ParticleSystem destroyParticles;
    [ConditionalHide("hasDestroyParticles")]
    [SerializeField] private Color destroyParticlesColor;

    private void OnEnable() {
        if (hasHitParticles && TryGetComponent(out IAttacker attacker)) {
            attacker.OnAttack += CreateHitTargetParticles;
        }
        if (hasSpawnParticles) {
            CreateParticles(spawnParticles, spawnParticlesColor);
        }
    }

    private void CreateHitTargetParticles() {
        if (hasHitParticles) {
            CreateParticles(hitTargetParticles, hitTargetParticlesColor);
        }
    }

    private void OnDisable() {
        if (hasDestroyParticles) {
            CreateParticles(destroyParticles, destroyParticlesColor);
        }
        if (hasHitParticles && TryGetComponent(out IAttacker attacker)) {
            attacker.OnAttack -= CreateHitTargetParticles;
        }
    }

    private void CreateParticles(ParticleSystem particleSystem, Color color) {
        ParticleSystem particles = particleSystem.Spawn(transform.position, Containers.Instance.Effects);
        var main = particles.main;
        main.startColor = color;
    }
}
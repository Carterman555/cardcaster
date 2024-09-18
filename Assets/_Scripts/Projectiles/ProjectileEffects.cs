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
            spawnParticles.CreateColoredParticles(transform.position, spawnParticlesColor);
        }
    }

    private void CreateHitTargetParticles() {
        if (hasHitParticles) {
            hitTargetParticles.CreateColoredParticles(transform.position, hitTargetParticlesColor);
        }
    }

    private bool isQuitting = false;

    private void OnApplicationQuit() {
        isQuitting = true;
    }

    private void OnDisable() {

        // don't create particles if the application is quitting
        if (isQuitting) {
            return;
        }

        if (hasDestroyParticles) {
            destroyParticles.CreateColoredParticles(transform.position, destroyParticlesColor);
        }
        if (hasHitParticles && TryGetComponent(out IAttacker attacker)) {
            attacker.OnAttack -= CreateHitTargetParticles;
        }
    }
}
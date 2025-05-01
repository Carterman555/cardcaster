using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class DeathParticles : MonoBehaviour {

    private EnemyHealth health;

    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private Color deathParticlesColor;

    [SerializeField] private bool hasParticlePoint;
    [ConditionalHide("hasParticlePoint")][SerializeField] private Transform particlePoint;

    [SerializeField] private bool playOnDeathEvent = true;
    [SerializeField] private bool playOnDisable = false;

    [SerializeField] private bool playSFX;
    [ConditionalHide("playSFX")] [SerializeField] private AudioClips deathSFX;

    private void Awake() {
        health = GetComponent<EnemyHealth>();
    }

    private void OnEnable() {
        if (playOnDeathEvent) {
            health.DeathEventTrigger.AddListener(GenerateParticles);
        }
    }

    private void OnDisable() {
        health.DeathEventTrigger.RemoveListener(GenerateParticles);

        if (playOnDisable && !Helpers.GameStopping()) {
            GenerateParticles();
        }
    }

    public void GenerateParticles() {
        Vector2 pos = hasParticlePoint ? particlePoint.position : transform.position;
        deathParticlesPrefab.CreateColoredParticles(pos, deathParticlesColor);

        if (playSFX) {
            AudioManager.Instance.PlaySound(deathSFX);
        }
    }
}

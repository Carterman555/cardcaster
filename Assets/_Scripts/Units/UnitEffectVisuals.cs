using UnityEngine;

public class UnitEffectVisuals : MonoBehaviour {

    private SpriteRenderer visual;

    private void Awake() {
        visual = GetComponent<SpriteRenderer>();
    }

    public ParticleSystem AddParticleEffect(ParticleSystem particleEffectPrefab) {
        ParticleSystem particles = particleEffectPrefab.Spawn(transform);

        float spriteSize = visual.bounds.size.y;
        float spriteSizeMult = 0.1f;
        particles.transform.localScale = Vector3.one * spriteSize * spriteSizeMult;

        return particles;
    }

    public void RemoveParticleEffect(ParticleSystem particlesInstance) {
        particlesInstance.gameObject.ReturnToPool();
    }
}

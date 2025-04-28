using UnityEngine;

public class UnitEffectVisuals : MonoBehaviour {

    [SerializeField] private Collider2D effectAreaCol;

    public ParticleSystem AddParticleEffect(ParticleSystem particleEffectPrefab) {
        ParticleSystem particles = particleEffectPrefab.Spawn(transform);

        if (particles.TryGetComponent(out IVisualEffect visualEffect)) {
            visualEffect.Setup(effectAreaCol);
        }

        return particles;
    }
}

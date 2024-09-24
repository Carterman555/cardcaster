using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffectVisuals : MonoBehaviour {

    private SpriteRenderer visual;
    private Material originalMaterial;

    private void Awake() {
        visual = GetComponent<SpriteRenderer>();
        originalMaterial = visual.material;
    }

    private void OnEnable() {
        visual.material = originalMaterial;
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

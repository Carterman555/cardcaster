using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffectVisuals : MonoBehaviour {

    private SpriteRenderer visual;
    private Material originalMaterial;

    private int currentParticleId = 0;

    private Dictionary<int, ParticleSystem> particleSystemIds = new Dictionary<int, ParticleSystem>();

    private void Awake() {
        visual = GetComponent<SpriteRenderer>();
        originalMaterial = visual.material;
    }

    private void OnEnable() {
        visual.material = originalMaterial;
    }

    public int AddParticleEffect(ParticleSystem particleEffectPrefab) {
        ParticleSystem particles = particleEffectPrefab.Spawn(transform);

        float spriteSize = visual.bounds.size.y;
        float spriteSizeMult = 0.1f;
        particles.transform.localScale = Vector3.one * spriteSize * spriteSizeMult;

        currentParticleId++;
        particleSystemIds.Add(currentParticleId, particles);

        return currentParticleId;
    }

    public void RemoveParticleEffect(int particleId) {

        if (!particleSystemIds.ContainsKey(particleId)) {
            Debug.LogError("ID Dictionary Does Not Contain Key!");
        }

        particleSystemIds[particleId].gameObject.ReturnToPool();
        particleSystemIds.Remove(particleId);
    }
}

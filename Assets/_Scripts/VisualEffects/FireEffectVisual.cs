using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffectVisual : MonoBehaviour, IVisualEffect {

    [SerializeField] private ParticleSystem mainParticles;
    [SerializeField] private ParticleSystem pixel2Particles;
    [SerializeField] private ParticleSystem glowParticles;

    public void Setup(Collider2D effectAreaCol) {

        var mainShape = mainParticles.shape;
        var pixel2Shape = pixel2Particles.shape;

        Vector3 centerBotPos = new Vector3(effectAreaCol.offset.x, effectAreaCol.offset.y - (effectAreaCol.bounds.size.y / 2), mainShape.position.z);
        mainShape.position = centerBotPos;
        pixel2Shape.position = centerBotPos;

        mainShape.length = effectAreaCol.bounds.size.y;
        pixel2Shape.length = effectAreaCol.bounds.size.y * 0.5f;

        int diameterToRadius = 2;
        float radiusMult = 0.5f;
        float radius = (effectAreaCol.bounds.size.x / diameterToRadius) * radiusMult;
        mainShape.radius = radius;
        pixel2Shape.radius = radius;

        var mainEmission = mainParticles.emission;
        var pixel2Emission = pixel2Particles.emission;
        float area = effectAreaCol.bounds.size.x * effectAreaCol.bounds.size.y;
        float rateMult = 4f;
        mainEmission.rateOverTime = area * rateMult;
        pixel2Emission.rateOverTime = area * rateMult;


        glowParticles.transform.position = effectAreaCol.bounds.center;
        
        float longestSize = Mathf.Max(effectAreaCol.bounds.size.x, effectAreaCol.bounds.size.y);
        float scale = longestSize / 2f;
        glowParticles.transform.localScale = new Vector3(scale, scale, 1f);
    }
}

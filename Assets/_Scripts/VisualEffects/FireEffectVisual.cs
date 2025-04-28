using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffectVisual : MonoBehaviour, IVisualEffect {

    [SerializeField] private ParticleSystem mainParticles;
    [SerializeField] private ParticleSystem pixel2Particles;

    public void Setup(Collider2D effectAreaCol) {

        print("Setup");

        var shape = mainParticles.shape;
        shape.position = new Vector3(effectAreaCol.offset.x, effectAreaCol.offset.y - (effectAreaCol.bounds.size.y / 2), shape.position.z);
        shape.length = effectAreaCol.bounds.size.y;

        int diameterToRadius = 2;
        float radiusMult = 0.5f;
        shape.radius = (effectAreaCol.bounds.size.x / diameterToRadius) * radiusMult;

        var emission = mainParticles.emission;
        float area = effectAreaCol.bounds.size.x * effectAreaCol.bounds.size.y;
        float rateMult = 4f;
        emission.rateOverTime = area * rateMult;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public static class EffectsHelper {
    public static void CreateColoredParticles(this ParticleSystem particleSystemPrefab, Vector2 position, Color color) {
        ParticleSystem newParticleSystem = particleSystemPrefab.Spawn(position, Containers.Instance.Effects);

        ParticleSystem[] allParticles = newParticleSystem.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particles in allParticles) {
            var main = particles.main;
            main.startColor = color;
        }
    }
}

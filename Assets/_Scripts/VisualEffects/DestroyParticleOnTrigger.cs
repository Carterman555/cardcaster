using UnityEngine;
using System.Collections.Generic;

public class DestroyParticleOnTrigger : MonoBehaviour {
    private ParticleSystem particles;
    private List<ParticleSystem.Particle> enterParticles = new List<ParticleSystem.Particle>();

    void Start() {
        particles = GetComponent<ParticleSystem>();
    }

    void OnParticleTrigger() {
        // Get particles that have entered the trigger
        int numEnter = particles.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);

        // Iterate over these particles and perform actions
        for (int i = 0; i < numEnter; i++) {
            // Example: Destroy particle by setting its lifetime to zero
            ParticleSystem.Particle p = enterParticles[i];
            p.remainingLifetime = 0;
            enterParticles[i] = p;
        }

        // Apply changes back to the particle system
        particles.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enterParticles);
    }
}
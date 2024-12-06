using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DeathParticles : MonoBehaviour {

    private Health health;

    [SerializeField] private ParticleSystem deathParticlesPrefab;
    [SerializeField] private Color deathParticlesColor;

    [SerializeField] private bool hasParticlePoint;
    [ConditionalHide("hasParticlePoint")][SerializeField] private Transform particlePoint;

    private void Awake() {
        health = GetComponent<Health>();
    }

    private void OnEnable() {
        health.OnDeath += GenerateParticles;
    }

    private void OnDisable() {
        health.OnDeath -= GenerateParticles;
    }

    private void GenerateParticles() {
        Vector2 pos = hasParticlePoint ? particlePoint.position : transform.position;
        deathParticlesPrefab.CreateColoredParticles(pos, deathParticlesColor);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesOnEnable : MonoBehaviour {
    [SerializeField] private ParticleSystem particlesPrefab;

    private void OnEnable() {
        particlesPrefab.Spawn(transform.position, Containers.Instance.Effects);
    }
}

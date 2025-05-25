using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetElectricChargingVisuals : MonoBehaviour {

    [SerializeField] private ParticleSystem electricParticles;
    [SerializeField] private ParticleSystem glowParticles;

    private void OnDisable() {
        // TODO - reset everything that was changed in the start charging animations
        // can reset material in idle animations
    }

}

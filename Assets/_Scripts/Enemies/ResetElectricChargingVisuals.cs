using UnityEngine;

public class ResetElectricChargingVisuals : MonoBehaviour {

    [SerializeField] private ParticleSystem electricParticles;
    [SerializeField] private ParticleSystem glowParticles;

    [SerializeField] private SpriteRenderer visual;

    private void OnDisable() {

        var electricEmissions = electricParticles.emission;
        electricEmissions.rateOverTime = 30;

        var glowMain = glowParticles.main;

        Color transparentWhite = Color.white;
        transparentWhite.a = 0;
        glowMain.startColor = transparentWhite;

        var glowRenderer = glowParticles.GetComponent<ParticleSystemRenderer>();
        glowRenderer.enabled = false;
    }

}

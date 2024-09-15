using UnityEngine;

public class AssetSystem : StaticInstance<AssetSystem> {

    [field: SerializeField] public ParticleSystem UnitFireParticles { get; private set; }
    [field: SerializeField] public ParticleSystem AbilityFireParticles { get; private set; }

    [field: SerializeField] public ParticleSystem UnitElectricityParticles { get; private set; }
}
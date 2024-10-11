using UnityEngine;

public class AssetSystem : StaticInstance<AssetSystem> {

    [field: SerializeField] public DebugText DebugTextPrefab { get; private set; }

    [field: SerializeField] public ParticleSystem UnitFireParticles { get; private set; }

    [field: SerializeField] public ParticleSystem UnitElectricityParticles { get; private set; }
}
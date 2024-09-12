using UnityEngine;

public class AssetSystem : StaticInstance<AssetSystem> {

    [field: SerializeField] public ParticleSystem UnitFireParticles { get; private set; }
}
using UnityEngine;

public class AssetSystem : StaticInstance<AssetSystem>
{
    [field: SerializeField] public GameObject Example { get; private set; }
}
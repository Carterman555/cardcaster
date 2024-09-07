using UnityEngine;

public abstract class ScriptableItemBase : ScriptableObject, ICollectable {

    [SerializeField] private string itemName;
    public string GetName() => itemName;

    [SerializeField] private string description;
    public string GetDescription() => description;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [SerializeField] private Sprite outlineSprite;
    public Sprite GetOutlineSprite() => outlineSprite;

    public abstract void Activate();
    public abstract void Deactivate();
}

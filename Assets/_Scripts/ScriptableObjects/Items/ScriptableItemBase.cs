using UnityEngine;

public abstract class ScriptableItemBase : ScriptableObject, ICollectable {

    [SerializeField] private string itemName;
    public string Name => itemName;

    [SerializeField] private string description;
    public string Description => description;

    [SerializeField] private int cost;
    public int Cost => cost;

    [SerializeField] private Rarity rarity;
    public Rarity Rarity => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    public abstract void Activate();
    public abstract void Deactivate();
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem>
{
    public List<ScriptableEnemy> Enemies { get; private set; }
    public List<ScriptableCardBase> Cards { get; private set; }
    public List<ScriptableItemBase> Items { get; private set; }

    protected override void Awake() {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources() {
        Enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
        Cards = Resources.LoadAll<ScriptableCardBase>("Cards").ToList();
        Items = Resources.LoadAll<ScriptableItemBase>("Items").ToList();
    }

    public List<ScriptableEnemy> GetAllEnemies() => Enemies;
    public List<ScriptableCardBase> GetAllCards() => Cards;
    public List<ScriptableItemBase> GetAllItems() => Items;
}   
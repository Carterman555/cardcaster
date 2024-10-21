using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem>
{
    public List<ScriptableLevelLayout> LevelLayouts { get; private set; }
    public Dictionary<RoomType, ScriptableRoom[]> Rooms { get; private set; }
    public List<ScriptableEnemy> Enemies { get; private set; }
    public Dictionary<Level, ScriptableBoss[]> Bosses { get; private set; }
    public List<ScriptableCardBase> Cards { get; private set; }
    public List<ScriptableItemBase> Items { get; private set; }

    protected override void Awake() {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources() {
        LevelLayouts = Resources.LoadAll<ScriptableLevelLayout>("Layouts").ToList();

        Rooms = Resources.LoadAll<ScriptableRoom>("Rooms")
            .GroupBy(r => r.RoomType)
            .ToDictionary(g => g.Key, g => g.ToArray());

        Enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();

        Bosses = Resources.LoadAll<ScriptableBoss>("Bosses")
            .SelectMany(boss => GetLevels(boss.PossibleLevels)
                .Select(level => new { level, boss })) // Create an anonymous object of level and boss
            .GroupBy(x => x.level)
            .ToDictionary(group => group.Key, group => group.Select(x => x.boss).ToArray());

        Cards = Resources.LoadAll<ScriptableCardBase>("Cards").ToList();
        Items = Resources.LoadAll<ScriptableItemBase>("Items").ToList();
    }

    // Helper method to extract individual levels from the PossibleLevels bit flags
    private IEnumerable<Level> GetLevels(Level possibleLevels) {
        foreach (Level level in Enum.GetValues(typeof(Level))) {
            if (level != Level.None && possibleLevels.HasFlag(level)) {
                yield return level;
            }
        }
    }

    public ScriptableLevelLayout GetRandomLayout() => LevelLayouts.RandomItem();
    public ScriptableRoom[] GetRooms(RoomType roomType) => Rooms[roomType];
    public List<ScriptableEnemy> GetAllEnemies() => Enemies;
    public ScriptableBoss[] GetBosses(Level level) => Bosses[level];
    public List<ScriptableCardBase> GetAllCards() => Cards;
    public List<ScriptableItemBase> GetAllItems() => Items;


}   
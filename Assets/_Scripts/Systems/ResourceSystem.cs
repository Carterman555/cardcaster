using QFSW.QC;
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
public class ResourceSystem : Singleton<ResourceSystem>
{
    public List<ScriptableLevelLayout> LevelLayouts { get; private set; }
    public Dictionary<RoomType, ScriptableRoom[]> Rooms { get; private set; }
    public List<ScriptableEnemy> Enemies { get; private set; }
    public List<ScriptableBoss> Bosses { get; private set; }
    public List<ScriptableCardBase> AllCards { get; private set; }
    public List<ScriptableCardBase> UnlockedCards { get; private set; }

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
        Bosses = Resources.LoadAll<ScriptableBoss>("Bosses").ToList();

        AllCards = Resources.LoadAll<ScriptableCardBase>("Cards").ToList();

        // convert to card type list in order to load and save the cards
        List<CardType> defaultUnlockedCardTypes = AllCards.Where(c => c.StartUnlocked).Select(c => c.CardType).ToList();
        List<CardType> unlockedCardTypes = ES3.Load("UnlockedCardTypes", defaultUnlockedCardTypes);
        UnlockedCards = AllCards.Where(c => unlockedCardTypes.Contains(c.CardType)).ToList();
    }

    public ScriptableLevelLayout GetRandomLayout() => LevelLayouts.RandomItem();
    public ScriptableRoom[] GetRooms(RoomType roomType) => Rooms[roomType];
    public List<ScriptableEnemy> GetAllEnemies() => Enemies;
    public List<ScriptableBoss> GetBosses(int level) => Bosses.Where(b => b.PossibleLevels.Contains(level)).ToList();

    public List<ScriptableCardBase> GetAllCardsWithLevel(int level) => AllCards.Where(c => c.MinLevel == level).ToList();
    public List<ScriptableCardBase> GetAllCardsUpToLevel(int level) => AllCards.Where(c => c.MinLevel <= level).ToList();

    public List<ScriptableCardBase> GetUnlockedCardsWithLevel(int level) => UnlockedCards.Where(c => c.MinLevel == level).ToList();
    public List<ScriptableCardBase> GetUnlockedCardsUpToLevel(int level) => UnlockedCards.Where(c => c.MinLevel <= level).ToList();

    public ScriptableCardBase GetCard(CardType cardType) => AllCards.FirstOrDefault(c => c.CardType == cardType);
    public ScriptableCardBase GetCard(string cardName) => AllCards.FirstOrDefault(c => c.name == cardName);

    [Command]
    public void UnlockCard(ScriptableCardBase cardToUnlock) {

        if (UnlockedCards.Contains(cardToUnlock)) {
            Debug.LogWarning("Trying to unlock card that is already unlocked");
        }

        UnlockedCards.Add(cardToUnlock);
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();

        //... convert to card type list in order to load and save the cards
        ES3.Save("UnlockedCardTypes", UnlockedCards.Select(c => c.CardType).ToList());
    }
}   
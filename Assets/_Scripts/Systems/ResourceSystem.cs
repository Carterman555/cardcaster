using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : Singleton<ResourceSystem> {
    public List<ScriptableLevelLayout> LevelLayouts { get; private set; }
    public Dictionary<RoomType, ScriptableRoom[]> Rooms { get; private set; }
    public List<ScriptableEnemy> Enemies { get; private set; }
    public List<ScriptableBoss> Bosses { get; private set; }
    public List<ScriptableCardBase> AllCards { get; private set; }
    public List<CardType> UnlockedCards { get; private set; }

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

        if (GameStateManager.InDemo) {
            AllCards = Resources.LoadAll<ScriptableCardBase>("Cards").Where(c => c.InDemo).ToList();
        }
        else {
            AllCards = Resources.LoadAll<ScriptableCardBase>("Cards").ToList();
        }

        UpdateUnlockedCards();
    }

    public void UpdateUnlockedCards() {
        // convert to card type list in order to load and save the cards
        List<CardType> defaultUnlockedCards = AllCards.Where(c => c.StartUnlocked).Select(c => c.CardType).ToList();
        UnlockedCards = ES3.Load("UnlockedCardTypes", defaultUnlockedCards);
    }

    public ScriptableLevelLayout GetRandomLayout() => LevelLayouts.RandomItem();
    public ScriptableRoom[] GetRooms(RoomType roomType) => Rooms[roomType];
    public List<ScriptableEnemy> GetAllEnemies() => Enemies;
    public List<ScriptableBoss> GetBosses(int level) => Bosses.Where(b => b.PossibleLevels.Contains(level)).ToList();

    public List<CardType> GetAllCards() => AllCards.Select(c => c.CardType).ToList();
    public List<CardType> GetAllCardsWithLevel(int level) => AllCards.Where(c => c.UnlockLevel == level).Select(c => c.CardType).ToList();

    public List<CardType> GetAllCardsUpToLevel(int level) => AllCards.Where(c => c.UnlockLevel <= level).Select(c => c.CardType).ToList();

    public List<CardType> GetUnlockedCards() => GetAllCards().Where(c => UnlockedCards.Contains(c)).ToList();
    public List<CardType> GetUnlockedCardsWithLevel(int level) => GetAllCardsWithLevel(level).Where(c => UnlockedCards.Contains(c)).ToList();
    public List<CardType> GetUnlockedCardsUpToLevel(int level) => GetAllCardsUpToLevel(level).Where(c => UnlockedCards.Contains(c)).ToList();

    public ScriptableCardBase GetCardInstance(CardType cardType) => CloneCard(AllCards.FirstOrDefault(c => c.CardType == cardType));
    public ScriptableCardBase GetCardInstance(string cardName) => CloneCard(AllCards.FirstOrDefault(c => c.name == cardName));

    public CardType GetRandomCardWeighted(List<CardType> cardsToChooseFrom) {

        if (cardsToChooseFrom.Count == 0) {
            Debug.LogError("GetRandomCardWeighted given 0 cards to choose from!");
        }

        float totalWeight = 0;
        foreach (CardType cardType in cardsToChooseFrom) {
            float weight = GetCardWeight(cardType);
            totalWeight += weight;
        }

        float remainWeight = UnityEngine.Random.Range(0, totalWeight);
        foreach (CardType cardType in cardsToChooseFrom) {
            float weight = GetCardWeight(cardType);
            remainWeight -= weight;

            if (remainWeight < 0) {
                return cardType;
            }
        }

        Debug.LogError("GetRandomCardWeighted broke!");
        return default;

        float GetCardWeight(CardType cardType) {
            Rarity rarity = AllCards.FirstOrDefault(c => c.CardType == cardType).Rarity;
            switch (rarity) {
                case Rarity.Common: return 1f;
                case Rarity.Uncommon: return 0.66f;
                case Rarity.Rare: return 0.33f;
                case Rarity.Epic: return 0.25f;
                case Rarity.Mythic: return 0.15f;
                default:
                    Debug.LogError("Rarity not supported!");
                    return 0f;
            }
        }
    }

    private ScriptableCardBase CloneCard(ScriptableCardBase original) {
        if (original == null) return null;

        // Use the actual type of the card for proper instantiation
        ScriptableCardBase instance = ScriptableObject.CreateInstance(original.GetType()) as ScriptableCardBase;
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(original), instance);
        instance.OnInstanceCreated();
        return instance;
    }

    [Command]
    public void UnlockCard(CardType cardToUnlock) {

        if (UnlockedCards.Contains(cardToUnlock)) {
            Debug.LogWarning("Trying to unlock card that is already unlocked");
        }

        UnlockedCards.Add(cardToUnlock);
    }

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();

        //... convert to card type list in order to load and save the cards
        ES3.Save("UnlockedCardTypes", UnlockedCards);
    }
}
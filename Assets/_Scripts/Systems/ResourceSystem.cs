using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

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
    public List<ScriptableEnchantment> Enchantments { get; private set; }

    protected override void Awake() {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources() {
        LevelLayouts = Resources.LoadAll<ScriptableLevelLayout>("LevelLayouts").ToList();

        Rooms = Resources.LoadAll<ScriptableRoom>("Rooms")
            .GroupBy(r => r.RoomType)
            .ToDictionary(g => g.Key, g => g.ToArray());

        Enemies = Resources.LoadAll<ScriptableEnemy>("Enemies").ToList();
        Bosses = Resources.LoadAll<ScriptableBoss>("Bosses").ToList();
        AllCards = Resources.LoadAll<ScriptableCardBase>("Cards").ToList();

        UpdateUnlockedCards();

        Enchantments = Resources.LoadAll<ScriptableEnchantment>("Enchantments").ToList();
    }

    public void UpdateUnlockedCards() {
        // convert to card type list in order to load and save the cards
        List<CardType> defaultUnlockedCards = AllCards.Where(c => c.StartUnlocked).Select(c => c.CardType).ToList();
        UnlockedCards = ES3.Load("UnlockedCardTypes", defaultUnlockedCards, ES3EncryptionMigration.GetES3Settings());

        // make sure has all default cards in case they were added after player played game
        foreach (CardType defaultUnlockCard in defaultUnlockedCards) {
            if (!UnlockedCards.Contains(defaultUnlockCard)) {
                UnlockedCards.Add(defaultUnlockCard);
            }
        }

        // make sure player's in demo that unlocked open palms don't have open palms unlocked
        bool defeatedDealer = ES3.Load("DealerDefeatedAmount", 0, ES3EncryptionMigration.GetES3Settings()) > 0;
        if (!defeatedDealer && UnlockedCards.Contains(CardType.OpenPalms)) {
            UnlockedCards.Remove(CardType.OpenPalms);
        }

        // save adjustments
        ES3.Save("UnlockedCardTypes", UnlockedCards, ES3EncryptionMigration.GetES3Settings());
    }

    public ScriptableLevelLayout GetRandomLayout() => LevelLayouts.RandomItem();
    public ScriptableRoom[] GetRooms(RoomType roomType) => Rooms[roomType];
    public List<ScriptableEnemy> GetAllEnemies() => Enemies;
    public List<ScriptableBoss> GetBosses(EnvironmentType environment) => Bosses.Where(b => b.EnvironmentType == environment).ToList();

    #region Cards

    public List<CardType> GetAllCards() => AllCards.Select(c => c.CardType).ToList();
    public List<CardType> GetAllCardsWithEnvironment(EnvironmentType environment) => AllCards.Where(c => c.UnlockEnvironment == environment).Select(c => c.CardType).ToList();

    public List<CardType> GetRewardCards() {
        ScriptableCardBase[] scriptableRewardCards = AllCards.Where(c => c is not ScriptableBlankMemoryCard).ToArray();
        List<CardType> rewardCardTypes = scriptableRewardCards.Select(c => c.CardType).ToList();
        return rewardCardTypes;
    }
    public List<CardType> GetUnlockedRewardCards() {
        List<CardType> rewardCards = GetRewardCards();
        List<CardType> unlockedRewardCards = rewardCards.Where(c => UnlockedCards.Contains(c)).ToList();
        return unlockedRewardCards;
    }
    public List<CardType> GetUnlockedRewardCards(EnvironmentType environment) {
        List<CardType> cardsWithEnv = GetAllCardsWithEnvironment(environment);
        List<CardType> unlockedRewardCardsWithLEnv = GetUnlockedRewardCards().Where(c => cardsWithEnv.Contains(c)).ToList();
        return unlockedRewardCardsWithLEnv;
    }
    public List<CardType> GetLockedRewardCards() {
        List<CardType> rewardCards = GetRewardCards();
        List<CardType> lockedRewardCards = rewardCards.Where(c => !UnlockedCards.Contains(c)).ToList();
        return lockedRewardCards;
    }
    public List<CardType> GetLockedRewardCards(EnvironmentType environment) {
        List<CardType> cardsWithEnv = GetAllCardsWithEnvironment(environment);
        List<CardType> lockedRewardCardsWithEnv = GetLockedRewardCards().Where(c => cardsWithEnv.Contains(c)).ToList();
        return lockedRewardCardsWithEnv;
    }

    public List<CardType> GetPersistentCards() => AllCards.Where(c => c is ScriptablePersistentCard).Select(c => c.CardType).ToList();

    public ScriptableCardBase GetCardInstance(CardType cardType) => CloneCard(AllCards.FirstOrDefault(c => c.CardType == cardType));

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
            Rarity cardRarity = AllCards.FirstOrDefault(c => c.CardType == cardType).Rarity;
            float weight = GetRarityWeight(cardRarity);

            // persistent cards are half as likely
            if (AllCards.First(c => c.CardType == cardType) is ScriptablePersistentCard) {
                weight *= 0.5f;
            }

            return weight;
        }

        float GetRarityWeight(Rarity rarity) {
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

    #endregion

    public ScriptableEnchantment GetEnchantment(EnchantmentType enchantmentType) => Enchantments.FirstOrDefault(e => e.EnchantmentType == enchantmentType);

    protected override void OnApplicationQuit() {
        base.OnApplicationQuit();

        //... convert to card type list in order to load and save the cards
        ES3.Save("UnlockedCardTypes", UnlockedCards, ES3EncryptionMigration.GetES3Settings());
    }
}
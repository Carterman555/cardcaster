using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    public static event Action<ScriptableCardBase> OnPlayCard;

    [Header("Basic Info")]
    [SerializeField] private CardType cardType;
    public CardType CardType => cardType;

    [SerializeField] private LocalizedString locName;
    public LocalizedString Name => locName;

    [FormerlySerializedAs("category")]
    [SerializeField] private LocalizedString locCategory;
    public LocalizedString LocCategory => locCategory;

    [SerializeField] private LocalizedString description;
    public LocalizedString Description => description;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    [SerializeField] private int cost;
    public int Cost => cost;

    [SerializeField] private Rarity rarity;
    public Rarity Rarity => rarity;

    [Header("Advanced Info")]
    [SerializeField] private StackType stackType;
    public StackType StackType => stackType;

    [SerializeField] private bool startUnlocked;
    public bool StartUnlocked => startUnlocked;

    [ConditionalHideReversed("startUnlocked")] [SerializeField] private int unlockLevel = 1;
    public int UnlockLevel => unlockLevel;

    [SerializeField] private bool memoryCard;
    public bool MemoryCard => memoryCard;

    public virtual void OnInstanceCreated() {
    }

    public virtual void OnRemoved() {
    }

    public virtual void TryPlay(Vector2 position) {
    }

    protected virtual void Play(Vector2 position) {
        OnPlayCard?.Invoke(this);
    }

    
    public virtual bool CanPlay() {
        return true;
    }
}

public enum CardType {
    BlackHole,
    Daggerstorm,
    Ghost,
    Teleport,
    Shockwave,
    SpinningFury,
    Launch,
    Boomerblade,
    Might,
    Bomb,
    SpectralBlade,
    ColossusBlade,
    Scorch,
    Electrify,
    Overload,
    Expanse,
    Ricochet,
    Magnetify,
    DyingRage,
    Sustain,
    ThrowingKnife,

    Quickfeet,
    Flurry,
    Power,
    Sharpshot,
    StoneHeart,
    Deadeye,
    LethalEdge,
    SmashingBlow,
    LongDash,
    EssenceReserve,
    EssenceHarvest,

    OpenPalms,
    BlankMemoryCard1,
    BlankMemoryCard2,
    BlankMemoryCard3,
    BlankMemoryCard4,
    BlankMemoryCard5,
    BlankMemoryCard6,
    BlankMemoryCard7,
    BlankMemoryCard8,
    BlankMemoryCard9,
    BlankMemoryCard10,
    ImaginaryTwin,
}

// what to do when play again while already playing
public enum StackType {
    Stackable,
    ResetDuration,
    NotStackable
}
using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private LocalizedString category;
    public LocalizedString Category => category;

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
    WisdomsHold
}

// what to do when play again while already playing
public enum StackType {
    Stackable,
    ResetDuration,
    NotStackable
}
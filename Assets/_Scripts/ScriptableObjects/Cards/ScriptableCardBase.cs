using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    public static event Action<ScriptableCardBase> OnPlayCard;

    [Header("Basic Info")]
    [SerializeField] private CardType cardType;
    public CardType CardType => cardType;

    public string GetName() => cardType.ToPrettyString();

    [SerializeField] private string description;
    public string GetDescription() => description;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private int minLevel = 1;
    public int MinLevel => minLevel;

    [Header("Advanced Info")]
    [SerializeField] private bool canStackWithSelf;
    public bool CanStackWithSelf => canStackWithSelf;

    [SerializeField] private bool startUnlocked;
    public bool StartUnlocked => startUnlocked;

    public virtual void TryPlay(Vector2 position) {

    }

    protected virtual void Play(Vector2 position) {
        OnPlayCard?.Invoke(this);
    }
}

public enum CardType {
    BlackHole,
    DaggerShoot,
    Ghost,
    Teleport,
    Shockwave,
    SwingSword,
    Launch,
    Boomerang,
    BLANK1,
    Bomb,
    SwordHologram,
    MassiveSword,
    Fire,
    Electrify,
    DamageUpDurationDown,
    IncreaseAreaSize,
    Bouncy,
    Magnecitify,
    MissingHealthDamageIncrease,
}
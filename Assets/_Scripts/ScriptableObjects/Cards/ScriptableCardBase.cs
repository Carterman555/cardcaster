using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    [Header("Basic Info")]
    [SerializeField] private CardType cardType;
    public CardType CardType => cardType;

    public string GetName() => cardType.ToPrettyString();

    [SerializeField] private string description;
    public string GetDescription() => description;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [Header("Advanced Info")]
    [SerializeField] private bool isPossibleStartingCard;
    public bool IsPossibleStartingCard => isPossibleStartingCard;

    public virtual void Play(Vector2 position) {

    }
}

public enum CardType {
    BlackHole,
    DaggerShoot,
    Ghost,
    Teleport,
    Shockwave,
    SwingSword,
    LaunchCard,
    BoomerangSword,
    Blast,
    Explosion,
    ShootSwordHologram,
    MassiveSword,
    Fire,
    Electrify,
    DamageUpDurationDown,
    IncreaseAreaSize,
    BouncyProjectiles,
    Magnecitify,
    MissingHealthDamageIncrease,
}
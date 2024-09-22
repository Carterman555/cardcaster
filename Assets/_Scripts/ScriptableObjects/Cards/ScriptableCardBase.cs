using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {
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

    [field: SerializeField] public bool IsPossibleStartingCard { get; private set; }

    public virtual void Play(Vector2 position) {

    }
}

public enum CardType {
    MoveSpeedIncrease,
    BlackHole,
    OrbitingBlackHole,
    DaggerShoot,
    Ghost,
    Teleport,
    Shockwave,
    SwingSword,
    LaunchCard,
    DeflectBullets,
    BoomerangSword,
    FrostBlast,
    Explosion,
    ShootSwordHologram,
    FireSword,
    ElectricSword,
    MassiveSword,
    IncreaseSummonDamage,
    BiggerSword,
    IncreaseAreaEffects
}
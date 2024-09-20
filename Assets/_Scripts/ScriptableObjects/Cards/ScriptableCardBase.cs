using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class ScriptableCardBase : ScriptableObject, ICollectable {

    [SerializeField] private CardType cardType;
    public CardType CardType => cardType;

    [SerializeField] private string cardName;
    public string GetName() => cardName;

    [SerializeField] private string description;
    public string GetDescription() => description;

    [SerializeField] private int cost;
    public int GetCost() => cost;

    [SerializeField] private Rarity rarity;
    public Rarity GetRarity() => rarity;

    [SerializeField] private Sprite sprite;
    public Sprite GetSprite() => sprite;

    [field: SerializeField] public bool IsPossibleStartingCard { get; private set; }

    [field: SerializeField] public bool IsPositional { get; private set; }

    [SerializeField] protected float effectDuration;


    private Coroutine draggingCardCoroutine;

    public virtual void OnStartDraggingCard() {
        draggingCardCoroutine = AbilityManager.Instance.StartCoroutine(DraggingCard());
    }

    private IEnumerator DraggingCard() {
        while (true) {
            yield return null;
            DraggingUpdate();
        }
    }

    protected virtual void DraggingUpdate() {

    }

    public virtual void Play(Vector2 position) {

        AbilityManager.Instance.StopCoroutine(draggingCardCoroutine);
        AbilityManager.Instance.StartCoroutine(StopAfterDuration());

        AbilityManager.Instance.AddActiveCard(CardType);
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(effectDuration);
        Stop();
    }

    public virtual void Stop() {
        AbilityManager.Instance.RemoveActiveCard(CardType);
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
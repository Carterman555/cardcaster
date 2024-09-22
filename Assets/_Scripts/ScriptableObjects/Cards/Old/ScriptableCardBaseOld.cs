using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class ScriptableCardBaseOld : ScriptableObject, ICollectable {

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

    [field: SerializeField] public bool IsPositional { get; private set; }

    [SerializeField] protected float effectDuration;


    private Coroutine draggingCardCoroutine;

    public virtual void OnStartDraggingCard(Transform cardTransform) {
        draggingCardCoroutine = AbilityManagerOld.Instance.StartCoroutine(DraggingCard(cardTransform));
    }

    private IEnumerator DraggingCard(Transform cardTransform) {
        while (true) {
            yield return null;
            DraggingUpdate(Camera.main.ScreenToWorldPoint(cardTransform.position));
        }
    }

    protected virtual void DraggingUpdate(Vector2 cardposition) {

    }

    public virtual void Play(Vector2 position) {

        if (draggingCardCoroutine != null) {
            AbilityManagerOld.Instance.StopCoroutine(draggingCardCoroutine);
        }

        AbilityManagerOld.Instance.StartCoroutine(StopAfterDuration());

        AbilityManagerOld.Instance.AddActiveCard(CardType);
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(effectDuration);
        Stop();
    }

    public virtual void Stop() {
        AbilityManagerOld.Instance.RemoveActiveCard(CardType);
    }
}

//public enum CardType {
//    MoveSpeedIncrease,
//    BlackHole,
//    OrbitingBlackHole,
//    DaggerShoot,
//    Ghost,
//    Teleport,
//    Shockwave,
//    SwingSword,
//    LaunchCard,
//    DeflectBullets,
//    BoomerangSword,
//    FrostBlast,
//    Explosion,
//    ShootSwordHologram,
//    FireSword,
//    ElectricSword,
//    MassiveSword,
//    IncreaseSummonDamage,
//    BiggerSword,
//    IncreaseAreaEffects
//}
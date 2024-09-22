using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableAbilityCardBase : ScriptableCardBase {

    [SerializeField] private AbilityAttribute abilityAttributes;
    public AbilityAttribute AbilityAttributes => abilityAttributes;

    [field: SerializeField] public bool IsPositional { get; private set; }
    [field: SerializeField] public bool IsModifiable { get; private set; } = true;

    [SerializeField] protected float effectDuration;

    public bool IsCompatible(ScriptableModifierCardBase modifier) {
        return (abilityAttributes & modifier.AbilityAttributesToModify) != 0;
    }

    private Coroutine draggingCardCoroutine;

    public virtual void OnStartDraggingCard(Transform cardTransform) {
        draggingCardCoroutine = AbilityManager.Instance.StartCoroutine(DraggingCard(cardTransform));
    }

    private IEnumerator DraggingCard(Transform cardTransform) {
        while (true) {
            yield return null;
            DraggingUpdate(Camera.main.ScreenToWorldPoint(cardTransform.position));
        }
    }

    protected virtual void DraggingUpdate(Vector2 cardposition) { }

    public override void Play(Vector2 position) {
        base.Play(position);

        if (draggingCardCoroutine != null) {
            AbilityManager.Instance.StopCoroutine(draggingCardCoroutine);
        }

        AbilityManager.Instance.StartCoroutine(StopAfterDuration());

        if (IsModifiable) {
            AbilityManager.Instance.ApplyModifiers(this);
        }
    }

    private IEnumerator StopAfterDuration() {
        yield return new WaitForSeconds(effectDuration);
        Stop();
    }

    public virtual void Stop() {
    }
}

[Flags]
public enum AbilityAttribute {
    None = 0,
    DealsDamage = 1 << 0,
    HasArea = 1 << 1,
    HasDuration = 1 << 2,
    IsProjectile = 1 << 3,
}

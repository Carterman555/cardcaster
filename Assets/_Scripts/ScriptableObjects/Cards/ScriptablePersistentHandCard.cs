using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptablePersistentHandCard : ScriptablePersistentCard {

    private Coroutine updateCor;

    protected bool inHand;

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();

        updateCor = AbilityManager.Instance.StartCoroutine(UpdateCor());
    }

    private IEnumerator UpdateCor() {
        while (true) {
            inHand = CardsUIManager.Instance.TryGetHandCard(this, out HandCard handCard);
            if (inHand) {
                InHandUpdate();
            }

            yield return null;
        }
    }

    public override void OnRemoved() {
        base.OnRemoved();

        AbilityManager.Instance.StopCoroutine(updateCor);
    }

    protected virtual void InHandUpdate() {

    }
}

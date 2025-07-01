using System;
using System.Collections;
using UnityEngine;

public class ScriptablePersistentHandCard : ScriptablePersistentCard {

    private Coroutine updateCor;

    protected bool inHand;

    public override void OnInstanceCreated() {
        base.OnInstanceCreated();

        updateCor = GameSceneManager.Instance.StartCoroutine(UpdateCor());
    }

    public override void OnRemoved() {
        base.OnRemoved();

        GameSceneManager.Instance.StopCoroutine(updateCor);
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

    protected virtual void InHandUpdate() {

    }
}

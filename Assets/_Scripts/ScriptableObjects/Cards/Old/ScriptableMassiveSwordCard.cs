using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MassiveSwordCard", menuName = "Cards/Massive Sword Card")]
public class ScriptableMassiveSwordCard : ScriptableStatsModifierCard {

    [Header("Visual")]
    [SerializeField] private Sprite bigSwordSprite;
    [SerializeField] private Sprite normalSwordSprite;

    public override void Play(Vector2 position) {
        base.Play(position);

        //PlayerMovement.Instance.enabled = false;

        GrowSword();
    }

    public override void Stop() {
        base.Stop();

        //PlayerMovement.Instance.enabled = true;

        ShrinkSword();
    }

    private void GrowSword() {

        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        swordVisual.sprite = bigSwordSprite;

        float transitionDuration = 0.5f;

        float sizeMult = GetPlayerStatsModifier().SwordSizePercent.PercentToMult();
        Vector2 bigSwordStartingSize = Vector2.one * (1f/sizeMult);

        // grow sword
        swordVisual.transform.localScale = bigSwordStartingSize;
        swordVisual.transform.DOScale(1f, transitionDuration);
    }

    private void ShrinkSword() {
        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;

        float transitionDuration = 0.5f;

        float sizeMult = GetPlayerStatsModifier().SwordSizePercent.PercentToMult();
        Vector2 bigSwordEndingSize = Vector2.one * (1f / sizeMult);

        // shrink sword, then switch back to orignal sword
        swordVisual.transform.DOScale(bigSwordEndingSize, transitionDuration).OnComplete(() => {
            swordVisual.sprite = normalSwordSprite;
            swordVisual.transform.localScale = Vector3.one;
        });
    }
}

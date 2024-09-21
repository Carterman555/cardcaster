using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MassiveSwordCard", menuName = "Cards/Massive Sword Card")]
public class ScriptableMassiveSwordCard : ScriptableStatsModifierCard {

    [Header("Visual")]
    [SerializeField] private SpriteRenderer bigSwordVisualPrefab;
    private SpriteRenderer bigSwordVisual;

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

        float transitionDuration = 0.5f;

        bigSwordVisual = bigSwordVisualPrefab.Spawn(swordVisual.transform.position, swordVisual.transform.rotation, ReferenceSystem.Instance.PlayerSword);
        ReferenceSystem.Instance.SetPlayerSwordVisual(bigSwordVisual);

        float sizeMult = PlayerStats.PercentToMult(GetPlayerStatsModifier().SwordSizePercent);
        Vector2 bigSwordStartingSize = Vector2.one * (1f/sizeMult);

        // grow big sword
        bigSwordVisual.transform.localScale = bigSwordStartingSize;
        bigSwordVisual.transform.DOScale(1f, transitionDuration);

        // fade in big sword
        bigSwordVisual.Fade(0f);
        bigSwordVisual.DOFade(1f, transitionDuration * 0.5f);

        // grow and fade out original sword for half the transition
        swordVisual.transform.DOScale(sizeMult * 0.5f, transitionDuration * 0.5f);
        swordVisual.DOFade(0f, transitionDuration * 0.5f).OnComplete(() => {

            // reset visual and disable
            swordVisual.gameObject.SetActive(false);
            swordVisual.transform.localScale = Vector3.one;
            swordVisual.Fade(1f);
        });
    }

    private void ShrinkSword() {
        SpriteRenderer swordVisual = ReferenceSystem.Instance.PlayerSwordVisual;
        ReferenceSystem.Instance.SetPlayerSwordVisual(swordVisual);

        float transitionDuration = 0.5f;

        float sizeMult = PlayerStats.PercentToMult(GetPlayerStatsModifier().SwordSizePercent);
        Vector2 bigSwordEndingSize = Vector2.one * (2f / sizeMult);

        // shrink big sword for half the transition
        bigSwordVisual.transform.DOScale(bigSwordEndingSize, transitionDuration * 0.5f);

        // fade out big sword for half the transition
        bigSwordVisual.DOFade(0f, transitionDuration * 0.5f).OnComplete(() => {
            bigSwordVisual.gameObject.ReturnToPool();
        });

        // shrink and fade in original sword
        swordVisual.gameObject.SetActive(true);

        swordVisual.transform.localScale = Vector2.one * sizeMult;
        swordVisual.transform.DOScale(1f, transitionDuration);

        swordVisual.Fade(0f);
        swordVisual.DOFade(1f, transitionDuration);
    }
}

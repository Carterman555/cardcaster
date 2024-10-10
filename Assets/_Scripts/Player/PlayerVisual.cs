using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class PlayerVisual : StaticInstance<PlayerVisual> {

    private SpriteRenderer[] playerSprites;

    private List<FadeEffect> fadeEffects = new();

    /// <summary>
    /// i need this to store a list or dictionary of the added fade effects so if a strong one gets removed the weak one comes back
    /// </summary>

    protected override void Awake() {
        base.Awake();
        playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();
    }

    public FadeEffect AddFadeEffect(int overrideStrength, float fadeAmount, float duration = 0f) {

        FadeEffect fadeEffect = new() {
            FadeAmount = fadeAmount,
            Strength = overrideStrength,
        };
        fadeEffects.Add(fadeEffect);

        UpdateFade(duration);

        return fadeEffect;
    }

    public void RemoveFadeEffect(FadeEffect fadeEffect, float duration = 0f) {

        if (!fadeEffects.Contains(fadeEffect)) {
            Debug.LogError("Trying to Remove Effect that isn't Active");
            return;
        }
        fadeEffects.Remove(fadeEffect);

        UpdateFade(duration);
    }

    private void UpdateFade(float duration = 0f) {

        float fadeAmount;
        if (fadeEffects.Count > 0) {
            FadeEffect strongestFadeEffect = fadeEffects.OrderBy(f => f.Strength).Last();
            fadeAmount = strongestFadeEffect.FadeAmount;
        }
        // if no effect then be fully opaque
        else {
            fadeAmount = 1f;
        }

        FadePlayer(fadeAmount, duration);
    }

    private void FadePlayer(float fadeAmount, float duration = 0f) {
        foreach (SpriteRenderer playerSprite in playerSprites) {
            if (duration > 0) {
                playerSprite.DOFade(fadeAmount, duration);
            }
            else {
                playerSprite.Fade(fadeAmount);
            }
        }
    }
}

public struct FadeEffect {
    public float FadeAmount;
    public float Strength;
}
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerVisual : StaticInstance<PlayerVisual> {

    private SpriteRenderer[] playerSprites;
    private Dictionary<string, float> fadeEffects = new Dictionary<string, float>();

    protected override void Awake() {
        base.Awake();
        playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetFadeEffect(string effectName, float fadeAmount, float duration = 0f) {
        fadeEffects[effectName] = fadeAmount;
        UpdatePlayerFade(duration);
    }

    public void RemoveFadeEffect(string effectName, float duration = 0f) {
        fadeEffects.Remove(effectName);
        UpdatePlayerFade(duration);
    }

    private void UpdatePlayerFade(float duration) {
        float lowestFadeAmount = 1f;
        foreach (float fadeAmount in fadeEffects.Values) {
            if (fadeAmount < lowestFadeAmount) {
                lowestFadeAmount = fadeAmount;
            }
        }

        foreach (SpriteRenderer playerSprite in playerSprites) {
            if (duration > 0) {
                playerSprite.DOFade(lowestFadeAmount, duration);
            }
            else {
                playerSprite.Fade(lowestFadeAmount);
            }
        }
    }
}
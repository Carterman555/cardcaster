using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerVisual : StaticInstance<PlayerVisual> {

    private SpriteRenderer[] playerSprites;

    private int currentOverrideStrength;

    protected override void Awake() {
        base.Awake();
        playerSprites = PlayerMovement.Instance.GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetFadeEffect(int overrideStrength, float fadeAmount, float duration = 0f) {

        if (overrideStrength < currentOverrideStrength) {
            return;
        }

        currentOverrideStrength = overrideStrength;

        foreach (SpriteRenderer playerSprite in playerSprites) {
            if (duration > 0) {
                playerSprite.DOFade(fadeAmount, duration);
            }
            else {
                playerSprite.Fade(fadeAmount);
            }
        }
    }

    public void RemoveFadeEffect(int overrideStrength, float duration = 0f) {

        if (overrideStrength < currentOverrideStrength) {
            return;
        }

        foreach (SpriteRenderer playerSprite in playerSprites) {
            playerSprite.Fade(1f);
        }

        currentOverrideStrength = 0;
    }
}
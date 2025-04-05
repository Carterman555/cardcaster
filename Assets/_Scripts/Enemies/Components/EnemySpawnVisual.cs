using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnVisual : MonoBehaviour {

    private List<Tween> fadeTweens;

    private void OnEnable() {

        // Fade in
        fadeTweens = new();

        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            float originalAlpha = spriteRenderer.color.a;

            spriteRenderer.Fade(0);

            float fadeDuration = 0.5f;
            fadeTweens.Add(spriteRenderer.DOFade(originalAlpha, fadeDuration));
        }
    }

    // if enemies dies while fading in, complete the fade in instantly
    private void OnDisable() {
        foreach (Tween fadeTween in fadeTweens) {
            fadeTween.Complete();
        }
    }
}

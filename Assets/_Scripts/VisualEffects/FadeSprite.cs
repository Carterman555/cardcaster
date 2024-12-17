using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FadeSprite : MonoBehaviour {

    private float originalFade;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalFade = spriteRenderer.color.a;
    }

    private void OnEnable() {
        spriteRenderer.Fade(originalFade);
    }

    public void FadeOut(float duration = 0.2f) {
        spriteRenderer.DOFade(0f, duration);
    }

    public void FadeIn(float duration = 0.2f) {
        spriteRenderer.DOFade(originalFade, duration);
    }
}

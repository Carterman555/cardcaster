using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnVisual : MonoBehaviour {


    private void OnEnable() {
        FadeIn();
    }

    private void FadeIn() {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            float originalAlpha = spriteRenderer.color.a;

            spriteRenderer.Fade(0);

            float fadeDuration = 0.5f;
            spriteRenderer.DOFade(originalAlpha, fadeDuration);
        }
    }


    [SerializeField] private Material spawnMaterial;

    private void DissolveIn() {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        float dissolveSpeed = 0.5f;
        DissolverVisual dissolverVisual = new DissolverVisual(spawnMaterial, spriteRenderers, dissolveSpeed);

        StartCoroutine(dissolverVisual.DissolveIn(true));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DissolverVisual {

    private Material originalMaterial;
    private Material materialInstance;

    private Image[] images;
    private SpriteRenderer[] spriteRenderers;

    private float dissolveSpeed;

    private Action onDissolveInFunc;
    private Action onDissolveOutFunc;

    public DissolverVisual(Material material, Image[] images, float dissolveSpeed) {
        this.images = images;
        this.dissolveSpeed = dissolveSpeed;

        originalMaterial = images[0].material;

        materialInstance = new Material(material);
        foreach (Image image in images) {
            image.material = materialInstance;
        }
    }

    public DissolverVisual(Material material, SpriteRenderer[] spriteRenderers, float dissolveSpeed) {
        this.spriteRenderers = spriteRenderers;
        this.dissolveSpeed = dissolveSpeed;

        originalMaterial = spriteRenderers[0].material;

        materialInstance = new Material(material);
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
            spriteRenderer.material = materialInstance;
        }
    }

    public void AddDissolvedInListener(Action onDissolveInFunc) {
        this.onDissolveInFunc = onDissolveInFunc;
    }

    public void AddDissolvedOutListener(Action onDissolveOutFunc) {
        this.onDissolveOutFunc = onDissolveOutFunc;
    }

    public IEnumerator DissolveIn(bool resetOnComplete = false) {

        float fadeAmount = 0.4f;
        while (fadeAmount > -0.1f) {
            materialInstance.SetFloat("_FadeAmount", fadeAmount);
            fadeAmount -= dissolveSpeed * Time.unscaledDeltaTime; // unscaled so can play when timescale = 0
            yield return null;
        }

        ResetMaterials();
        onDissolveInFunc?.Invoke();
    }

    public IEnumerator DissolveOut(bool resetOnComplete = false) {

        float fadeAmount = -0.1f;
        while (fadeAmount < 0.4f) {
            materialInstance.SetFloat("_FadeAmount", fadeAmount);
            fadeAmount += dissolveSpeed * Time.unscaledDeltaTime; // unscaled so can play when timescale = 0
            yield return null;
        }

        ResetMaterials();
        onDissolveOutFunc?.Invoke();
    }

    public void ResetMaterials() {

        if (images != null) {
            foreach (Image image in images) {
                image.material = originalMaterial;
            }
        }

        if (spriteRenderers != null) {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers) {
                spriteRenderer.material = originalMaterial;
            }
        }
    }
}

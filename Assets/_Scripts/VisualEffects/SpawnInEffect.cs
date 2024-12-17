using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInEffect : MonoBehaviour {

    private void OnEnable() {
        transform.localScale = Vector3.zero;
    }

    public void Grow() {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, duration: 0.2f);
    }

    public void FadeOut() {
        FadeSprite[] fadeSprites = GetComponentsInChildren<FadeSprite>();
        foreach (FadeSprite fadeSprite in fadeSprites) {
            fadeSprite.FadeOut();
        }
    }
}

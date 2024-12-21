using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : StaticInstance<Trainer> {

    #region Teleport to next room

    [Header("Teleport")]
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private SpriteRenderer visual;
    private float originalFade;

    protected override void Awake() {
        base.Awake();
        originalFade = visual.color.a;
    }

    private void OnEnable() {
        visual.Fade(originalFade);
    }

    public void TeleportToNextRoom() {

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);

        float duration = 0.3f;
        visual.DOFade(0, duration).SetEase(Ease.InSine).OnComplete(() => {
            transform.position = teleportPoint.position;

            visual.DOFade(originalFade, duration).SetEase(Ease.InSine);
        });
    }

    #endregion


}

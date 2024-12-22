using DG.Tweening;
using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trainer : StaticInstance<Trainer> {

    [SerializeField] private SpriteRenderer visual;

    protected override void Awake() {
        base.Awake();

        SetOriginalFade();
        SetOriginalMaterial();
    }

    #region Teleport to next room

    [Header("Teleport")]
    [SerializeField] private Transform teleportPoint;
    private float originalFade;

    private void SetOriginalFade() {
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

    #region Rage

    [SerializeField] private Material redMaterial;
    private Material redMaterialInstance;
    private Material originalMaterial;

    private BreakOnDamaged[] barrels;

    private void SetOriginalMaterial() {
        originalMaterial = visual.material;
    }

    private void Start() {
        barrels = FindObjectsOfType<BreakOnDamaged>().Where(b => b.name.StartsWith("Barrel")).ToArray();

        foreach (BreakOnDamaged barrel in barrels) {
            barrel.OnDamaged += EnterRage;
            print("sub");
        }
    }

    private void EnterRage() {
        print("enter rage");

        redMaterialInstance = new Material(redMaterial);
        visual.material = redMaterialInstance;

        StartCoroutine(FadeInRed());
    }

    private IEnumerator FadeInRed() {

        float glow = 0f;

        float finalGlow = 2f;
        float glowFadeSpeed = 3f;

        while (glow < finalGlow) {
            redMaterialInstance.SetFloat("_ShineGlow", glow);
            glow += glowFadeSpeed * Time.deltaTime;

            yield return null;
        }
    }

    #endregion

}

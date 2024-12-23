using DG.Tweening;
using Mono.CSharp;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Trainer : StaticInstance<Trainer> {

    [SerializeField] private SpriteRenderer visual;

    protected override void Awake() {
        base.Awake();

        SetOriginalFade();
        SetOriginalMaterial();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void OnEnable() {
        visual.Fade(originalFade);

        swordRotate.gameObject.SetActive(false);
        inRage = false;
    }

    #region Teleport to next room

    [Header("Teleport")]
    [SerializeField] private Transform teleportPoint;
    private float originalFade;

    private void SetOriginalFade() {
        originalFade = visual.color.a;
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

    [SerializeField] private MMAutoRotate swordRotate;

    [SerializeField] private Material redMaterial;
    [SerializeField] private SpriteRenderer swordRenderer;
    private Material redMaterialInstance;
    private Material originalMaterial;

    private BreakOnDamaged[] barrels;

    private NavMeshAgent agent;

    private bool inRage;

    private void SetOriginalMaterial() {
        originalMaterial = visual.material;
    }

    private void Start() {
        barrels = FindObjectsOfType<BreakOnDamaged>().Where(b => b.name.StartsWith("Barrel")).ToArray();

        foreach (BreakOnDamaged barrel in barrels) {
            barrel.OnDamaged += EnterRage;
        }
    }

    private void EnterRage() {
        inRage = true;

        StartCoroutine(FadeInRed());
        SetupSword();

        agent.isStopped = false;

        DialogBox.Instance.ShowText("DO NOT BREAK MY BARRELS!!!", showEnterText: false);
    }

    private IEnumerator FadeInRed() {

        redMaterialInstance = new Material(redMaterial);
        visual.material = redMaterialInstance;

        float glow = 0f;

        float finalGlow = 2f;
        float glowFadeSpeed = 3f;

        while (glow < finalGlow) {
            redMaterialInstance.SetFloat("_ShineGlow", glow);
            glow += glowFadeSpeed * Time.deltaTime;

            yield return null;
        }
    }

    private void SetupSword() {

        swordRotate.gameObject.SetActive(true);

        swordRotate.enabled = false;

        swordRenderer.Fade(0f);
        swordRenderer.DOFade(1f, duration: 0.3f).OnComplete(() => {
            swordRotate.enabled = true;
        });
    }

    private void Update() {
        if (inRage) {
            if (PlayerMeleeAttack.Instance != null) {
                agent.SetDestination(PlayerMeleeAttack.Instance.transform.position);
            }
        }
    }

    #endregion

}

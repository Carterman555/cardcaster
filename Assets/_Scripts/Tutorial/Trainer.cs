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

        SubToRageEvents();
    }

    private void OnDisable() {
        UnsubFromRageEvents();
    }

    #region Teleport to next room

    [Header("Teleport")]
    [SerializeField] private Transform roomOneTeleportPoint;
    [SerializeField] private Transform roomTwoTeleportPoint;
    private float originalFade;

    private void SetOriginalFade() {
        originalFade = visual.color.a;
    }

    public void TeleportToRoomTwo() {
        TeleportToPoint(roomTwoTeleportPoint);
    }

    private void TeleportToPoint(Transform teleportPoint) {
        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);

        float duration = 0.3f;
        agent.enabled = false; // agent messes with teleport so disable it
        visual.DOFade(0, duration).SetEase(Ease.InSine).OnComplete(() => {
            transform.position = teleportPoint.position;
            agent.enabled = true;

            visual.DOFade(originalFade, duration).SetEase(Ease.InSine);
        });
    }

    #endregion

    #region Rage

    [Header("Rage")]
    [SerializeField] private MMAutoRotate swordRotate;

    [SerializeField] private Material redMaterial;
    [SerializeField] private SpriteRenderer swordRenderer;
    private Material redMaterialInstance;
    private Material originalMaterial;

    private BreakOnDamaged[] barrels;

    private NavMeshAgent agent;

    private bool inRage;

    [SerializeField] private TriggerContactTracker roomTwoTrigger;
    private bool enteredRoomTwo;

    private void SetOriginalMaterial() {
        originalMaterial = visual.material;
    }

    private void SubToRageEvents() {
        // enter rage if break barrel
        barrels = FindObjectsOfType<BreakOnDamaged>().Where(b => b.name.StartsWith("Barrel")).ToArray();
        foreach (BreakOnDamaged barrel in barrels) {
            barrel.OnDamaged += OnBreakBarrel;
        }

        ScriptableCardBase.OnPlayCard += EnterRageIfWrongTeleport;

        roomTwoTrigger.OnEnterContact += OnEnteredRoomTwo;
        roomTwoTrigger.OnExitContact += EnterRageTeleportOut;

    }

    private void UnsubFromRageEvents() {
        foreach (BreakOnDamaged barrel in barrels) {
            barrel.OnDamaged -= OnBreakBarrel;
        }

        ScriptableCardBase.OnPlayCard -= EnterRageIfWrongTeleport;

        roomTwoTrigger.OnEnterContact -= OnEnteredRoomTwo;
        roomTwoTrigger.OnExitContact -= EnterRageTeleportOut;
    }

    private void OnBreakBarrel() {
        DialogBox.Instance.ShowText("DO NOT BREAK MY BARRELS!!!", showEnterText: false);
        EnterRage();
    }

    // enter rage if don't teleport into next room
    private void EnterRageIfWrongTeleport(ScriptableCardBase playedCard) => StartCoroutine(EnterRageIfWrongTeleportCor(playedCard));
    private IEnumerator EnterRageIfWrongTeleportCor(ScriptableCardBase playedCard) {

        if (enteredRoomTwo) {
            yield break; // exit coroutine
        }

        //... wait for trigger to detect player entering next room
        yield return null;

        bool playerInRoomTwo = roomTwoTrigger.HasContact();

        if (playedCard is ScriptableTeleportCard && !playerInRoomTwo) {
            DialogBox.Instance.ShowText("YOU WERE SUPPOSED TO TELEPORT TO THE NEXT ROOM ON THE RIGHT!!", showEnterText: false);
            EnterRage();
            print("wrong teleport");
        }
    }

    private void OnEnteredRoomTwo() {
        enteredRoomTwo = true;
    }

    // enter rage if the player teleports out of the second room
    private void EnterRageTeleportOut() {
        DialogBox.Instance.ShowText("I'M TRYING TO TEACH YOU! DON'T LEAVE THAT ROOM!!!", showEnterText: false);

        TeleportToPoint(roomOneTeleportPoint);

        EnterRage();
        print("exit room");
    }

    private void EnterRage() {
        inRage = true;

        StartCoroutine(FadeInRed());
        SetupSword();
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
            if (PlayerMeleeAttack.Instance != null && agent.enabled) {
                agent.isStopped = false;
                agent.SetDestination(PlayerMeleeAttack.Instance.transform.position);
            }
        }
    }

    #endregion

}

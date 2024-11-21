using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Chest : MonoBehaviour {

    private ScriptableCardBase[] scriptableCards;
    [SerializeField] private ChestCard[] chestCards;

    private bool opened;

    [SerializeField] private Animator anim;

    private Interactable interactable;

    private const int CARD_AMOUNT = 3;

    private void Awake() {
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() {
        interactable.OnInteract += TryOpenChest;

        opened = false;
    }
    private void OnDisable() {
        interactable.OnInteract -= TryOpenChest;
    }

    private void Start() {
        ChooseUniqueRandomCards();
    }

    private void ChooseUniqueRandomCards() {

        int currentLevel = LevelManager.Instance.GetLevel();
        List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetUnlockedCardsUpToLevel(currentLevel);

        // Check if we have enough cards to choose from
        if (possibleCards.Count < CARD_AMOUNT) {
            Debug.LogError("Not enough cards to choose from.");
        }

        // select [CARD_AMOUNT] unique random cards
        scriptableCards = possibleCards.OrderBy(x => UnityEngine.Random.value).Distinct().Take(CARD_AMOUNT).ToArray();
    }

    private void TryOpenChest() {
        if (!opened) {
            StartCoroutine(Open());
        }
    }

    private IEnumerator Open() {

        opened = true;
        interactable.enabled = false;

        anim.SetTrigger("open");

        float delay = 0.3f;
        yield return new WaitForSeconds(delay);

        // show cards
        for (int cardIndex = 0; cardIndex < scriptableCards.Length; cardIndex++) {
            chestCards[cardIndex].Setup(this, scriptableCards[cardIndex], cardIndex);
        }
    }

    public IEnumerator OnSelectCollectable(int selectedCollectableIndex) {

        float duration = 0.5f;

        // hide other collectables in chest
        for (int collectableIndex = 0; collectableIndex < chestCards.Length; collectableIndex++) {
            if (collectableIndex != selectedCollectableIndex) {
                chestCards[collectableIndex].ReturnToChest(duration);
            }
        }

        yield return new WaitForSeconds(duration);

        // then hide chest
        transform.ShrinkThenDestroy();
    }
}

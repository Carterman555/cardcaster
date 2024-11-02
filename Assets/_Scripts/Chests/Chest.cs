using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Chest : MonoBehaviour {

    private ICollectable[] scriptableCollectables;
    [SerializeField] private ChestCollectable[] collectables;

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

        List<ScriptableCardBase> possibleCards = ResourceSystem.Instance.GetAllCards();

        // Check if we have enough cards to choose from
        if (possibleCards.Count < CARD_AMOUNT) {
            Debug.LogError("Not enough cards to choose from.");
        }

        // select [CARD_AMOUNT] unique random cards
        scriptableCollectables = possibleCards.OrderBy(x => UnityEngine.Random.value).Distinct().Take(CARD_AMOUNT).ToArray();
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
        for (int collectableIndex = 0; collectableIndex < scriptableCollectables.Length; collectableIndex++) {
            collectables[collectableIndex].Setup(this, scriptableCollectables[collectableIndex], collectableIndex);
        }
    }

    public IEnumerator OnSelectCollectable(int selectedCollectableIndex) {

        float duration = 0.5f;

        // hide other collectables in chest
        for (int collectableIndex = 0; collectableIndex < collectables.Length; collectableIndex++) {
            if (collectableIndex != selectedCollectableIndex) {
                collectables[collectableIndex].ReturnToChest(duration);
            }
        }

        yield return new WaitForSeconds(duration);

        // then hide chest
        transform.ShrinkThenDestroy();
    }
}

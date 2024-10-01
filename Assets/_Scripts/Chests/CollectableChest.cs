using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CollectableChest : MonoBehaviour {

    [SerializeField] private InputActionReference interactAction;

    private ICollectable[] scriptableCollectables;
    [SerializeField] private ChestCollectable[] collectables;
    [SerializeField] private bool cardChest;

    private bool canOpen;
    private bool opened;

    [SerializeField] private Animator anim;

    private const int CARD_AMOUNT = 3;
    private const int ITEM_AMOUNT = 1;

    private void Start() {

        if (cardChest) {
            ChooseUniqueRandomCards();
        }
        else {
            ChooseUniqueRandomItems();
        }
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

    private void ChooseUniqueRandomItems() {

        List<ScriptableItemBase> possibleItems = ResourceSystem.Instance.GetAllItems();

        // Check if we have enough items to choose from
        if (possibleItems.Count < ITEM_AMOUNT) {
            Debug.LogError("Not enough items to choose from.");
        }

        // select [ITEM_AMOUNT] unique random items
        scriptableCollectables = possibleItems.OrderBy(x => UnityEngine.Random.value).Distinct().Take(ITEM_AMOUNT).ToArray();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (!opened && collision.gameObject.layer == GameLayers.PlayerLayer) {
            // shine or outline - TODO
            anim.SetBool("hovering", true);

            canOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!opened && collision.gameObject.layer == GameLayers.PlayerLayer) {
            // disable shine or outline - TODO
            anim.SetBool("hovering", false);

            canOpen = false;
        }
    }

    private void Update() {
        if (canOpen) {
            if (interactAction.action.triggered) {
                StartCoroutine(Open());
            }
        }
    }

    private IEnumerator Open() {
        opened = false;
        canOpen = false;

        anim.SetTrigger("open");

        float delay = 0.3f;
        yield return new WaitForSeconds(delay);

        // show collectables
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

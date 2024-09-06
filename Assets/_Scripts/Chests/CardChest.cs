using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CardChest : MonoBehaviour {

    [SerializeField] private InputActionReference interactAction;

    private ScriptableCardBase[] scriptableCards = new ScriptableCardBase[3];
    [SerializeField] private ChestCollectable[] cards;

    private bool canOpen;
    private bool opened;

    [SerializeField] private Animator anim;

    private const int CARD_AMOUNT = 3;

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
        scriptableCards = possibleCards.OrderBy(x => UnityEngine.Random.value).Distinct().Take(CARD_AMOUNT).ToArray();
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

        // show cards
        for (int cardIndex = 0; cardIndex < scriptableCards.Length; cardIndex++) {
            cards[cardIndex].Setup(this, scriptableCards[cardIndex], cardIndex);
        }
    }

    public IEnumerator SelectCard(int collectableIndex) {

        float duration = 0.5f;

        // hide other cards in chest
        for (int cardIndex = 0; cardIndex < cards.Length; cardIndex++) {
            if (cardIndex != collectableIndex) {
                cards[cardIndex].ReturnToChest(duration);
            }
        }

        yield return new WaitForSeconds(duration);

        // then hide chest
        transform.ShrinkThenDestroy();
    }
}

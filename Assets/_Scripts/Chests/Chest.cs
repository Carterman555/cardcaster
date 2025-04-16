using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour {

    [SerializeField][Range(0f, 1f)] private float healProbability;

    [SerializeField] private Transform chestItemContainer;
    [SerializeField] private ChestCard chestCardPrefab;
    [SerializeField] private ChestHeal chestHealPrefab;
    public List<IChestItem> ChestItems { get; private set; } = new();

    private List<CardType> remainingPossibleCards;

    private bool opened;

    [SerializeField] private Animator anim;

    private Interactable interactable;

    private const int ITEM_AMOUNT = 3;

    private void Awake() {
        interactable = GetComponent<Interactable>();
    }

    private void OnEnable() {
        interactable.OnInteract += TryOpenChest;

        opened = false;
        SetupRemainingCardsList();
    }
    private void OnDisable() {
        interactable.OnInteract -= TryOpenChest;
    }

    private void SetupRemainingCardsList() {

        int currentLevel = GameSceneManager.Instance.Level;
        remainingPossibleCards = ResourceSystem.Instance.GetUnlockedCards();

        // Check if we have enough cards to choose from
        if (remainingPossibleCards.Count < ITEM_AMOUNT) {
            Debug.LogError("Not enough cards to choose from.");
        }
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

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.OpenChest);

        float delay = 0.3f;
        yield return new WaitForSeconds(delay);

        SetupItems();
    }

    private void SetupItems() {

        bool hasHeal = UnityEngine.Random.value < healProbability;
        int healIndex = 0;

        // if has a heal, pick a random position for it to be
        if (hasHeal) {
            healIndex = UnityEngine.Random.Range(0, ITEM_AMOUNT);
        }

        for (int itemIndex = 0; itemIndex < ITEM_AMOUNT; itemIndex++) {

            bool isHeal = hasHeal && healIndex == itemIndex;
            if (isHeal) {
                ChestHeal chestHeal = chestHealPrefab.Spawn(transform.position, chestItemContainer);
                chestHeal.Setup(this, GetItemPosition(itemIndex));

                ChestItems.Add(chestHeal);
            }
            else {
                ChestCard chestCard = chestCardPrefab.Spawn(transform.position, chestItemContainer);
                chestCard.Setup(this, GetItemPosition(itemIndex));

                CardType chosenCardType = ResourceSystem.Instance.GetRandomCardWeighted(remainingPossibleCards);
                remainingPossibleCards.Remove(chosenCardType); // so won't choose two of the same card
                chestCard.SetCard(ResourceSystem.Instance.GetCardInstance(chosenCardType));

                ChestItems.Add(chestCard);
            }
        }
    }

    private Vector2 GetItemPosition(int itemIndex) {
        if (itemIndex == 0) return new Vector2(-1f, 2.6f);
        if (itemIndex == 1) return new Vector2(0f, 3f);
        if (itemIndex == 2) return new Vector2(1f, 2.6f);
        else {
            Debug.LogError("itemIndex position not set: " + itemIndex);
            return Vector2.zero;
        }
    }

    public IEnumerator OnSelectCollectable() {

        GetComponent<CreateMapIcon>().HideMapIcon();

        float duration = 0.5f;

        yield return new WaitForSeconds(duration);

        // then hide chest
        transform.ShrinkThenDestroy();
    }
}

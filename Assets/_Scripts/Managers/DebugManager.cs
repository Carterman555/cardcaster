using QFSW.QC;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DebugManager : StaticInstance<DebugManager> {

    [SerializeField] private bool playerInvincible;

    [SerializeField] private bool unlimitedEssence;
    public bool UnlimitedEssence => unlimitedEssence;

    private void Start() {
        if (playerInvincible) {
            PlayerMovement.Instance.AddComponent<PlayerInvincibility>();
        }
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += OnRoomsGenerated;
        GameSceneManager.OnStartGameLoadingCompleted += OnStartGame;
    }

    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= OnRoomsGenerated;
        GameSceneManager.OnStartGameLoadingCompleted -= OnStartGame;
    }

    private void OnStartGame() {

        //print("DealerDefeatedAmount: " + ES3.Load("DealerDefeatedAmount", ES3EncryptionMigration.GetES3Settings()));

        GiveStartingCards();
        ApplyPlayerStatModifiers();
        ApplyStartingEnchantments();
    }

    private void Update() {
        if (printMouseOver) {
            PrintMouseOver();
        }

        if (testKnockback) {
            HandleKnockbackTesting();
        }
    }

    [SerializeField] private bool noEnemies;

    private void OnRoomsGenerated() {
        if (noEnemies) {
            ClearAllRooms();
        }

        if (spawnAtBossRoom) {
            //... wait a frame to teleport, so boss room gets entered after starting room does
            Invoke(nameof(TeleportToBossRoom), Time.deltaTime);
        }
    }

    [Command]
    private void ClearRoom() {
        Room.GetCurrentRoom().IsRoomCleared = true;

        KillAllEnemies();

        EnemySpawner.Instance.StopSpawningInRoom();

        CheckEnemiesCleared.InvokeCleared();
    }

    [Command]
    private void ClearAllRooms() {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            room.IsRoomCleared = true;
        }

        ClearRoom();
    }

    [Command("kill_enemies", MonoTargetType.All)]
    private void KillAllEnemies() {
        foreach (Transform enemy in Containers.Instance.Enemies) {
            //enemy.GetComponent<EnemyHealth>().Die();
            if (enemy.gameObject.activeSelf) {
                enemy.gameObject.ReturnToPool();
            }
        }
    }


    [SerializeField] private bool printMouseOver;

    private void PrintMouseOver() {

        // Convert mouse position to world position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Perform the 2D raycast
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null) {
            Debug.Log($"Mouse is over: {hit.collider.gameObject.name}");
        }
        else {
            Debug.Log("Mouse is not over any 2D object");
        }
    }

    [SerializeField] private List<CardType> startingCards;

    [SerializeField] private bool giveOneOfEachCard;

    private void GiveStartingCards() {
        if (startingCards.Count > 0) {

            //... remove current starting cards
            DeckManager.Instance.ClearDeckAndEssence();

            foreach (CardType cardType in startingCards) {
                ScriptableCardBase card = ResourceSystem.Instance.GetCardInstance(cardType);
                DeckManager.Instance.GainCard(card);
            }
        }

        if (giveOneOfEachCard) {
            foreach (CardType cardType in Enum.GetValues(typeof(CardType))) {
                ScriptableCardBase card = ResourceSystem.Instance.GetCardInstance(cardType);
                DeckManager.Instance.GainCard(card);
            }
        }
    }

    [SerializeField] private PlayerStatModifier[] startingPlayerStatModifiers;

    private void ApplyPlayerStatModifiers() {
        StatsManager.AddPlayerStatModifiers(startingPlayerStatModifiers);
    }

    [SerializeField] private EnchantmentType[] startingEnchantments;

    private void ApplyStartingEnchantments() {
        foreach (EnchantmentType enchantmentType in startingEnchantments) {
            StatsManager.AddEnchantment(ResourceSystem.Instance.GetEnchantment(enchantmentType));
        }
    }


    [SerializeField] private bool spawnAtBossRoom;

    [Command]
    private void TeleportToBossRoom() {
        BossRoom bossRoom = FindObjectOfType<BossRoom>();

        bossRoom.GetComponent<Room>().OnEnterRoom();

        Vector3 offset = new Vector3(-5f, 0);
        PlayerMovement.Instance.transform.position = bossRoom.GetBossSpawnPoint().position + offset;
    }


    [Command, ContextMenu("PrintUnlockedCards")]
    private void PrintUnlockedCards() {
        ResourceSystem.Instance.UnlockedCards.Print();
    }

    [Command, ContextMenu("RemoveUnlockCard")]
    private void RemoveUnlockCard(CardType cardType) {
        ResourceSystem.Instance.UnlockedCards.Remove(cardType);
        ES3.Save("UnlockedCardTypes", ResourceSystem.Instance.UnlockedCards, ES3EncryptionMigration.GetES3Settings());
    }

    [SerializeField] private bool testKnockback;

    private void HandleKnockbackTesting() {
        if (Input.GetKeyDown(KeyCode.Equals)) { // '+' key (usually shift + '=')
            ModifyKnockback(1f);
        }
        else if (Input.GetKeyDown(KeyCode.Minus)) {
            ModifyKnockback(-1f);
        }
    }

    private void ModifyKnockback(float delta) {
        PlayerStatModifier playerStatModifier = new() {
            PlayerStatType = PlayerStatType.KnockbackStrength,
            ModifyType = ModifyType.Additive,
            Value = delta
        };

        StatsManager.AddPlayerStatModifier(playerStatModifier);
        print($"Current Knockback: {StatsManager.PlayerStats.KnockbackStrength}");
    }

    [SerializeField] private EnchantmentOrb enchantmentOrbPrefab;

    [Command]
    public void SpawnOrb(Vector2 pos) {
        enchantmentOrbPrefab.Spawn(pos, Containers.Instance.Drops);
    }
}

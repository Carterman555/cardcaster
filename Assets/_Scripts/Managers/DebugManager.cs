using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class DebugManager : StaticInstance<DebugManager> {

    [SerializeField] private bool playerInvincible;

    [SerializeField] private bool unlimitedEssence;
    public bool UnlimitedEssence => unlimitedEssence;


    private void Start() {

        if (playerInvincible) {
            PlayerMovement.Instance.AddComponent<Invincibility>();
        }

        GiveStartingCards();
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += OnRoomsGenerated;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= OnRoomsGenerated;
    }

    [SerializeField] private bool noEnemies;

    private void Update() {
        if (printMouseOver) {
            PrintMouseOver();
        }
    }

    private void OnRoomsGenerated() {
        if (noEnemies) {
            ClearAllRooms();
        }

        if (spawnAtBossRoom) {
            TeleportToBossRoom();
        }
    }

    [Command]
    private void ClearRoom() {
        Room.GetCurrentRoom().SetRoomCleared();

        KillAllEnemies();

        EnemySpawnerOld.Instance.StopSpawning();

        CheckRoomCleared.InvokeCleared();
    }

    [Command]
    private void ClearAllRooms() {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            room.SetRoomCleared();
        }

        ClearRoom();
    }

    [Command("kill_enemies", MonoTargetType.All)]
    private void KillAllEnemies() {
        foreach (Transform enemy in Containers.Instance.Enemies) {
            enemy.gameObject.ReturnToPool();
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

    private void GiveStartingCards() {
        foreach (CardType cardType in startingCards) {
            ScriptableCardBase card = ResourceSystem.Instance.GetCard(cardType);
            DeckManager.Instance.GainCard(card);
        }
    }


    [SerializeField] private bool spawnAtBossRoom;

    [Command]
    private void TeleportToBossRoom() {
        BossRoom bossRoom = FindObjectOfType<BossRoom>();

        bossRoom.GetComponent<Room>().OnEnterRoom();

        Vector3 offset = new Vector3(-5f, 0);
        PlayerMeleeAttack.Instance.transform.position = bossRoom.GetBossSpawnPoint().position + offset;
    }
}

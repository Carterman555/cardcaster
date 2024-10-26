using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
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
    }

    [Command]
    private void ClearRoom() {
        print("cleared");

        Room.GetCurrentRoom().SetRoomCleared();

        foreach (GameObject enemy in Containers.Instance.Enemies) {
            enemy.ReturnToPool();
        }

        EnemySpawner.Instance.StopSpawning();

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
}

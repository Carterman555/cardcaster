using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : StaticInstance<DebugManager> {

    [SerializeField] private bool playerInvincible;

    [SerializeField] private bool unlimitedEssence;
    public bool UnlimitedEssence => unlimitedEssence;

    [SerializeField] private bool noEnemies;

    private void Start() {

        if (playerInvincible) {
            PlayerMovement.Instance.GetComponent<Health>().SetInvincible(true);
        }

        if (unlimitedEssence) {
            //DeckManager.Instance.ChangeMaxEssence(9999);
            //DeckManager.Instance.ChangeEssenceAmount(9999f);
        }
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += OnRoomsGenerated;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= OnRoomsGenerated;
    }

    private void OnRoomsGenerated() {
        if (noEnemies) {
            ClearAllRooms();
        }
    }

    [Command]
    private void ClearAllRooms() {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            room.SetRoomCleared();
        }
    }
}

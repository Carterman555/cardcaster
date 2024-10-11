using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour {

    [Command]
    private void ClearAllRooms() {
        Room[] rooms = FindObjectsOfType<Room>();
        foreach (Room room in rooms) {
            room.SetRoomCleared();
        }
    }
}

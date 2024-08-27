using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeRoom : MonoBehaviour {

    private CinemachineConfiner2D confiner;

    private void Awake() {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable() {
        Room.OnAnyRoomEnter_Room += SetRoomConfiner;
        Room.OnAnyRoomExit_Room += DisableRoomConfiner;
    }
    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= SetRoomConfiner;
        Room.OnAnyRoomExit_Room -= DisableRoomConfiner;
    }

    private void SetRoomConfiner(Room enteredRoom) {
        confiner.m_BoundingShape2D = enteredRoom.GetCameraConfiner();
        confiner.InvalidateCache();
    }

    private void DisableRoomConfiner(Room exitedRoom) {
        confiner.m_BoundingShape2D = null;
        confiner.InvalidateCache();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMapIcon : MonoBehaviour {

    [SerializeField] private SpriteRenderer chestIconPrefab;
    private SpriteRenderer mapIcon;

    private bool showingMapIcon;

    private bool childOfRoom;
    private int roomNum;

    private void OnEnable() {
        StartCoroutine(SetRoomNum());
    }

    private void OnDisable() {

        Room.OnAnyRoomEnter_Room -= TryShowMapIcon;

        if (Helpers.GameStopping()) {
            return;
        }

        HideMapIcon();
    }

    private IEnumerator SetRoomNum() {

        //... wait a frame for the room to get setup
        yield return null;

        Room room = GetComponentInParent<Room>(true);
        childOfRoom = room != null;

        if (childOfRoom) {
            roomNum = room.GetRoomNum();
            Room.OnAnyRoomEnter_Room += TryShowMapIcon;
        }
    }

    private void TryShowMapIcon(Room room) {
        //... if entered the room this object is in
        if (room.GetRoomNum() == roomNum) {
            ShowMapIcon();

            Room.OnAnyRoomEnter_Room -= TryShowMapIcon;
        }
    }

    public void ShowMapIcon() {
        if (!showingMapIcon) {
            showingMapIcon = true;
            mapIcon = chestIconPrefab.Spawn(transform.position, Containers.Instance.MapIcons);
        }
    }

    public void HideMapIcon() {
        if (showingMapIcon) {
            showingMapIcon = false;
            mapIcon.gameObject.ReturnToPool();
        }
    }
}

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hallway : MonoBehaviour {

    [SerializeField] private Light2D hallwayLight;

    private int[] connectingRoomNums = new int[2];

    [SerializeField] private SpriteRenderer mapIconToSpawn;
    private SpriteRenderer mapIcon;

    private void OnEnable() {
        connectingRoomNums = new int[2];
        hallwayLight.intensity = 0f;

        SetupMapIcon();

        Room.OnAnyRoomEnter_Room += TryLightPartially;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryLightPartially;
    }

    // spawn the map icon as a child of LevelMapIcons so LevelMapIcons can create a unified outline around all the
    // rooms and hallways
    private void SetupMapIcon() {
        mapIcon = mapIconToSpawn.Spawn(mapIconToSpawn.transform.position, Containers.Instance.LevelMapIcons);
        //miniMapIcon.Fade(0f);
        mapIcon.Fade(1f);

        mapIconToSpawn.enabled = false;
    }

    private void TryLightPartially(Room room) {

        bool alreadyLit = hallwayLight.intensity > 0f;
        if (alreadyLit) {
            return;
        }

        //... if entered a room connecting to this hallway
        if (connectingRoomNums.Contains(room.GetRoomNum())) {
            LightPartially();

            //... show room on minimap
            LevelMapIcons.Instance.ShowMapIcon(mapIcon);
        }
    }

    public void SetConnectingRoomNums(int roomNum1, int roomNum2) {
        connectingRoomNums = new int[] { roomNum1, roomNum2 };

        mapIcon.name = "HallwayMapIcon" + roomNum1 + "-" + roomNum2;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == GameLayers.PlayerLayer && enabled) {
            LightFully();
        }
    }

    private void LightPartially() {
        DOTween.To(() => hallwayLight.intensity, x => hallwayLight.intensity = x, 0.2f, duration: 0.5f);
    }

    private void LightFully() {
        DOTween.To(() => hallwayLight.intensity, x => hallwayLight.intensity = x, 1, duration: 0.5f);
    }

}

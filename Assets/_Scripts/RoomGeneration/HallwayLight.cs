using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HallwayLight : MonoBehaviour {

    private Light2D hallwayLight;

    private int[] connectingRoomNums = new int[2];

    private void Awake() {
        hallwayLight = GetComponent<Light2D>();
    }

    private void OnEnable() {
        connectingRoomNums = new int[2];
        hallwayLight.intensity = 0f;

        Room.OnAnyRoomEnter_Room += TryLightPartially;
    }

    private void OnDisable() {
        Room.OnAnyRoomEnter_Room -= TryLightPartially;
    }

    private void TryLightPartially(Room room) {

        bool alreadyLit = hallwayLight.intensity > 0f;
        if (alreadyLit) {
            return;
        }

        //... if the room entered is a connecting room
        if (connectingRoomNums.Contains(room.GetRoomNum())) {
            LightPartially();
        }
    }

    public void SetConnectingRoomNums(int roomNum1, int roomNum2) {
        connectingRoomNums = new int[] { roomNum1, roomNum2 };
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

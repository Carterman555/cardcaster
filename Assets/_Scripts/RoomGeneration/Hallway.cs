using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hallway : MonoBehaviour {

    [SerializeField] private Light2D hallwayLight;

    private int[] connectingRoomNums = new int[2];

    [SerializeField] private Sprite mapIcon;
    public Sprite MapIcon => mapIcon;

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

        //... if entered a room connecting to this hallway
        if (connectingRoomNums.Contains(room.GetRoomNum())) {
            LightPartially();

            MinimapManager.Instance.StartCoroutine(MinimapManager.Instance.ShowRoomOrHallIcon(transform));
        }
    }

    public void SetConnectingRoomNums(int roomNum1, int roomNum2) {
        connectingRoomNums = new int[] { roomNum1, roomNum2 };

        mapIcon.name = "HallwayMapIcon" + roomNum1 + "-" + roomNum2;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (GameLayers.AllPlayerLayerMask.ContainsLayer(collision.gameObject.layer) && enabled) {
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

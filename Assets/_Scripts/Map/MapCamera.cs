using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapCamera : MonoBehaviour {

    private Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += ResizeAndPositionMap;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= ResizeAndPositionMap;
    }

    private void ResizeAndPositionMap() {
        CenterCamera();

        FitCameraSizeToLevel();
    }

    private void CenterCamera() {
        Transform[] rooms = FindObjectsOfType<Room>().Select(r => r.transform).ToArray();

        float maxX = rooms.Max(r => r.position.x);
        float maxY = rooms.Max(r => r.position.y);

        float minX = rooms.Min(r => r.position.x);
        float minY = rooms.Min(r => r.position.y);

        float middleX = (maxX + minX) / 2;
        float middleY = (maxY + minY) / 2;

        Vector3 centerOfRooms = new Vector3(middleX, middleY, -10);

        transform.position = centerOfRooms;

        print("Length: " + (maxX - minX));
        print("Height: " + (maxY - minY));
    }

    private void FitCameraSizeToLevel() {




        //cam.orthographicSize = ;
    }
}

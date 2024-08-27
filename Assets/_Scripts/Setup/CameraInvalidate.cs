using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInvalidate : MonoBehaviour {

    private CinemachineConfiner2D confiner;

    private void Awake() {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += InvalidateCache;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= InvalidateCache;
    }

    private void InvalidateCache() {
        confiner.InvalidateCache();
    }
}

using Cinemachine;
using MoreMountains.Tools;
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
        Tutorial.OnTutorialRoomStart += InvalidateCache;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= InvalidateCache;
        Tutorial.OnTutorialRoomStart -= InvalidateCache;
    }

    private void InvalidateCache() {
        confiner.InvalidateCache();

        print("InvalidateCache");

        RemoveConfinerBoxCollider();

    }

    // it needs a box collider to prevent the camera from glitch when the rooms get spawned, but it needs to be destroyed to confine the
    // camera properly
    private void RemoveConfinerBoxCollider() {
        Destroy(ReferenceSystem.Instance.CameraConfiner.GetComponent<BoxCollider2D>());
    }

    private IEnumerator LoadWhileBaking() {

        yield return new WaitForSeconds(5f);

        MMAdditiveSceneLoadingManager.AllowUnload();

    }
}

using Cinemachine;
using MoreMountains.Tools;
using System;
using System.Collections;
using UnityEngine;

public class CameraInvalidate : MonoBehaviour {

    public static event Action OnFinishLoading;

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

        RemoveConfinerBoxCollider();
        StartCoroutine(LoadWhileBaking());
    }

    // it needs a box collider to prevent the camera from glitch when the rooms get spawned, but it needs to be destroyed to confine the
    // camera properly
    private void RemoveConfinerBoxCollider() {
        Destroy(ReferenceSystem.Instance.CameraConfiner.GetComponent<BoxCollider2D>());
    }

    private IEnumerator LoadWhileBaking() {

        GetComponent<CameraLookInfluence>().enabled = false;

        //... unnessecary i think, but just in case camera is close to player for the first few frames, before moving away
        yield return new WaitForSeconds(0.1f);

        // wait until camera gets close enough to player before unloading the scene
        float distanceThreshold = 2f;
        float distanceThresholdSquared = distanceThreshold * distanceThreshold;

        float xDiff = PlayerMovement.Instance.CenterPos.x - Camera.main.transform.position.x;
        float yDiff = PlayerMovement.Instance.CenterPos.y - Camera.main.transform.position.y;
        float distanceSquared = xDiff * xDiff + yDiff * yDiff;

        while (distanceSquared > distanceThresholdSquared) {
            yield return null;

            xDiff = PlayerMovement.Instance.CenterPos.x - Camera.main.transform.position.x;
            yDiff = PlayerMovement.Instance.CenterPos.y - Camera.main.transform.position.y;
            distanceSquared = xDiff * xDiff + yDiff * yDiff;
        }

        GetComponent<CameraLookInfluence>().enabled = true;

        MMAdditiveSceneLoadingManager.AllowUnload();
        OnFinishLoading?.Invoke();
    }
}

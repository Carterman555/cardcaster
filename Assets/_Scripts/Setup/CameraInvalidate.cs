using Cinemachine;
using MoreMountains.Tools;
using System;
using System.Collections;
using UnityEngine;

public class CameraInvalidate : MonoBehaviour {

    private CinemachineConfiner2D confiner;

    [SerializeField] private CompositeCollider2D allRoomsCollider;
    [SerializeField] private CompositeCollider2D firstRoomsCollider;

    private void Awake() {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += QuickThenFullBake;
        Tutorial.OnTutorialRoomStart += FullBake;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= QuickThenFullBake;
        Tutorial.OnTutorialRoomStart -= FullBake;
    }

    private void QuickThenFullBake() {
        StartCoroutine(QuickThenFullBakeCor());
    }

    private IEnumerator QuickThenFullBakeCor() {
        confiner.m_BoundingShape2D = firstRoomsCollider;
        confiner.InvalidateCache();

        float maxQuickBakeTime = 0.25f;
        yield return new WaitForSeconds(maxQuickBakeTime);

        Vector3 camPos = new Vector3(PlayerMovement.Instance.CenterPos.x, PlayerMovement.Instance.CenterPos.y, -10f);
        confiner.ForceCameraPosition(camPos, Quaternion.identity);

        confiner.m_BoundingShape2D = allRoomsCollider;
        confiner.InvalidateCache();
    }

    private void FullBake() {
        confiner.m_BoundingShape2D = allRoomsCollider;
        confiner.InvalidateCache();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMapInTutorial : MonoBehaviour
{
    [SerializeField] private GameObject openMapPlayer;

    private void OnEnable() {
        Tutorial.OnTutorialRoomStart += DisableMap;
    }

    private void OnDisable() {
        Tutorial.OnTutorialRoomStart -= DisableMap;
    }

    private void DisableMap() {
        openMapPlayer.SetActive(false);
        gameObject.SetActive(false);
    }
}

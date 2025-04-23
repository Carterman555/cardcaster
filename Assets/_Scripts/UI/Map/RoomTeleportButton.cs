using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTeleportButton : MonoBehaviour {

    private Vector2 roomPos;
    private bool setup;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void OnEnable() {
        button.onClick.AddListener(CloseMap);
    }

    private void OnDisable() {
        button.onClick.RemoveListener(CloseMap);
        setup = false;
    }

    public void SetRoom(Transform room) {
        roomPos = room.position;

        setup = true;
    }

    public void CloseMap() {
        if (!setup) {
            Debug.LogError("Tried to teleport to room before setup!");
            return;
        }

        MMF_Player toggleMapPlayer = FeedbackPlayerReference.GetPlayer("ToggleMap");
        toggleMapPlayer.PlayFeedbacks();
        toggleMapPlayer.Events.OnComplete.AddListener(FadeOutPlayer);
    }

    private void FadeOutPlayer() {
        MMF_Player toggleMapPlayer = FeedbackPlayerReference.GetPlayer("ToggleMap");
        toggleMapPlayer.Events.OnComplete.RemoveListener(FadeOutPlayer);

        Animator anim = PlayerFadeManager.Instance.GetComponent<Animator>();
        anim.SetTrigger("teleportFade");

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);

        PlayerFadeManager.OnTeleportFadedOut += TeleportPlayer;
    }

    private void TeleportPlayer() {
        PlayerFadeManager.OnTeleportFadedOut -= TeleportPlayer;

        PlayerMovement.Instance.transform.position = roomPos;

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);
    }
}

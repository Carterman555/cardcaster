using UnityEngine;

public class BossIcon : MonoBehaviour {

    [SerializeField] private SpriteRenderer bossIcon;
    [SerializeField] private TriggerContactTracker showIconTrigger;

    private void OnEnable() {
        bossIcon.enabled = false;

        showIconTrigger.OnEnterContact += ShowIcon;
        Room.OnAnyRoomEnter_Room += TryHideIcon;
    }

    private void OnDisable() {
        showIconTrigger.OnEnterContact -= ShowIcon;
        Room.OnAnyRoomEnter_Room -= TryHideIcon;
    }

    private void TryHideIcon(Room room) {
        if (room.TryGetComponent(out BossRoom bossRoom)) {
            HideIcon();
        }
    }

    private void ShowIcon() {
        bossIcon.enabled = true;
        showIconTrigger.OnEnterContact -= ShowIcon;
    }

    private void HideIcon() {
        bossIcon.enabled = false;

        showIconTrigger.OnEnterContact -= ShowIcon;
        Room.OnAnyRoomEnter_Room -= TryHideIcon;
    }
}

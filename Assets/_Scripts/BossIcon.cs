using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIcon : MonoBehaviour {

    [SerializeField] private SpriteRenderer bossIcon;
    [SerializeField] private TriggerContactTracker showIconTrigger;

    private void OnEnable() {
        bossIcon.enabled = false;

        showIconTrigger.OnEnterContact += ShowIcon;
    }

    private void OnDisable() {
        showIconTrigger.OnEnterContact -= ShowIcon;
    }

    private void ShowIcon() {
        showIconTrigger.OnEnterContact -= ShowIcon;

        bossIcon.enabled = true;
    }

    private void OnBossRoomEntered() {
        bossIcon.enabled = false;
        showIconTrigger.OnEnterContact -= ShowIcon;
    }
}

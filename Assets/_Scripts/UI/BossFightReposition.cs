using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightReposition : MonoBehaviour {

    private Vector2 originalPos;

    [SerializeField] private bool moveX;
    [SerializeField, ConditionalHide("moveX")] private float bossXPos;

    [SerializeField] private bool moveY;
    [SerializeField, ConditionalHide("moveY")] private float bossYPos;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
    }

    private void OnEnable() {
        BossManager.OnStartBossFight += MoveToBossPos;
        BossManager.OnBossKilled += MoveToOriginalPos;
    }

    private void OnDisable() {
        BossManager.OnStartBossFight -= MoveToBossPos;
        BossManager.OnBossKilled -= MoveToOriginalPos;
    }

    private void MoveToBossPos() {
        Vector2 bossPos = originalPos;

        if (moveX) {
            bossPos.x = bossXPos;
        }
        if (moveY) {
            bossPos.y = bossYPos;
        }

        rectTransform.DOAnchorPos(bossPos, duration: 0.3f);
    }

    private void MoveToOriginalPos() {
        rectTransform.DOAnchorPos(originalPos, duration: 0.3f);
    }
}

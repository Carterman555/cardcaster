using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowardsPlayer : MonoBehaviour {

    [SerializeField] private float angleOffset;
    private Vector2 originalDirection;

    private void Awake() {
        originalDirection = transform.up;
    }

    void Update() {
        Vector2 toPlayer = PlayerMovement.Instance.transform.position - transform.position;
        transform.up = toPlayer.RotateDirection(angleOffset);
    }
}

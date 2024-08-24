using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowardsPlayer : MonoBehaviour {

    private Vector2 originalDirection;

    private void Awake() {
        originalDirection = transform.up;
    }

    void Update() {
        Vector2 toPlayer = PlayerMovement.Instance.transform.position - transform.position;
        transform.up = toPlayer;
    }

    private void OnDisable() {
        float duration = 0.3f;
        transform.DORotateQuaternion(Quaternion.LookRotation(Vector3.forward, originalDirection), duration);
    }
}

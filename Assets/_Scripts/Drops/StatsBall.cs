using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsBall : MonoBehaviour {

    [SerializeField] private float destinationObstacleDistance;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1f);

        Vector2 destination = FindDestination();
        transform.DOMove(destination, duration: 1f).OnComplete(() => {
            
        });
    }

    // move to a location away from obstacles where the player will be able to choose stat
    private Vector2 FindDestination() {

        if (IsValidPos(transform.position)) {
            return transform.position;
        }

        int maxIterations = 500;
        int iterationCounter = 0;

        float searchDistanceIncrement = 0.5f;
        float maxSearchDistance = 5f;
        float currentSearchDistance = 0.5f;

        while (currentSearchDistance <= maxSearchDistance) {

            float searchAngleIncrement = 20f;
            float currentSearchAngle = 0f;
            while (currentSearchAngle <= 360) {

                Vector2 currentDirection = Vector2.up.GetDirectionRotated(currentSearchAngle);
                Vector2 currentPos = (Vector2)transform.position + (currentDirection * currentSearchDistance);

                if (IsValidPos(currentPos)) {
                    return currentPos;
                }

                currentSearchAngle += searchAngleIncrement;

                iterationCounter++;
                if (iterationCounter >= maxIterations) {
                    Debug.LogError("Max iterations reached!");
                    return transform.position;
                }
            }

            currentSearchDistance += searchDistanceIncrement;
        }

        Debug.LogWarning("No valid positions found!");
        return transform.position;
    }

    private bool IsValidPos(Vector2 pos) {
        Collider2D col = Physics2D.OverlapCircle(pos, destinationObstacleDistance, GameLayers.ObstacleLayerMask);
        return col == null;
    }
}

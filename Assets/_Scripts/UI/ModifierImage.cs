using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierImage : MonoBehaviour {

    [SerializeField] private Vector2 firstModifierPos;
    [SerializeField] private float modifierSpacing;

    private void OnEnable() {
        MoveToModifierPosition();
    }

    private void MoveToModifierPosition() {

        Vector2 targetPosition = firstModifierPos;
        targetPosition += new Vector2(AbilityManager.Instance.ActiveModifierCount() * modifierSpacing, 0f);

        float moveDuration = 0.3f;
        transform.DOMove(targetPosition, moveDuration).OnComplete(() => {
            transform.SetParent(Containers.Instance.ActiveModifierImages);
            print(transform.position);
        });
    }
}

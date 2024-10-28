using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour {

    private TextMeshPro text;

    [Header("Color")]
    [SerializeField] private Color lowDamageColor;
    [SerializeField] private float mediumDamageThreshold;
    [SerializeField] private Color mediumDamageColor;
    [SerializeField] private float highDamageThreshold;
    [SerializeField] private Color highDamageColor;

    [Header("Animate")]
    [SerializeField] private Vector2 moveAmount;

    private void Awake() {
        text = GetComponent<TextMeshPro>();
    }

    public void Setup(float damage) {

        if (damage < 1f) {
            // round to nearest tenths place
            damage = Mathf.Round(damage * 10f) / 10f;
        }
        else {
            damage = Mathf.Round(damage);
        }


        text.text = damage.ToString();

        // set color based on damage
        if (damage > highDamageThreshold) {
            text.color = highDamageColor;
        }
        else if (damage > mediumDamageThreshold) {
            text.color = mediumDamageColor;
        }
        else {
            text.color = lowDamageColor;
        }

        // move and fade
        float duration = 0.5f;

        Vector2 targetPos = (Vector2)transform.position + moveAmount;
        transform.DOMove(targetPos, duration);

        text.Fade(1f);
        text.DOFade(0f, duration).OnComplete(() => {
            gameObject.ReturnToPool();
        });
    }
}

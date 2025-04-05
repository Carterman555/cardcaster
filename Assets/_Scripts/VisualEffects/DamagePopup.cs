using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour {

    private TextMeshPro text;

    [Header("Color")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color critColor;

    [Header("Animate")]
    [SerializeField] private Vector2 moveAmount;

    private void Awake() {
        text = GetComponent<TextMeshPro>();
    }

    public void Setup(float damage, bool crit) {

        if (damage < 1f) {
            // round to nearest tenths place
            damage = Mathf.Round(damage * 10f) / 10f;
        }
        else {
            damage = Mathf.Round(damage);
        }

        text.text = damage.ToString();
        text.color = crit ? critColor : normalColor;

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

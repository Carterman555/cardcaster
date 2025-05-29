using DG.Tweening;
using UnityEngine;

public class CardSurge : MonoBehaviour {

    public enum TargetType { Player, Random }

    [Header("Warning")]
    [SerializeField] private SpriteRenderer warningSpriteRenderer;
    [SerializeField] private float warningTime;

    [Header("Surge Action")]
    [SerializeField] private ParticleSystem surgeEffect;
    [SerializeField] private StraightMovement cardProjectile;
    [SerializeField] private float damage;

    public void Setup(TargetType targetType) {
        SetupPositionAndRotation(targetType);

        FadeInWarningVisual();

        cardProjectile.gameObject.SetActive(false);

        Invoke(nameof(FadeOutWarningVisual), warningTime);
        Invoke(nameof(SurgeCard), warningTime);
    }

    private void SetupPositionAndRotation(TargetType targetType) {
        Vector2 targetPoint = Vector2.zero;

        if (targetType == TargetType.Player) {
            targetPoint = PlayerMovement.Instance.CenterPos;
        }
        else if (targetType == TargetType.Random) {
            targetPoint = new RoomPositionHelper().GetRandomRoomPos();
        }

        transform.position = targetPoint;

        //... choose random direction
        float angle = Random.Range(0f, 360f);

        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    private void FadeInWarningVisual() {
        float fade = 50f / 255f;
        warningSpriteRenderer.Fade(0);
        warningSpriteRenderer.DOFade(fade, duration: 0.3f);
    }

    private void FadeOutWarningVisual() {
        float fade = 50f / 255f;
        warningSpriteRenderer.Fade(fade);
        warningSpriteRenderer.DOFade(0, duration: 0.3f);
    }


    private void SurgeCard() {
        surgeEffect.Play();

        // setup projectile
        cardProjectile.gameObject.SetActive(true);

        cardProjectile.transform.localPosition = new Vector3(0f, -25f);

        Vector2 direction = transform.eulerAngles.z.RotationToDirection();
        cardProjectile.Setup(direction);

        cardProjectile.GetComponent<DamageOnContact>().Setup(damage, knockbackStrength: 1f);

        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.DeckOfDoomSurge);
    }
}


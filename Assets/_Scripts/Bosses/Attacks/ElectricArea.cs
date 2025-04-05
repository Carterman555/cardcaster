using DG.Tweening;
using UnityEngine;

public class ElectricArea : MonoBehaviour {

    [SerializeField] private SpriteRenderer visual;

    [Header("Warning")]
    [SerializeField] private float warningTime;
    private float warningTimer;

    [SerializeField, Range(0f, 1f)] private float warningFadeAmount;

    [Header("Activate")]
    private bool active;

    [SerializeField] private float activeTime;
    private float activeTimer;

    [SerializeField, Range(0f, 1f)] private float activeFadeAmount;

    [SerializeField] private ParticleSystem electricParticles;

    [SerializeField] private Collider2D touchDamageTrigger;

    private GameObject loopAudioSourceGO;

    private void OnEnable() {
        visual.Fade(0f);
        visual.DOFade(warningFadeAmount, duration: 0.3f);

        electricParticles.Stop();

        active = false;
        warningTimer = 0;
        activeTimer = 0;

        touchDamageTrigger.enabled = false;
    }

    private void Update() {

        if (!active) {
            warningTimer += Time.deltaTime;
            if (warningTimer > warningTime) {
                visual.DOFade(activeFadeAmount, duration: 0.3f);

                electricParticles.Play();

                active = true;
                touchDamageTrigger.enabled = true;

                loopAudioSourceGO = AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.ThunderGolemArea, uiSound: false, loop: true);
            }
        }
        else {
            activeTimer += Time.deltaTime;
            if (activeTimer > activeTime) {
                electricParticles.Stop();

                activeTimer = 0f;
                touchDamageTrigger.enabled = false;

                loopAudioSourceGO.GetComponent<AudioSource>().DOFade(0f, duration: 0.3f);
                visual.DOFade(0f, duration: 0.3f).OnComplete(() => {
                    gameObject.ReturnToPool();
                    loopAudioSourceGO.ReturnToPool();
                });
            }
        }
    }
}

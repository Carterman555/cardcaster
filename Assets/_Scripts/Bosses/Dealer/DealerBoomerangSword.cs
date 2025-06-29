using DG.Tweening;
using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class DealerBoomerangSword : MonoBehaviour {

    private BoomerangMovement boomerangMovement;
    private MMAutoRotate autoRotate;
    private SpriteRenderer spriteRenderer;
    private EnemyTouchDamage touchDamage;

    private Transform dealerCenterTransform;
    private float delayBeforeShoot;
    private float startingSpeed;
    private float acceleration;

    [SerializeField] private ParticleSystem destroyParticles;

    [SerializeField] private AudioClips swingSfx;
    [SerializeField] private AudioClips shootSfx;
    private GameObject swingAudioSource;

    private void Awake() {
        boomerangMovement = GetComponent<BoomerangMovement>();
        autoRotate = GetComponent<MMAutoRotate>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        touchDamage = GetComponent<EnemyTouchDamage>();
    }

    private void OnEnable() {
        boomerangMovement.enabled = false;
        boomerangMovement.OnReturn += ReturnThis;

        touchDamage.enabled = false;
    }

    public void Setup(Transform dealerCenterTransform, bool orbiting, int orbitDirection, float delayBeforeShoot, float startingSpeed, float acceleration) {
        this.dealerCenterTransform = dealerCenterTransform;
        this.delayBeforeShoot = delayBeforeShoot;
        this.startingSpeed = startingSpeed;
        this.acceleration = acceleration;

        transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
        autoRotate.Orbiting = orbiting;
        autoRotate.OrbitCenterTransform = dealerCenterTransform;

        if (orbiting) {
            float orbitSpeed = Mathf.Abs(autoRotate.OrbitRotationSpeed);
            autoRotate.OrbitRotationSpeed = orbitSpeed * orbitDirection;
        }

        spriteRenderer.Fade(0f);
        spriteRenderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
            touchDamage.enabled = true;
        });

        swingAudioSource = AudioManager.Instance.PlaySound(swingSfx, loop: true);

        StartCoroutine(DelayedShoot());
    }

    private IEnumerator DelayedShoot() {
        yield return new WaitForSeconds(delayBeforeShoot);

        autoRotate.Orbiting = false;

        boomerangMovement.enabled = true;
        Vector2 awayFromDealerDirection = (transform.position - dealerCenterTransform.position).normalized;
        boomerangMovement.Setup(dealerCenterTransform, awayFromDealerDirection, startingSpeed, acceleration);

        AudioManager.Instance.PlaySound(shootSfx);
    }

    private void ReturnThis(BoomerangMovement movement) {
        gameObject.ReturnToPool();
    }

    private void OnDisable() {
        boomerangMovement.OnReturn -= ReturnThis;

        if (!Helpers.GameStopping()) {
            ParticleSystem newDestroyParticles = destroyParticles.Spawn(transform.position, transform.rotation, Containers.Instance.Effects);
            newDestroyParticles.Play();
        }

        swingAudioSource.ReturnToPool();
    }
}

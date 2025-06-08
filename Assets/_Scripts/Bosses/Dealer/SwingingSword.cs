using DG.Tweening;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingSword : MonoBehaviour {

    [SerializeField] private SpriteRenderer sword1Renderer;
    [SerializeField] private EnemyTouchDamage sword1TouchDamage;
    [SerializeField] private StraightShooter sword1Shooter;

    [SerializeField] private SpriteRenderer sword2Renderer;
    [SerializeField] private EnemyTouchDamage sword2TouchDamage;
    [SerializeField] private StraightShooter sword2Shooter;

    private bool secondSwordActive;

    public void Setup(bool secondSwordActive, float spinSpeed) {
        this.secondSwordActive = secondSwordActive;

        MMAutoRotate autoRotate = GetComponent<MMAutoRotate>();
        autoRotate.enabled = false;
        autoRotate.RotationSpeed = new(0f, 0f, spinSpeed);

        sword1TouchDamage.enabled = false;
        sword1Shooter.enabled = false;

        sword1Renderer.Fade(0f);
        sword1Renderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
            sword1TouchDamage.enabled = true;
            sword1Shooter.enabled = true;

            autoRotate.enabled = true;
        });


        sword2Renderer.gameObject.SetActive(secondSwordActive);

        if (secondSwordActive) {
            sword2TouchDamage.enabled = false;
            sword2Shooter.enabled = false;

            sword2Renderer.Fade(0f);
            sword2Renderer.DOFade(1f, duration: 1.5f).OnComplete(() => {
                sword2TouchDamage.enabled = true;
                sword2Shooter.enabled = true;
            });
        }
    }

    public void FadeOut() {
        sword1TouchDamage.enabled = false;
        sword1Shooter.enabled = false;

        sword1Renderer.DOFade(0f, duration: 0.5f).OnComplete(() => {
            gameObject.ReturnToPool();
        });

        if (secondSwordActive) {
            sword2TouchDamage.enabled = false;
            sword2Shooter.enabled = false;

            sword2Renderer.DOFade(0f, duration: 0.5f);
        }
    }
}

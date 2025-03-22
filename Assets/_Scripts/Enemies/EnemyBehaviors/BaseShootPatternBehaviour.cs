using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseShootPatternBehaviour : MonoBehaviour, IAttacker {

    public event Action OnAttack;

    protected IHasCommonStats hasStats;

    [SerializeField] protected Transform shootPoint;

    [SerializeField] private bool overrideDamage;
    [ConditionalHide("overrideDamage")][SerializeField] private float damage;

    protected float Damage => overrideDamage ? damage : hasStats.CommonStats.Damage;

    [Header("SFX")]
    [SerializeField] private bool customSFX;
    [ConditionalHide("customSFX")][SerializeField] private AudioClips shootSFX;

    protected virtual void Awake() {
        hasStats = GetComponent<IHasCommonStats>();
    }

    public virtual void ShootProjectile() {
        PlaySFX();

        OnAttack?.Invoke();
    }

    private void PlaySFX() {
        if (customSFX) {
            AudioManager.Instance.PlaySound(shootSFX);
        }
        else {
            AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.BasicEnemyShoot);
        }
    }
}

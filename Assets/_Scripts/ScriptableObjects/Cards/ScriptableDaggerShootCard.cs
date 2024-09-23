using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "DaggerShootCard", menuName = "Cards/Dagger Shoot Card")]
public class ScriptableDaggerShootCard : ScriptableAbilityCardBase {

    [SerializeField] private StraightMovement daggerPrefab;
    [SerializeField] private float spawnOffsetValue;

    private GameObject effectPrefab;
    private Transform visualEffect;

    private Coroutine shootCoroutine;

    public override void Play(Vector2 position) {
        base.Play(position);

        shootCoroutine = DeckManager.Instance.StartCoroutine(ShootDaggers());
    }

    public override void Stop() {
        base.Stop();

        DeckManager.Instance.StopCoroutine(shootCoroutine);
    }

    

    private IEnumerator ShootDaggers() {
        while (true) {
            yield return new WaitForSeconds(Stats.Cooldown);

            // get direction to shoot (towards mouse
            Vector2 toMouseDirection = (MouseTracker.Instance.transform.position - PlayerMovement.Instance.transform.position).normalized;
            Vector2 offset = spawnOffsetValue * toMouseDirection;
            Vector2 spawnPos = (Vector2)PlayerMovement.Instance.transform.position + offset;

            // spawn and setup dagger
            StraightMovement straightMovement = daggerPrefab.Spawn(spawnPos, Containers.Instance.Projectiles);
            straightMovement.Setup(toMouseDirection, Stats.ProjectileSpeed);
            straightMovement.GetComponent<DamageOnContact>().Setup(Stats.Damage, Stats.KnockbackStrength);

            // apply effect
            TryApplyEffects(straightMovement);
        }
    }

    #region Effects

    public override void AddEffect(GameObject effectPrefab) {
        base.AddEffect(effectPrefab);
        this.effectPrefab = effectPrefab;
    }

    public override void TryApplyVisualEffect(Transform visualEffect) {
        base.TryApplyVisualEffect(visualEffect);
        this.visualEffect = visualEffect;
    }

    // applies the effects set by the modifier
    private void TryApplyEffects(StraightMovement straightMovement) {

        if (visualEffect != null) {
            visualEffect.Spawn(straightMovement.transform);
        }
        if (effectPrefab != null) {
            var effect = Instantiate(effectPrefab, straightMovement.transform).GetComponent<IAbilityEffect>();
        }
    }

    #endregion
}

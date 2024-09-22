using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Shockwave Card")]
public class ScriptableHitShockwave : ScriptableCardBaseOld {

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private float radius = 0.75f;
    [SerializeField] private float damage = 1;
    [SerializeField] private float knockbackStrength = 1;

    [SerializeField] private ParticleSystem shockwaveEffectPrefab;

    public override void Play(Vector2 position) {
        base.Play(position);

        PlayerMeleeAttack.OnAttack_Position += CreateShockwave;
    }

    public override void Stop() {
        base.Stop();

        PlayerMeleeAttack.OnAttack_Position -= CreateShockwave;
    }

    private void CreateShockwave(Vector2 position) {
        DamageDealer.DealCircleDamage(enemyLayerMask, position, radius, damage, knockbackStrength);

        ParticleSystem effect = shockwaveEffectPrefab.Spawn(position, Containers.Instance.Effects);
        effect.Play();
    }

}

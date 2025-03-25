using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEffectOnDamage : MonoBehaviour, IAbilityEffect {

    [SerializeField] private ParticleSystem electricAbilityParticlesPrefab;

    [SerializeField] private float damage;
    [SerializeField] private int chainSize;

    private EffectOnDamage effectOnDamage;

    private void OnEnable() {
        StartCoroutine(OnEnableCor());
    }

    private IEnumerator OnEnableCor() {

        //... wait a frame before checking for attackers because an attacker could have been added
        //... in the same frame. Like if they were both added from an ability card. This makes sure
        //... this subscribes to the attackers' onDamage event
        yield return null;

        effectOnDamage = new(Shock, transform.parent, electricAbilityParticlesPrefab);
    }

    private void OnDisable() {
        effectOnDamage.Disable();
    }

    private void Shock(GameObject target) {
        if (target.TryGetComponent(out IEffectable effectable)) {
            ElectricChain electricChain = target.AddComponent<ElectricChain>();

            float dmg = damage;
            if (target.layer == GameLayers.EnemyLayer) {
                dmg *= StatsManager.Instance.PlayerStats.AllDamageMult;
            }

            electricChain.Setup(dmg, chainSize);
        }
    }
}

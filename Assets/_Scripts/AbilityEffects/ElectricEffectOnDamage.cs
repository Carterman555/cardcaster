using System.Collections;
using UnityEngine;

public class ElectricEffectOnDamage : MonoBehaviour, IAbilityEffect {

    [SerializeField] private float damage;
    [SerializeField] private int chainSize;

    private ITargetAttacker[] attackers = new ITargetAttacker[0];

    private void OnEnable() {
        StartCoroutine(OnEnableCor());
    }

    private IEnumerator OnEnableCor() {

        //... wait a frame before checking for attackers because an attacker could have been added
        //... in the same frame. Like if they were both added from an ability card. This makes sure
        //... this subscribes to the attackers' onDamage event
        yield return null;

        attackers = transform.parent.GetComponentsInChildren<ITargetAttacker>();
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target += Shock;
        }
    }

    private void OnDisable() {
        foreach (ITargetAttacker attacker in attackers) {
            attacker.OnDamage_Target -= Shock;
        }
    }

    private void Shock(GameObject target) {

        if (target.TryGetComponent(out IDamagable damagable) && damagable.Dead) {
            return;
        }

        if (target.TryGetComponent(out IEffectable effectable)) {
            ElectricChain electricChain = target.AddComponent<ElectricChain>();

            float dmg = damage;
            if (target.layer == GameLayers.EnemyLayer) {
                dmg *= StatsManager.PlayerStats.AllDamageMult;
            }

            electricChain.Setup(dmg, chainSize);
        }
    }
}

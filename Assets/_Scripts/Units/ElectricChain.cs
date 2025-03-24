using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricChain : MonoBehaviour {

    private float damage;
    private int electricityLeft;

    public void Setup(float damage, int electricityLeft) {
        this.damage = damage;
        this.electricityLeft = electricityLeft;

        StartCoroutine(SpreadElectricity());

        // effects
        AssetSystem.Instance.UnitElectricityParticles.Spawn(transform);
    }

    // if the electricity hasn't run out and the target enemy isn't already getting electrified, spread
    // the electricity to them and damage them
    private IEnumerator SpreadElectricity() {

        float spreadDelay = 0.15f;
        yield return new WaitForSeconds(spreadDelay);

        if (electricityLeft > 0) {
            float spreadReach = 2f;
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, spreadReach, GameLayers.EnemyLayerMask);

            foreach (Collider2D col in cols) {
                if (!col.TryGetComponent(out ElectricChain _electricChain)) {
                    ElectricChain electricChain = col.AddComponent<ElectricChain>();
                    electricChain.Setup(damage, electricityLeft - 1);

                    DamageDealer.TryDealDamage(col.gameObject, transform.position, damage, 0, canCrit: true);

                    // only spread one on other enemy
                    break;
                }
            }
        }

        // the shock doesn't spread to enemies with this component, so delaying the removal
        // acts as a cooldown to be shocked again
        float delayToShockAgain = 0.5f;
        yield return new WaitForSeconds(delayToShockAgain);

        Destroy(this);
    }

    // when the enemy gets set inactive, like dies, remove this component
    private void OnDisable() {
        Destroy(this);
    }
}

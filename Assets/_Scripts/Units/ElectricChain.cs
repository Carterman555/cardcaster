using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElectricChain : MonoBehaviour {

    private int electricityLeft;

    public void Setup(int electricityLeft) {
        this.electricityLeft = electricityLeft;

        StartCoroutine(SpreadElectricity());

        // effects
        AssetSystem.Instance.UnitElectricityParticles.Spawn(transform);
    }

    private IEnumerator SpreadElectricity() {

        float spreadDelay = 0.1f;
        yield return new WaitForSeconds(spreadDelay);

        if (electricityLeft > 0) {
            float spreadReach = 2f;
            Collider2D col = Physics2D.OverlapCircle(transform.position, spreadReach, GameLayers.EnemyLayerMask);
            ElectricChain electricChain = col.AddComponent<ElectricChain>();
            electricChain.Setup(electricityLeft - 1);
        }
    }

    // when the enemy gets set inactive, like dies, remove this component
    private void OnDisable() {
        Destroy(this);
    }
}

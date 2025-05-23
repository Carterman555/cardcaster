using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChargeBehavior))]
public class SplitOnChargeBehavior : MonoBehaviour {

    [SerializeField] private Enemy splitEnemyPrefab;

    private ChargeBehavior chargeBehavior;

    private void Awake() {
        chargeBehavior = GetComponent<ChargeBehavior>();
    }

    private void OnEnable() {
        chargeBehavior.OnCharge += OnCharge;
    }

    private void OnDisable() {
        chargeBehavior.OnCharge -= OnCharge;
    }

    private void OnCharge(bool fromAnim) {

        // make sure charged from anim to prevent mega electric minion splitting into 4 minions
        if (fromAnim) {
            SplitAndCharge();
        }
    }

    private void SplitAndCharge() {

        Vector2 toPlayerDirection = (PlayerMovement.Instance.CenterPos - transform.position).normalized;
        float launchAngle = 8f;


        Enemy enemy1 = splitEnemyPrefab.Spawn(transform.position, Containers.Instance.Enemies);
        enemy1.GetComponent<FacePlayerBehavior>().enabled = true;

        ChargeBehavior chargeBehavior1 = enemy1.GetComponent<ChargeBehavior>();
        chargeBehavior1.enabled = true;

        Vector2 chargeDirection1 = toPlayerDirection.GetDirectionRotated(launchAngle);
        chargeBehavior1.Charge(chargeDirection1);

        enemy1.GetComponentInChildren<Animator>().SetTrigger("forceCharge");


        Enemy enemy2 = splitEnemyPrefab.Spawn(transform.position, Containers.Instance.Enemies);
        enemy2.GetComponent<FacePlayerBehavior>().enabled = true;

        ChargeBehavior chargeBehavior2 = enemy2.GetComponent<ChargeBehavior>();
        chargeBehavior2.enabled = true;

        Vector2 chargeDirection2 = toPlayerDirection.GetDirectionRotated(-launchAngle);
        chargeBehavior2.Charge(chargeDirection2);

        enemy2.GetComponentInChildren<Animator>().SetTrigger("forceCharge");


        gameObject.ReturnToPool();
    }
}

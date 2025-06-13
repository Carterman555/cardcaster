using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonCaster : Enemy {

    [Header("Teleport")]
    [SerializeField] private RandomFloat teleportCooldown;
    private float teleportTimer;

    private RandomTeleportBehavior randomTeleportBehavior;

    protected override void Awake() {
        base.Awake();
        randomTeleportBehavior = GetComponent<RandomTeleportBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        teleportTimer = 0;
        teleportCooldown.Randomize();
    }

    protected override void Update() {
        base.Update();

        teleportTimer += Time.deltaTime;
        if (teleportTimer > teleportCooldown.Value) {
            teleportTimer = 0;
            teleportCooldown.Randomize();

            float avoidPlayerTeleportRadius = 3f;
            randomTeleportBehavior.VisualTeleport(PlayerMovement.Instance.CenterPos, avoidPlayerTeleportRadius);
        }
    }

}

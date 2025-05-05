using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bee : Enemy {

    private SwarmMovementBehavior swarmMovementBehavior;

    // TODO - remove flowers from list that get destroyed
    private List<Transform> bluePlantsInRoom;

    protected override void Awake() {
        base.Awake();

        swarmMovementBehavior = GetComponent<SwarmMovementBehavior>();
    }

    protected override void OnEnable() {
        base.OnEnable();

        Transform[] allObjectsInRoom = Room.GetCurrentRoom().GetComponentsInChildren<Transform>(true);
        bluePlantsInRoom = allObjectsInRoom.Where(g => g.CompareTag("BluePlant")).ToList();
    }

    protected override void Update() {
        base.Update();

        if (swarmMovementBehavior.IsLeader && !swarmMovementBehavior.IsSwarmMoving()) {
            if (bluePlantsInRoom.Count > 0) {
                float minDistanceSquared = float.MaxValue;
                Transform closestPlant = null;
                foreach (Transform plant in bluePlantsInRoom) {

                    float xDiff = transform.position.x - plant.position.x;
                    float yDiff = transform.position.y - plant.position.y;
                    float distanceSquared = (xDiff * xDiff) + (yDiff * yDiff);
                    if (distanceSquared < minDistanceSquared) {
                        minDistanceSquared = distanceSquared;
                        closestPlant = plant;
                    }
                }

                if (closestPlant == null) {
                    Debug.Log("Could not find closest plant!");
                }
                swarmMovementBehavior.SetSwarmDestination(closestPlant.position);
            }
        }
    }
}

using UnityEngine;

public class DistanceTravelReturn : MonoBehaviour {

    private float distanceToReturn;
    private Vector2 originalPos;

    public void Setup(float distanceToReturn) {
        this.distanceToReturn = distanceToReturn;

        originalPos = transform.position;
    }

    private void Update() {
        float distanceTravelled = Vector2.Distance(originalPos, transform.position); // could be costly
        if (distanceTravelled > distanceToReturn) {
            gameObject.ReturnToPool();
        }
    }
}


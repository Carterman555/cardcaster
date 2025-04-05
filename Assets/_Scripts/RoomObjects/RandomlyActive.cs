using UnityEngine;

public class RandomlyActive : MonoBehaviour {
    [SerializeField][Range(0f, 1f)] private float activeProbability = 0.75f;

    private void Awake() {
        RoomGenerator.OnCompleteGeneration += RandomizeActive;
    }

    private void OnDestroy() {
        RoomGenerator.OnCompleteGeneration -= RandomizeActive;
    }

    public void RandomizeActive() {
        bool active = activeProbability > Random.Range(0f, 1f);
        gameObject.SetActive(active);
    }
}

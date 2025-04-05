using UnityEngine;

public class AnimReturn : MonoBehaviour {
    [SerializeField] private GameObject objectToReturn;

    private void ReturnToPool() {
        objectToReturn.ReturnToPool();
    }
}

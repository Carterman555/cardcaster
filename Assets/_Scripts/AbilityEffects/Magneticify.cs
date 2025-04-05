using UnityEngine;

public class Magneticify : MonoBehaviour {

    [SerializeField] private float suckRadius;
    private SuckBehaviour suckBehaviour;

    private void Awake() {
        suckBehaviour = GetComponent<SuckBehaviour>();
    }

    private void Start() {
        suckBehaviour.Setup(suckRadius);
    }
}

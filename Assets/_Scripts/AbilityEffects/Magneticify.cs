using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script might be useless
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

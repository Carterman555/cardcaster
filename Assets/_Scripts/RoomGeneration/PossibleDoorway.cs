using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleDoorway : MonoBehaviour {

    [SerializeField] private DoorwaySide side;

    public DoorwaySide GetSide() {
        return side;
    }

    public void SetSide(DoorwaySide side) {
        this.side = side;
    }
}

public enum DoorwaySide { Top, Bottom, Left, Right }

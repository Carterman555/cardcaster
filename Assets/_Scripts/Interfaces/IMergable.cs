using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMergable {

    public GameObject GetObject();
    public MergeBehavior GetMergeBehavior();
}

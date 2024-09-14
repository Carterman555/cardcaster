using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// initialized scripts that can't run the code by themselves because they start out inactive
public class ScriptInitializer : MonoBehaviour {

    private void Awake() {
        IInitializable[] initializables = FindObjectsOfType<MonoBehaviour>(true).OfType<IInitializable>().ToArray();

        foreach (var initializable in initializables) {
            initializable.Initialize();
        }
    }

}

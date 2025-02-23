using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelText : MonoBehaviour {

    private TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        string environmentName = GameSceneManager.Instance.GetEnvironment().ToPrettyString();
        text.text = environmentName;

        //int subLevel = GameSceneManager.Instance.GetSubLevel();
        //text.text = environmentName + " - " + subLevel;
    }
}

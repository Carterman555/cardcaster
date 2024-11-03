using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisabledButtonFade : MonoBehaviour {

    [SerializeField] private Image[] imagesToUpdate;
    [SerializeField] private float deactiveFade;

    private bool deactiveColor;

    private Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    private void Update() {
        if (!deactiveColor && !button.interactable) {
            SetFade(deactiveFade);
            deactiveColor = true;
        }
        else if (deactiveColor && button.interactable) {
            SetFade(1f);
            deactiveColor = false;
        }
    }


    private void SetFade(float fade) {
        foreach (var image in imagesToUpdate) {
            image.Fade(fade);
        }
    }
}

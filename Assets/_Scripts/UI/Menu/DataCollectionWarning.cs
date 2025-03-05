using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollectionWarning : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            canvasGroup.DOFade(0f, duration: 0.3f).OnComplete(() => {
                gameObject.SetActive(false);
            });
        }
    }

}

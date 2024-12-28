using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateFadePanel : MonoBehaviour {
    private static List<ActivateFadePanel> activeFadePanelList;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        activeFadePanelList = new();
    }

    [SerializeField] private Image fadePanel;

    private void OnEnable() {
        activeFadePanelList.Add(this);
        UpdateFadePanelActive();
    }

    private void OnDisable() {
        activeFadePanelList.Remove(this);
        UpdateFadePanelActive();
    }

    // active fade if any objects with this script are active
    private void UpdateFadePanelActive() {
        bool activePanelForFadePanel = activeFadePanelList.Count > 0;

        float fadePanelAlpha = 0.25f;

        if (activePanelForFadePanel && !fadePanel.gameObject.activeSelf) {
            fadePanel.gameObject.SetActive(true);

            fadePanel.Fade(0f);
            fadePanel.DOFade(fadePanelAlpha, duration: 0.3f).SetUpdate(true);
        }
        else if (!activePanelForFadePanel && fadePanel.gameObject.activeSelf) {
            fadePanel.Fade(fadePanelAlpha);
            fadePanel.DOFade(0f, duration: 0.3f).SetUpdate(true).OnComplete(() => {
                fadePanel.gameObject.SetActive(false);
            });
        }
    }
}

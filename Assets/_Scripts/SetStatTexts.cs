using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class SetStatTexts : MonoBehaviour {

    [SerializeField] private LocalizeStringEvent killCountLocStringEvent;
    [SerializeField] private LocalizeStringEvent levelLocStringEvent;

    // for localized string event (it sets the text)
    public int KillCount;
    public int Level;

    private void OnEnable() {
        KillCount = GameStatsTracker.Instance.Kills;
        Level = GameSceneManager.Instance.Level;

        killCountLocStringEvent.RefreshString();
        levelLocStringEvent.RefreshString();
    }
}

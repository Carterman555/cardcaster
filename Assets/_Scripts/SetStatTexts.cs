using UnityEngine;

public class SetStatTexts : MonoBehaviour {

    // for localized string event (it sets the text)
    public int KillCount;
    public int Level;

    private void OnEnable() {
        KillCount = GameStatsTracker.Instance.GetKills();
        Level = GameSceneManager.Instance.GetLevel();
    }
}

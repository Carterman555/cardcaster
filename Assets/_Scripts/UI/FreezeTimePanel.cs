using System.Collections.Generic;
using UnityEngine;

public class FreezeTimePanel : MonoBehaviour {

    private static List<FreezeTimePanel> activeFreezePanelList;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init() {
        activeFreezePanelList = new();
    }

    private void OnEnable() {
        activeFreezePanelList.Add(this);
        UpdateFreezeTime();
    }

    private void OnDisable() {
        activeFreezePanelList.Remove(this);
        UpdateFreezeTime();
    }

    // freeze time if any objects with this script are active
    private void UpdateFreezeTime() {
        bool anyActiveFreezePanels = activeFreezePanelList.Count > 0;

        if (anyActiveFreezePanels) {
            Time.timeScale = 0f;
        }
        else {
            Time.timeScale = 1f;
        }
    }

}

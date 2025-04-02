using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamTest : MonoBehaviour {

    private void Start() {
        if (!SteamManager.Initialized) {
            print("Not initialized");
            return;
        }

    }

    [ContextMenu("Print game language")]
    private void PrintLanguage() {
        if (!SteamManager.Initialized) {
            print("Not initialized");
            return;
        }

        print(SteamApps.GetCurrentGameLanguage());
    }
}

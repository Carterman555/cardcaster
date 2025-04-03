using System.Collections;
using UnityEngine;
using Steamworks;
using UnityEngine.Localization.Settings;
using QFSW.QC;

public class LocalizationManager : MonoBehaviour {

    private void Start() {
        if (!SteamManager.Initialized) {
            return;
        }

        string localIdentifier = GetIdentifierFromLanguage(SteamApps.GetAvailableGameLanguages());
        StartCoroutine(SetUnityLanguage(localIdentifier));
    }

    [Command]
    private void PrintLanguage() {
        if (!SteamManager.Initialized) {
            print("Not initialized");
            return;
        }

        print(SteamApps.GetCurrentGameLanguage());
    }

    private string GetIdentifierFromLanguage(string language) {
        switch (language) {
            case "english": return "en";
            case "schinese": return "zh-Hans";
            default: return null;
        }
    }

    private IEnumerator SetUnityLanguage(string localeCode) {
        yield return LocalizationSettings.InitializationOperation;

        var availableLocales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in availableLocales) {
            if (locale.Identifier.Code == localeCode) {
                LocalizationSettings.SelectedLocale = locale;
                yield break;
            }
        }

        Debug.LogWarning($"Could not find locale for code: {localeCode}, using default");
    }
}

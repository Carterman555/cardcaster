using System.Collections;
using UnityEngine;
using Steamworks;
using UnityEngine.Localization.Settings;
using QFSW.QC;
using UnityEngine.Localization;
using System;

public class LocalizationManager : Singleton<LocalizationManager> {

    private IEnumerator Start() {

        int maxTries = 100;
        int tryCount = 0;

        if (!SteamManager.Initialized) {
            yield return null;
            tryCount++;

            if (tryCount >= maxTries) {
                yield break;
            }
        }

        if (SettingsManager.CurrentSettings.Language == "not set") {
            string localIdentifier = GetLanguageCode();
            SetUnityLanguage(localIdentifier);
        }
    }

    public string GetLanguageCode() {
        return GetIdentifierFromLanguage(SteamApps.GetCurrentGameLanguage());
    }

    private string GetIdentifierFromLanguage(string language) {
        switch (language) {
            case "english": return "en";
            case "schinese": return "zh-Hans";
            default: return null;
        }
    }

    public void SetUnityLanguage(string localeCode) {
        StartCoroutine(SetUnityLanguageCor(localeCode));
    }
    private IEnumerator SetUnityLanguageCor(string localeCode) {
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

    [Command]
    private void PrintLanguage() {
        if (!SteamManager.Initialized) {
            print("Not initialized");
            return;
        }

        print(SteamApps.GetCurrentGameLanguage());
    }
}

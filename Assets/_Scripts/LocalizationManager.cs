using System.Collections;
using UnityEngine;
using Steamworks;
using UnityEngine.Localization.Settings;
using QFSW.QC;
using System.IO;
using System.Text;

#if UNITY_EDITOR
using UnityEditor.Localization;
using UnityEditor.Localization.Plugins.CSV;
#endif

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
        else {
            SetUnityLanguage(SettingsManager.CurrentSettings.Language);
        }
    }

    public static string GetLanguageCode() {

        if (!SteamManager.Initialized) {
            Debug.LogError("Trying to get language code, the SteamManager is not initialized!");
            return "en";
        }

        return GetIdentifierFromLanguage(SteamApps.GetCurrentGameLanguage());
    }

    private static string GetIdentifierFromLanguage(string language) {
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

#if UNITY_EDITOR
    [ContextMenu("Export")]
    public void Export() {
        var collection = LocalizationEditorSettings.GetStringTableCollection("StringTable");
        using (var stream = new StreamWriter("LocalizationTable.csv", false, new UTF8Encoding(false))) {
            Csv.Export(stream, collection);
        }
    }
#endif
}

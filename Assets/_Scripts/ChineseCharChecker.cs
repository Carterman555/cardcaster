using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class ChineseCharChecker : MonoBehaviour {

    [SerializeField] private Transform englishTextContainer;
    [SerializeField] private Transform chineseTextContainer;

    [SerializeField] private TextMeshProUGUI englishTextPrefab;
    [SerializeField] private TextMeshProUGUI chineseTextPrefab;

    private IEnumerator Start() {

        string tableName = "StringTable";

        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;

        // Get all unique keys from the first available table
        HashSet<string> allKeys = new HashSet<string>();
        StringTable firstTable = null;

        foreach (var locale in locales) {
            var table = LocalizationSettings.StringDatabase.GetTable(tableName, locale) as StringTable;
            if (table != null) {
                firstTable = table;
                foreach (var entry in table.Values) {
                    if (entry != null) {
                        allKeys.Add(entry.Key);
                    }
                }
                break; // Use first available table to get keys
            }
        }

        if (firstTable == null) {
            Debug.LogError($"Table '{tableName}' not found in any locale!");
            yield break;
        }

        // Print each key with all its translations
        foreach (string key in allKeys) {

            foreach (var locale in locales) {
                // Try to get localized string directly
                var localizedString = new LocalizedString(tableName, key);

                // Set the locale temporarily to get the translation
                var originalLocale = LocalizationSettings.SelectedLocale;
                LocalizationSettings.SelectedLocale = locale;

                string translation = localizedString.GetLocalizedString();

                // Restore original locale
                LocalizationSettings.SelectedLocale = originalLocale;

                if (!string.IsNullOrEmpty(translation)) {
                    Debug.Log($"  {locale.LocaleName} ({locale.Identifier.Code}): \"{translation}\"");

                    bool english = locale.Identifier.Code == "en";
                    TextMeshProUGUI textPrefab = english ? englishTextPrefab : chineseTextPrefab;
                    Transform parent = english ? englishTextContainer : chineseTextContainer;
                    TextMeshProUGUI newText = textPrefab.Spawn(parent);
                    newText.text = translation;
                }
                else {
                    Debug.Log($"  ({locale.Identifier.Code}): [MISSING]");
                }
            }
        }
    }
}

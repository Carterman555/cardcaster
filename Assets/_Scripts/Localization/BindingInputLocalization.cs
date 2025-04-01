using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using TMPro;

public class BindingInputLocalization : MonoBehaviour {

    private TextMeshProUGUI text;
    private string inputStr;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        UpdateText();
    }

    private void Update() {
        bool textChanged = text.text != inputStr;
        if (textChanged) {
            UpdateText();
        }
    }

    private void UpdateText() {
        bool inChinese = LocalizationSettings.SelectedLocale.Identifier == "zh-Hans";
        if (inChinese) {
            var stringTableCollection = LocalizationEditorSettings.GetStringTableCollection("StringTable");
            var chineseTable = stringTableCollection.GetTable(LocalizationSettings.SelectedLocale.Identifier) as StringTable;

            var entry = chineseTable.GetEntry(text.text);
            if (entry != null) {
                text.text = entry.LocalizedValue;
            }
        }

        inputStr = text.text;
    }
}

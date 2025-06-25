using UnityEngine;
using UnityEngine.Localization.Settings;
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
        bool inChinese = LocalizationSettings.SelectedLocale.Identifier.Code == "zh-Hans";
        if (inChinese) {

            var stringTable = LocalizationSettings.StringDatabase.GetTable("StringTable");
            if (stringTable == null) {
                Debug.LogError($"String table not found.");
                return;
            }

            var entry = stringTable.GetEntry(text.text);
            if (entry != null) {
                text.text = entry.LocalizedValue;
            }
        }
        inputStr = text.text;
    }
}

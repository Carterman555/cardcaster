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
            // Runtime approach to get the localized string
            var localizedString = LocalizationSettings.StringDatabase.GetLocalizedString("StringTable", text.text);
            if (!string.IsNullOrEmpty(localizedString)) {
                text.text = localizedString;
            }
        }
        inputStr = text.text;
    }
}

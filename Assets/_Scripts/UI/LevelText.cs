using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LevelText : MonoBehaviour {

    private TextMeshProUGUI text;

    [SerializeField] private LocalizedString stoneLocString;
    [SerializeField] private LocalizedString smoothStoneLocString;
    [SerializeField] private LocalizedString blueStoneLocString;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateText;
        UpdateText(LocalizationSettings.SelectedLocale);
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateText;
    }

    private void UpdateText(Locale locale) {
        EnvironmentType environmentType = GameSceneManager.Instance.GetEnvironment();

        if (environmentType == EnvironmentType.Stone) {
            text.text = stoneLocString.GetLocalizedString();
        }
        else if (environmentType == EnvironmentType.SmoothStone) {
            text.text = smoothStoneLocString.GetLocalizedString();
        }
        else if (environmentType == EnvironmentType.BlueStone) {
            text.text = blueStoneLocString.GetLocalizedString();
        }
        else {
            Debug.LogError($"{environmentType} is not supported.");
        }

        //int subLevel = GameSceneManager.Instance.GetSubLevel();
        //text.text += " - " + subLevel;
    }
}

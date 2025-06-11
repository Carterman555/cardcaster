using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LevelText : MonoBehaviour {

    private TextMeshProUGUI text;
    private MMF_Player fadePlayer;

    [SerializeField] private LocalizedString stoneLocString;
    [SerializeField] private LocalizedString smoothStoneLocString;
    [SerializeField] private LocalizedString blueStoneLocString;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        fadePlayer = GetComponent<MMF_Player>();
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateText;
        GameSceneManager.OnLoadingCompleted += ShowText;

        UpdateText(LocalizationSettings.SelectedLocale);
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateText;
        GameSceneManager.OnLoadingCompleted -= ShowText;
    }

    private void ShowText() {
        fadePlayer.PlayFeedbacks();
    }

    private void UpdateText(Locale locale) {
        EnvironmentType environmentType = GameSceneManager.Instance.CurrentEnvironment;

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

        int environmentLevel = GameSceneManager.Instance.GetEnvironmentLevel();
        text.text += " " + environmentLevel;
    }
}

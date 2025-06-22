using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizeFont : MonoBehaviour {

    private TMP_Text text;

    private float englishFontSize;
    private bool autoResizeInEnglish;

    private float chineseFontSizeMult = 1.5f;

    [SerializeField] private bool overrideEnglishFontSize;
    [SerializeField, ConditionalHide("overrideEnglishFontSize")] private float englishFontSizeOverride;

    // do these if in chinese
    [SerializeField] private bool roundToNearest11 = true;
    [SerializeField] private bool forceRoundUp = false;
    [SerializeField] private bool disableAutoResize = true;

    [SerializeField] private bool differentChineseLineSpacing;
    [SerializeField, ConditionalHide("differentChineseLineSpacing")] private float chineseLineSpacing;
    private float defaultLineSpacing;

    private void Awake() {
        text = GetComponent<TMP_Text>();

        englishFontSize = text.fontSize;
        if (overrideEnglishFontSize) {
            englishFontSize = englishFontSizeOverride;
        }

        autoResizeInEnglish = text.enableAutoSizing;

        defaultLineSpacing = text.lineSpacing;
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateFont;

        UpdateFont(LocalizationSettings.SelectedLocale);
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateFont;
    }

    private void UpdateFont(Locale locale) {
        if (locale.Identifier == "en") {
            TMP_FontAsset englishFont = Resources.Load<TMP_FontAsset>("Fonts/pixelfont-4x5 SDF");
            text.font = englishFont;

            text.fontSize = englishFontSize;
            text.enableAutoSizing = autoResizeInEnglish;
            text.lineSpacing = defaultLineSpacing;
        }
        else if (locale.Identifier == "zh-Hans") {
            TMP_FontAsset chineseFont = Resources.Load<TMP_FontAsset>("Fonts/LanaPixel SDF");
            text.font = chineseFont;

            float fontSize = englishFontSize * chineseFontSizeMult;
            if (roundToNearest11) {
                // the lanapixel font is a little stretched and distorted when the font is not a multiple of 11
                // so round to nearest 11
                if (forceRoundUp) {
                    fontSize = Mathf.Ceil(fontSize / 11f) * 11f;
                }
                else {
                    fontSize = Mathf.Round(fontSize / 11f) * 11f;
                }
            }

            if (disableAutoResize) {
                text.enableAutoSizing = false;
            }

            text.fontSize = fontSize;
            text.lineSpacing = differentChineseLineSpacing ? chineseLineSpacing : defaultLineSpacing;
        }
    }
}

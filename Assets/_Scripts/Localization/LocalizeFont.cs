using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizeFont : MonoBehaviour {

    private TextMeshProUGUI text;

    private float englishFontSize;
    private float chineseFontSizeMult = 1.5f;

    [SerializeField] private bool overrideEnglishFontSize;
    [ConditionalHide("overrideEnglishFontSize")]
    [SerializeField] private float englishFontSizeOverride;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();

        englishFontSize = text.fontSize;
        if (overrideEnglishFontSize) {
            englishFontSize = englishFontSizeOverride;
        }

        //if (text.alignment != TextAlignmentOptions.Capline &&
        //    text.alignment != TextAlignmentOptions.CaplineLeft &&
        //    text.alignment != TextAlignmentOptions.CaplineRight) {
        //    Debug.LogWarning("Text with LocalizeFont does not have Capline alignment! " + text.alignment);
        //}
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
        }
        else if (locale.Identifier == "zh-Hans") {
            TMP_FontAsset chineseFont = Resources.Load<TMP_FontAsset>("Fonts/LanaPixel SDF");
            text.font = chineseFont;
            text.fontSize = englishFontSize * chineseFontSizeMult;
        }
    }
}

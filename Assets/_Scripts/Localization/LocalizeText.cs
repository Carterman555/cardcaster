using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

public class LocalizeText : MonoBehaviour {

    public TextMeshProUGUI text;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        var localizer = gameObject.AddComponent<GameObjectLocalizer>();

        var trackedText = localizer.GetTrackedObject<TrackedUGuiGraphic>(text);

        var textVariant = trackedText.GetTrackedProperty<LocalizedStringProperty>("m_text");
        textVariant.LocalizedString.SetReference("StringTable", "testKey");

        var fontVariant = trackedText.GetTrackedProperty<LocalizedAssetProperty>("m_FontData.m_Font");
        fontVariant.LocalizedObject = new LocalizedAsset<TMP_FontAsset>() { TableReference = "NewFontTable", TableEntryReference = "NewFont" };

        var fontSize = trackedText.GetTrackedProperty<IntTrackedProperty>("m_FontData.m_FontSize");
        fontSize.SetValue(LocalizationSettings.ProjectLocale.Identifier, 50); // default font size
        fontSize.SetValue("en", 50);
        fontSize.SetValue("zh-Hans", 80);

        // force update
        //localizer.ApplyLocaleVariant(LocalizationSettings.SelectedLocale);
    }
}

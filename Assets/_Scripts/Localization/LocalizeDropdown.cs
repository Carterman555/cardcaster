using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizeDropdown : MonoBehaviour {

    [SerializeField] private LocalizedString[] localizedOptions;

    private TMP_Dropdown dropdown;

    private void Awake() {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnEnable() {
        LocalizationSettings.SelectedLocaleChanged += UpdateOptions;
        UpdateOptions();
    }

    private void OnDisable() {
        LocalizationSettings.SelectedLocaleChanged -= UpdateOptions;
    }

    private void UpdateOptions(Locale locale = null) {

        if (dropdown.options.Count != localizedOptions.Length) {
            Debug.LogError("Dropdown option count not equal to localizedOptions length!");
            return;
        }

        for (int i = 0; i < dropdown.options.Count; i++) {
            dropdown.options[i].text = localizedOptions[i].GetLocalizedString();
        }
        dropdown.RefreshShownValue();
    }
}

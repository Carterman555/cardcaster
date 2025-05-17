using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ClearDataButton : GameButton {

    [SerializeField] private WarningPopup warningPopup;

    [SerializeField] private LocalizedString locWarningText;
    [SerializeField] private CanvasGroup canvasGroupToDisable;

    protected override void OnClick() {
        base.OnClick();

        warningPopup.Setup(locWarningText, canvasGroupToDisable, GetComponent<Button>());
        warningPopup.OnAccepted += ClearData;
    }

    private void ClearData() {
        warningPopup.OnAccepted -= ClearData;

        ES3.DeleteFile();
        print("Delete file");

        ResourceSystem.Instance.UpdateUnlockedCards();
    }
}

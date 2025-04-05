using UnityEngine;

public class UIPosDebuger : MonoBehaviour {

    [ContextMenu("Print Pos")]
    private void PrintPos() {

        RectTransform rectTransform = GetComponent<RectTransform>();

        print($"rectTransform.position {rectTransform.position.y}");

        float selectedBottomYPos = rectTransform.position.y - (rectTransform.rect.height / 2f);

        print($"bottom position {selectedBottomYPos}");
    }
}

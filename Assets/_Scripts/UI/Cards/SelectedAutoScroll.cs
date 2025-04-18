using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedAutoScroll : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;
    [SerializeField] private RectTransform content;

    [SerializeField] private float spacing = 30f;

    private void OnEnable() {
        InputManager.OnSelectedChanged += UpdateScroll;
    }

    private void OnDisable() {
        InputManager.OnSelectedChanged -= UpdateScroll;
    }

    private void UpdateScroll(GameObject selectedObject) {
        if (selectedObject != null) {
            HandleScrollDown();
            HandleScrollUp();
        }
    }

    private void HandleScrollDown() {

        float viewHeightFromBottom = viewport.rect.height * viewport.pivot.y;
        float viewBottomYPos = viewport.position.y - viewHeightFromBottom;

        RectTransform selectedTransform = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        //... the minimum y value that needs to be showing in the scroll view. Comes from bottom of selectedTransform
        float selectedBottomYPos = selectedTransform.position.y - (selectedTransform.rect.height / 2f);

        //... positive when selected button is below viewport, then the content should move up
        float difference = viewBottomYPos - (selectedBottomYPos - spacing);

        if (difference > 0) {
            content.position += Vector3.up * difference;
        }
    }

    private void HandleScrollUp() {

        float viewHeightFromTop = viewport.rect.height * (1 - viewport.pivot.y);
        float viewTopYPos = viewport.position.y + viewHeightFromTop;

        RectTransform selectedTransform = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();

        //... the max y value that needs to be showing in the scroll view. Comes from top of selectedTransform (pos + half of height)
        float selectedTopYPos = selectedTransform.position.y + (selectedTransform.rect.height / 2f);

        //... negative when selected button is above viewport, then the content should move down
        float difference = viewTopYPos - (selectedTopYPos + spacing);

        if (difference < 0) {
            content.position += Vector3.up * difference;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaterialOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private Image image;
    [SerializeField] private Material hoverMaterial;

    private Material originalMaterial;

    private void Awake() {
        originalMaterial = image.material;
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        image.material = hoverMaterial;
    }

    public void OnPointerExit(PointerEventData eventData) {

        if (InputManager.Instance.GetControlScheme() != ControlSchemeType.Keyboard) {
            return;
        }

        image.material = originalMaterial;
    }
}

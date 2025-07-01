using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MaterialOnSelected : MonoBehaviour, ISelectHandler, IDeselectHandler {

    [SerializeField] private Image image;
    [SerializeField] private Material selectMaterial;
    
    private Material originalMaterial;

    private void Awake() {
        originalMaterial = image.material;
    }

    public void OnSelect(BaseEventData eventData) {
        image.material = selectMaterial;
    }

    public void OnDeselect(BaseEventData eventData) {
        image.material = originalMaterial;
    }
}

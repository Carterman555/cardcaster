using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedAutoScroll : MonoBehaviour
{
    [SerializeField] private RectTransform content;

    private void Update() {
        RectTransform selectedTransform = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
    }
}

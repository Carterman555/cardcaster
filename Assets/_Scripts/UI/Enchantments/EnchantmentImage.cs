using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnchantmentImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    
    private Image image;
    [SerializeField] private TextMeshProUGUI amountText;

    private EnchantmentType enchantmentType;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void Setup(EnchantmentType enchantmentType, int amount) {
        this.enchantmentType = enchantmentType;

        ScriptableEnchantment scriptableEnchantment = ResourceSystem.Instance.GetEnchantment(enchantmentType);
        image.sprite = scriptableEnchantment.Sprite;
        amountText.text = amount.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        EnchantmentPanel.Instance.ShowEnchantmentInfo(enchantmentType, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        EnchantmentPanel.Instance.HideEnchantmentInfo();
    }
}

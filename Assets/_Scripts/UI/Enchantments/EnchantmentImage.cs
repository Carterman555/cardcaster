using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnchantmentImage : Selectable {

    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image amountBackground;

    private EnchantmentType enchantmentType;

    public void Setup(EnchantmentType enchantmentType, int amount) {
        this.enchantmentType = enchantmentType;

        ScriptableEnchantment scriptableEnchantment = ResourceSystem.Instance.GetEnchantment(enchantmentType);
        image.sprite = scriptableEnchantment.Sprite;

        if (amount == 1) {
            amountText.text = "";
            amountBackground.enabled = false;
        }
        else {
            amountText.text = amount.ToString();
            amountBackground.enabled = true;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
        EnchantmentInfoManager.Instance.ShowEnchantmentInfo(enchantmentType, transform.position);
    }

    public override void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);
        EnchantmentInfoManager.Instance.HideEnchantmentInfo();
    }

    public override void OnSelect(BaseEventData eventData) {
        base.OnSelect(eventData);

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Keyboard) {
            //... delay because can't change selected object two time in one frame
            DOVirtual.DelayedCall(Time.deltaTime, () => {
                EventSystem.current.SetSelectedGameObject(null);
            });
        }
        else if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            if (EnchantmentInfoManager.Instance != null) {
                EnchantmentInfoManager.Instance.ShowEnchantmentInfo(enchantmentType, transform.position);
            }
        }
    }

    public override void OnDeselect(BaseEventData eventData) {
        base.OnDeselect(eventData);

        if (InputManager.Instance.GetControlScheme() == ControlSchemeType.Controller) {
            if (EnchantmentInfoManager.Instance != null) {
                EnchantmentInfoManager.Instance.HideEnchantmentInfo();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillController : MonoBehaviour
{
    [SerializeField] private Image fill;

    public void SetFillAmount(float fillAmount) {
        fill.fillAmount = fillAmount;
    }
}

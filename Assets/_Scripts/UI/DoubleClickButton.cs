using System;
using UnityEngine;
using UnityEngine.UI;

public class DoubleClickButton : MonoBehaviour
{
    public event Action OnDoubleClick;

    private Button _myButton;
    private int _clickCount = 0;
    private float _clickTime = 0;
    private float _clickDelay = 0.5f;

    private void Awake() => _myButton = GetComponent<Button>();

    private void Start() => _myButton.onClick.AddListener(OnClick);

    private void OnClick()
    {
        _clickCount++;
        if (_clickCount == 1)
            _clickTime = Time.time;

        if (_clickCount > 1 && Time.time - _clickTime < _clickDelay)
        {
            OnDoubleClick?.Invoke();

            _clickCount = 0;
            _clickTime = 0;
        }
        else if (Time.time - _clickTime > _clickDelay)
            _clickCount = 0;
    }
}
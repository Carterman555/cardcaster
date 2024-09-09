using UnityEngine;
using UnityEngine.UI;

public class GameButton : MonoBehaviour {

    protected Button button;

    protected virtual void Awake() {
        button = GetComponent<Button>();
    }

    protected virtual void OnEnable() {
        button.onClick.AddListener(OnClick);
    }
    protected virtual void OnDisable() {
        button.onClick.RemoveListener(OnClick);
    }

    //[SerializeField] private bool playClickAudio = true;

    protected virtual void OnClick() {
        //if (playClickAudio) {
        //    AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.ButtonClick, 0f, 1f);
        //}
    }
}
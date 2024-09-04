using UnityEngine;
using UnityEngine.UI;

public class GameButton : MonoBehaviour {

    protected Button button;

    private void Awake() {
        button = GetComponent<Button>();
    }

    protected virtual void OnEnable() {
        button.onClick.AddListener(OnClicked);
    }
    protected virtual void OnDisable() {
        button.onClick.RemoveListener(OnClicked);
    }

    //[SerializeField] private bool playClickAudio = true;

    protected virtual void OnClicked() {
        //if (playClickAudio) {
        //    AudioSystem.Instance.PlaySound(AudioSystem.SoundClips.ButtonClick, 0f, 1f);
        //}
    }
}
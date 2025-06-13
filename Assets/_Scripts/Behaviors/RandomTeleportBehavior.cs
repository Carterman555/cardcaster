using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class RandomTeleportBehavior : MonoBehaviour {

    [SerializeField] private Animator anim;

    private bool avoidCircle;
    private Vector2 noTeleportCenter;
    private float noTeleportRadius;

    public void VisualTeleport() {
        avoidCircle = false;

        anim.SetTrigger("teleportFade");
    }

    public void VisualTeleport(Vector2 noTeleportCenter, float noTeleportRadius) {
        this.noTeleportCenter = noTeleportCenter;
        this.noTeleportRadius = noTeleportRadius;

        avoidCircle = true;

        anim.SetTrigger("teleportFade");
    }

    // played by anim
    public void Teleport() {

        Vector2 newPosition;
        if (avoidCircle) {
            newPosition = new RoomPositionHelper().GetRandomRoomPos(noTeleportCenter, noTeleportRadius);
        }
        else {
            newPosition = new RoomPositionHelper().GetRandomRoomPos();
        }

        transform.position = newPosition;
    }

    public void Teleport(Vector2 noTeleportCenter, float noTeleportRadius) {
        this.noTeleportCenter = noTeleportCenter;
        this.noTeleportRadius = noTeleportRadius;
        avoidCircle = true;

        Teleport();
    }

    // played by anim
    public void TeleportSfx() {
        AudioManager.Instance.PlaySound(AudioManager.Instance.AudioClips.Teleport);
    }
}

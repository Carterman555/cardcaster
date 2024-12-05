using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSurge : MonoBehaviour {

    public enum TargetType { Player, Random }

    public void Setup(TargetType type) {

        Vector2 targetPoint = Vector2.zero;

        if (type == TargetType.Player) {
            targetPoint = PlayerMeleeAttack.Instance.transform.position;
        }
        else if (type == TargetType.Random) {
            targetPoint = new RoomPositionHelper().GetRandomRoomPos();
        }

        transform.position = targetPoint;

        //... choose random direction
        float angle = Random.Range(0f, 360f);

        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}

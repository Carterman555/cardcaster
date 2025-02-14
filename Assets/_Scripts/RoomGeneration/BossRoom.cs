using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour {
    [SerializeField] private Transform bossSpawnPoint;

    public Transform GetBossSpawnPoint() {
        return bossSpawnPoint;
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += CreateBossIcons;
    }

    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= CreateBossIcons;
    }

    #region Boss Icon

    private List<BossIcon> bossIcons = new();

    [SerializeField] private BossIcon verticalBossIconPrefab;
    [SerializeField] private BossIcon horizontalBossIconPrefab;

    private void CreateBossIcons() {
        List<PossibleDoorway> createdDoorways = GetComponent<Room>().GetCreateDoorways();

        foreach (PossibleDoorway createdDoorway in createdDoorways) {
            if (createdDoorway.GetSide() == DoorwaySide.Top) {
                BossIcon bossIcon = verticalBossIconPrefab.Spawn(createdDoorway.transform.position, transform);
                bossIcon.GetComponent<BoxCollider2D>().size = new Vector2(2f, 13f);
                bossIcon.GetComponentInChildren<SpriteRenderer>().transform.localPosition = new Vector2(0f, -4f);
            }
            else if (createdDoorway.GetSide() == DoorwaySide.Bottom) {
                BossIcon bossIcon = verticalBossIconPrefab.Spawn(createdDoorway.transform.position, transform);
                bossIcon.GetComponent<BoxCollider2D>().size = new Vector2(2f, 13f);
                bossIcon.GetComponentInChildren<SpriteRenderer>().transform.localPosition = new Vector2(0f, 4f);
            }
            else if (createdDoorway.GetSide() == DoorwaySide.Left) {
                BossIcon bossIcon = horizontalBossIconPrefab.Spawn(createdDoorway.transform.position, transform);
                bossIcon.GetComponent<BoxCollider2D>().size = new Vector2(13f, 2f);
                bossIcon.GetComponentInChildren<SpriteRenderer>().transform.localPosition = new Vector2(4f, 0f);
            }
            else if (createdDoorway.GetSide() == DoorwaySide.Right) {
                BossIcon bossIcon = horizontalBossIconPrefab.Spawn(createdDoorway.transform.position, transform);
                bossIcon.GetComponent<BoxCollider2D>().size = new Vector2(13f, 2f);
                bossIcon.GetComponentInChildren<SpriteRenderer>().transform.localPosition = new Vector2(-4f, 0f);
            }
        }
    }

    #endregion
}

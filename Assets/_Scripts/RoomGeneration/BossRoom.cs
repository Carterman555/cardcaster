using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour {
    [SerializeField] private Transform bossSpawnPoint;

    public Transform GetBossSpawnPoint() {
        return bossSpawnPoint;
    }

    private void OnEnable() {
        CreateBossIcons();
    }

    #region Boss Icon

    private List<BossIcon> bossIcons = new();

    [SerializeField] private BossIcon verticalBossIcon;
    [SerializeField] private BossIcon horizontalBossIcon;

    private void CreateBossIcons() {

    }

    #endregion
}

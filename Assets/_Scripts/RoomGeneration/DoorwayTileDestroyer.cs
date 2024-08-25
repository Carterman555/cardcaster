using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DoorwayTileReplacer", menuName = "Misc/DoorwayTileReplacer")]
public class DoorwayTileDestroyer : ScriptableObject {

    [SerializeField] private DestroyInfo[] destroyTileInfo;

    public void DestroyTiles(Tilemap groundTilemap, Tilemap colliderTilemap, DoorwaySide doorwaySide, Vector2 doorwayPosition) {
        DestroyInfo correctDestroyInfo = destroyTileInfo.FirstOrDefault(info => info.DoorwaySide == doorwaySide);
        Vector3Int startPosition = new Vector3Int((int)doorwayPosition.x, (int)doorwayPosition.y) + (Vector3Int)correctDestroyInfo.Offset;
        DestroyTiles(groundTilemap, colliderTilemap, startPosition, correctDestroyInfo.Size);
    }

    private void DestroyTiles(Tilemap groundTilemap, Tilemap colliderTilemap, Vector3Int startPosition, Vector2Int size) {
        for (int x = 0; x < size.x; x++) {
            for (int y = 0; y < size.y; y++) {
                Vector3Int tilePosition = new Vector3Int(startPosition.x + x, startPosition.y + y);
                colliderTilemap.SetTile(tilePosition, null);
                groundTilemap.SetTile(tilePosition, null);
            }
        }
    }
}

[Serializable]
public struct DestroyInfo {
    public DoorwaySide DoorwaySide;
    public Vector2Int Offset;
    public Vector2Int Size;
}

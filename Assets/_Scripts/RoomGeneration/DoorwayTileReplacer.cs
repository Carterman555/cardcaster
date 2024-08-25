using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DoorwayTileReplacer", menuName = "Misc/DoorwayTileReplacer")]
public class DoorwayTileReplacer : ScriptableObject {

    [SerializeField] private TileInfo[] topSideTileReplacements;
    [SerializeField] private TileInfo[] bottomSideTileReplacements;
    [SerializeField] private TileInfo[] leftSideTileReplacements;
    [SerializeField] private TileInfo[] rightSideTileReplacements;

    public void ReplaceTiles(Tilemap groundTilemap, Tilemap colliderTilemap, DoorwaySide doorwaySide, Vector2 doorwayPosition) {

        if (doorwaySide == DoorwaySide.Top) {
            ReplaceTilesHorizontally(groundTilemap, colliderTilemap, doorwayPosition, topSideTileReplacements);
        }
        else if (doorwaySide == DoorwaySide.Bottom) {
            ReplaceTilesHorizontally(groundTilemap, colliderTilemap, doorwayPosition, bottomSideTileReplacements);
        }
        else if (doorwaySide == DoorwaySide.Left) {
            ReplaceTilesVertically(groundTilemap, colliderTilemap, doorwayPosition, leftSideTileReplacements);
        }
        else if (doorwaySide == DoorwaySide.Right) {
            ReplaceTilesVertically(groundTilemap, colliderTilemap, doorwayPosition, rightSideTileReplacements);
        }
    }

    private void ReplaceTilesVertically(Tilemap groundTilemap, Tilemap colliderTilemap, Vector2 doorwayPosition, TileInfo[] tileReplacements) {

        Vector3Int offset = new Vector3Int(0, -2);
        Vector3Int startPosition = new Vector3Int((int)doorwayPosition.x, (int)doorwayPosition.y) + offset;

        for (int y = 0; y < tileReplacements.Length; y++) {
            Vector3Int tilePosition = new Vector3Int(startPosition.x, startPosition.y + y);
            TileInfo tileReplacement = tileReplacements[y];

            ReplaceTile(groundTilemap, colliderTilemap, tilePosition, tileReplacement);
        }
    }

    private void ReplaceTilesHorizontally(Tilemap groundTilemap, Tilemap colliderTilemap, Vector2 doorwayPosition, TileInfo[] tileReplacements) {

        Vector3Int offset = new Vector3Int(-2, 0);
        Vector3Int startPosition = new Vector3Int((int)doorwayPosition.x, (int)doorwayPosition.y) + offset;

        for (int x = 0; x < tileReplacements.Length; x++) {
            Vector3Int tilePosition = new Vector3Int(startPosition.x + x, startPosition.y);
            TileInfo tileReplacement = tileReplacements[x];

            ReplaceTile(groundTilemap, colliderTilemap, tilePosition, tileReplacement);
        }
    }

    private void ReplaceTile(Tilemap groundTilemap, Tilemap colliderTilemap, Vector3Int tilePosition, TileInfo tileReplacement) {
        if (tileReplacement.Collider) {
            colliderTilemap.SetTile(tilePosition, tileReplacement.Tile);
            groundTilemap.SetTile(tilePosition, null);
        }
        else {
            groundTilemap.SetTile(tilePosition, tileReplacement.Tile);
            colliderTilemap.SetTile(tilePosition, null);
        }
    }
}

[Serializable]
public struct TileInfo {
    public Tile Tile;
    public bool Collider;
}

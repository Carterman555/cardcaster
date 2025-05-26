using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// <summary>
/// This class contains the tiles and the logic for creating the room tiles from the ground tiles.
/// </summary>
[CreateAssetMenu(fileName = "RoomTilemapCreator", menuName = "RoomCreator/RoomTilemapCreator")]
public class RoomTilemapCreator : ScriptableObject {

    [SerializeField] private TileSet[] tileSets;

    [SerializeField] private TileBase[] groundTilesToIngore;

    // I realized I want to have variables instead of passing these through as parameters, but I haven't 
    // removed them as parameters yet
    private Tilemap groundTilemap;
    private Tilemap topWallsTilemap;
    private Tilemap botWallsTilemap;

    // takes in the three tilemaps: ground (already contains tiles), top walls, bot walls.
    // 1) match tileGrid values to ground tilemap
    // 2) turn tiles the outline the ground into wall tile type
    // 3) goes through each outline and decides which tile it's going to be based on the position of the surrounding
    // ground tiles
    // 4) places the tiles in the array
    public void CreateRoomTiles(EnvironmentType environmentType, Tilemap groundTilemap, Tilemap topWallsTilemap, Tilemap botWallsTilemap) {

        this.groundTilemap = groundTilemap;
        this.topWallsTilemap = topWallsTilemap;
        this.botWallsTilemap = botWallsTilemap;

        BoundsInt bounds = groundTilemap.cellBounds;

        //... the wall tiles increases the size of the tileGrid
        Vector2Int outlineSize = new Vector2Int(2, 4);
        TileType[,] tileGrid = new TileType[bounds.size.x + outlineSize.x, bounds.size.y + outlineSize.y];

        CopyGroundTilesToTileGrid(ref tileGrid, groundTilemap);

        OutlineGridWithWallTiles(ref tileGrid);

        SetSpecificWallTileTypes(ref tileGrid);

        PlaceTiles(tileGrid, environmentType, groundTilemap, topWallsTilemap, botWallsTilemap);
    }

    private void CopyGroundTilesToTileGrid(ref TileType[,] tileGrid, Tilemap groundTilemap) {

        BoundsInt bounds = groundTilemap.cellBounds;

        // iterate through each cell in the Tilemap
        for (int x = 0; x < bounds.size.x; x++) {
            for (int y = 0; y < bounds.size.y; y++) {
                Vector3Int tilemapPosition = new Vector3Int(x + bounds.min.x, y + bounds.min.y, 0);

                // set the tile to the tileGrid
                TileBase tile = groundTilemap.GetTile(tilemapPosition);
                if (tile != null && !groundTilesToIngore.Contains(tile)) {
                    //... add 1 because index 0 includes the walls so ground starts at (1, 1)
                    tileGrid[x + 1, y + 1] = TileType.Ground;
                }
            }
        }
    }

    private void OutlineGridWithWallTiles(ref TileType[,] tileGrid) {

        for (int x = 0; x < tileGrid.GetLength(0); x++) {
            for (int y = 0; y < tileGrid.GetLength(1); y++) {
                if (tileGrid[x, y] == TileType.Ground) {
                    TurnSurroundingIntoWallTiles(ref tileGrid, x, y);
                }
            }
        }
    }

    // turn the surrounding 'none' tiles into wall tiles. The surrounding tiles are 1 space in every direction (including
    // diagonals), except three spaces up because the top walls are three tiles high
    private void TurnSurroundingIntoWallTiles(ref TileType[,] tileGrid, int groundX, int groundY) {

        // go through each tile surrounding the ground tile
        for (int x = groundX - 1; x <= groundX + 1; x++) {
            for (int y = groundY - 1; y <= groundY + 3; y++) {

                if (!IsPointOnGrid(tileGrid, x, y)) {
                    continue;
                }

                // if no tile is there
                if (tileGrid[x, y] == TileType.None) {
                    // make it a generic wall tile (the specific wall tile will be decided later)
                    tileGrid[x, y] = TileType.Wall;
                }
            }
        }
    }

    private void SetSpecificWallTileTypes(ref TileType[,] tileGrid) {

        // go through each tile in grid
        for (int x = 0; x < tileGrid.GetLength(0); x++) {
            for (int y = 0; y < tileGrid.GetLength(1); y++) {

                // if the tile is the generic wall type
                if (tileGrid[x, y] == TileType.Wall) {
                    tileGrid[x, y] = GetWallTileType(tileGrid, x, y);
                }
            }
        }
    }

    private TileType GetWallTileType(TileType[,] tileGrid, int x, int y) {
        if (!IsPointOnGrid(tileGrid, x, y)) {
            Debug.LogError("Point is not in grid!");
            return default;
        }

        //... if the tile below is ground
        if (GetTileAtPos(tileGrid, x, y - 1) == TileType.Ground) {
            return TileType.LowerTopWall;
        }

        //... if the tile below is wall and below that is ground
        if (IsWallTile(tileGrid, x, y - 1) &&
            GetTileAtPos(tileGrid, x, y - 2) == TileType.Ground) {

            return TileType.MiddleTopWall;
        }

        //... if 2 tiles below are walls and below that is ground
        if (IsWallTile(tileGrid, x, y - 1) &&
            IsWallTile(tileGrid, x, y - 2) &&
            GetTileAtPos(tileGrid, x, y - 3) == TileType.Ground) {

            // it can either be UpperTopWall, InnerTopRightCorner, InnerTopLeftCorner

            //... if 2 down and 1 left is ground
            if (GetTileAtPos(tileGrid, x - 1, y - 2) == TileType.Ground) {
                return TileType.InnerTopRightCorner;
            }

            //... if 2 down and 1 right is ground
            if (GetTileAtPos(tileGrid, x + 1, y - 2) == TileType.Ground) {
                return TileType.InnerTopLeftCorner;
            }

            return TileType.UpperTopWall;
        }


        bool threeBelowAreWalls = IsWallTile(tileGrid, x, y - 1) &&
            IsWallTile(tileGrid, x, y - 2) &&
            IsWallTile(tileGrid, x, y - 3);

        bool threeToLeftGoingDownAreWalls = IsWallTile(tileGrid, x - 1, y) &&
            IsWallTile(tileGrid, x - 1, y - 1) &&
            IsWallTile(tileGrid, x - 1, y - 2);

        if (threeBelowAreWalls &&
            threeToLeftGoingDownAreWalls &&
            GetTileAtPos(tileGrid, x - 1, y - 3) == TileType.Ground) {
            return TileType.OuterTopRightCorner;
        }

        bool threeToRightGoingDownAreWalls = IsWallTile(tileGrid, x + 1, y) &&
            IsWallTile(tileGrid, x + 1, y - 1) &&
            IsWallTile(tileGrid, x + 1, y - 2);

        if (threeBelowAreWalls &&
            threeToRightGoingDownAreWalls &&
            GetTileAtPos(tileGrid, x + 1, y - 3) == TileType.Ground) {
            return TileType.OuterTopLeftCorner;
        }


        //... if tile above is ground
        if (GetTileAtPos(tileGrid, x, y + 1) == TileType.Ground) {

            // it can either be BottomWall, InnerBottomRightCorner, InnerBottomLeftCorner

            //... if tile to left is ground
            if (GetTileAtPos(tileGrid, x - 1, y) == TileType.Ground) {
                return TileType.InnerBottomRightCorner;
            }

            //... if tile to right is ground
            if (GetTileAtPos(tileGrid, x + 1, y) == TileType.Ground) {
                return TileType.InnerBottomLeftCorner;
            }

            return TileType.BottomWall;
        }

        //... if above is wall, to the left is wall, and to the left and up is ground
        if (IsWallTile(tileGrid, x, y + 1) &&
            IsWallTile(tileGrid, x - 1, y) &&
            GetTileAtPos(tileGrid, x - 1, y + 1) == TileType.Ground) {

            return TileType.OuterBottomRightCorner;
        }

        //... if above is wall, to the right is wall, and to the right and up is ground
        if (IsWallTile(tileGrid, x, y + 1) &&
            IsWallTile(tileGrid, x + 1, y) &&
            GetTileAtPos(tileGrid, x + 1, y + 1) == TileType.Ground) {

            return TileType.OuterBottomLeftCorner;
        }

        // check if side walls last because makes logic easier

        //... if any of the three tiles to the left going down are ground
        if (GetTileAtPos(tileGrid, x - 1, y) == TileType.Ground ||
            GetTileAtPos(tileGrid, x - 1, y - 1) == TileType.Ground ||
            GetTileAtPos(tileGrid, x - 1, y - 2) == TileType.Ground) {
            return TileType.RightWall;
        }

        //... if any of the three tiles to the right going down are ground
        if (GetTileAtPos(tileGrid, x + 1, y) == TileType.Ground ||
            GetTileAtPos(tileGrid, x + 1, y - 1) == TileType.Ground ||
            GetTileAtPos(tileGrid, x + 1, y - 2) == TileType.Ground) {
            return TileType.LeftWall;
        }

        Vector3Int tilePos = GridToTilemapPos(groundTilemap, new Vector2(x, y));
        TileSet tileSet = tileSets.First(t => t.EnvironmentType == EnvironmentType.Stone);
        topWallsTilemap.SetTile(tilePos, GetRandomTileFromType(tileSet, TileType.Ground));

        Debug.LogError($"Could not find which wall type should be used at {x}, {y}! Placed ground tile there.");



        return TileType.Wall;
    }

    private bool IsWallTile(TileType[,] tileGrid, int x, int y) {
        TileType tileType = GetTileAtPos(tileGrid, x, y);
        bool isWallTile = tileType != TileType.Ground && tileType != TileType.None;
        return isWallTile;
    }

    private void PlaceTiles(TileType[,] tileGrid, EnvironmentType environmentType, Tilemap groundTilemap, Tilemap topWallsTilemap, Tilemap botWallsTilemap) {

        TileSet tileSet = tileSets.First(t => t.EnvironmentType == environmentType);

        // go through each tile in grid
        for (int x = 0; x < tileGrid.GetLength(0); x++) {
            for (int y = 0; y < tileGrid.GetLength(1); y++) {

                TileType tileType = tileGrid[x, y];

                Vector3Int tilePos = GridToTilemapPos(groundTilemap, new Vector2(x, y));
                Tile tile = GetRandomTileFromType(tileSet, tileType);
                Tilemap tilemap = GetTilemapFromTileType(tileType, groundTilemap, topWallsTilemap, botWallsTilemap);

                tilemap.SetTile(tilePos, tile);
            }
        }
    }

    private Tile GetRandomTileFromType(TileSet tileSet, TileType tileType) {

        if (tileType == TileType.None)
            return null;

        if (tileType == TileType.Ground)
            return tileSet.GroundTiles.RandomItem();

        if (tileType == TileType.LowerTopWall)
            return tileSet.LowerTopWalls.RandomItem();

        if (tileType == TileType.MiddleTopWall)
            return tileSet.MiddleTopWalls.RandomItem();

        if (tileType == TileType.UpperTopWall)
            return tileSet.UpperTopWalls.RandomItem();

        if (tileType == TileType.BottomWall)
            return tileSet.BottomWalls.RandomItem();

        if (tileType == TileType.LeftWall)
            return tileSet.LeftWalls.RandomItem();

        if (tileType == TileType.RightWall)
            return tileSet.RightWalls.RandomItem();

        if (tileType == TileType.InnerTopLeftCorner)
            return tileSet.InnerTopLeftCorners.RandomItem();

        if (tileType == TileType.InnerTopRightCorner)
            return tileSet.InnerTopRightCorners.RandomItem();

        if (tileType == TileType.InnerBottomLeftCorner)
            return tileSet.InnerBottomLeftCorners.RandomItem();

        if (tileType == TileType.InnerBottomRightCorner)
            return tileSet.InnerBottomRightCorners.RandomItem();

        if (tileType == TileType.OuterTopLeftCorner)
            return tileSet.OuterTopLeftCorners.RandomItem();

        if (tileType == TileType.OuterTopRightCorner)
            return tileSet.OuterTopRightCorners.RandomItem();

        if (tileType == TileType.OuterBottomLeftCorner)
            return tileSet.OuterBottomLeftCorners.RandomItem();

        if (tileType == TileType.OuterBottomRightCorner)
            return tileSet.OuterBottomRightCorners.RandomItem();

        // If the tileType doesn't match any of the above, return a default tile (optional)
        return null;
    }

    private Tilemap GetTilemapFromTileType(TileType tileType, Tilemap groundTilemap, Tilemap topWallsTilemap, Tilemap botWallsTilemap) {

        TileType[] groundTiles = new TileType[] {
            TileType.Ground,
            TileType.LowerTopWall,
        };

        TileType[] topWallTiles = new TileType[] {
            TileType.MiddleTopWall,
            TileType.UpperTopWall,
            TileType.InnerTopLeftCorner,
            TileType.InnerTopRightCorner,
            TileType.OuterTopLeftCorner,
            TileType.OuterTopRightCorner,
        };

        TileType[] bottomWallTiles = new TileType[] {
            TileType.BottomWall,
            TileType.LeftWall,
            TileType.RightWall,
            TileType.InnerBottomLeftCorner,
            TileType.InnerBottomRightCorner,
            TileType.OuterBottomLeftCorner,
            TileType.OuterBottomRightCorner
        };


        if (groundTiles.Contains(tileType)) {
            return groundTilemap;
        }
        else if (topWallTiles.Contains(tileType)) {
            return topWallsTilemap;
        }
        else if (bottomWallTiles.Contains(tileType)) {
            return botWallsTilemap;
        }
        else if (tileType == TileType.None) {
            return groundTilemap;
        }


        Debug.LogError($"Tile type: {tileType} does not fit into any tilemap!");
        return null;
    }


    // if outside range then tiletype is none
    private TileType GetTileAtPos(TileType[,] tileGrid, int x, int y) {
        if (IsPointOnGrid(tileGrid, x, y)) {
            return tileGrid[x, y];
        }
        else {
            return TileType.None;
        }
    }

    private bool IsPointOnGrid(TileType[,] tileGrid, int x, int y) {
        bool withinX = x >= 0 && x < tileGrid.GetLength(0);
        bool withinY = y >= 0 && y < tileGrid.GetLength(1);
        return withinX && withinY;
    }

    private Vector3Int GridToTilemapPos(Tilemap tilemap, Vector2 gridPos) {
        BoundsInt bounds = tilemap.cellBounds;
        return new Vector3Int((int)gridPos.x + bounds.min.x - 1, (int)gridPos.y + bounds.min.y - 1, 0);
    }

    private void PrintTileGrid(TileType[,] tileGrid) {
        int maxTileNameLength = tileGrid.Cast<TileType>()
            .Select(t => t.ToString().Length)
            .Max();

        string gridStr = "";
        for (int y = 0; y < tileGrid.GetLength(1); y++) {
            for (int x = 0; x < tileGrid.GetLength(0); x++) {
                // Format the tile string with uniform spacing
                gridStr += $"{tileGrid[x, y].ToString().PadRight(maxTileNameLength)}";
            }
            gridStr += "\n";
        }
        Debug.Log(gridStr);
    }

    private void PrintTileGridInitials(TileType[,] tileGrid) {
        int maxTileNameLength = tileGrid.Cast<TileType>()
            .Select(t => GetTileInitials(t))
            .Max(s => s.Length);

        string gridStr = "";
        for (int y = tileGrid.GetLength(1) - 1; y >= 0; y--) {
            for (int x = 0; x < tileGrid.GetLength(0); x++) {
                // Format the tile string with uniform spacing
                gridStr += $"{GetTileInitials(tileGrid[x, y]).PadRight(maxTileNameLength)}";
            }
            gridStr += "\n";
        }
        Debug.Log(gridStr);
    }

    private string GetTileInitials(TileType tileType) {
        // Get the capital letters from the tile type name
        return string.Concat(tileType.ToString().Where(char.IsUpper));
    }

    private enum TileType {

        None,
        Wall,

        Ground,
        LowerTopWall,
        MiddleTopWall,
        UpperTopWall,
        BottomWall,
        LeftWall,
        RightWall,

        InnerTopLeftCorner,
        InnerTopRightCorner,
        InnerBottomLeftCorner,
        InnerBottomRightCorner,
        OuterTopLeftCorner,
        OuterTopRightCorner,
        OuterBottomLeftCorner,
        OuterBottomRightCorner,
    }

    [Serializable]
    private struct TileSet {
        public EnvironmentType EnvironmentType;

        public Tile[] GroundTiles;

        public Tile[] LowerTopWalls;
        public Tile[] MiddleTopWalls;
        public Tile[] UpperTopWalls;
        public Tile[] BottomWalls;
        public Tile[] LeftWalls;
        public Tile[] RightWalls;

        public Tile[] InnerTopLeftCorners;
        public Tile[] InnerTopRightCorners;
        public Tile[] InnerBottomLeftCorners;
        public Tile[] InnerBottomRightCorners;

        public Tile[] OuterTopLeftCorners;
        public Tile[] OuterTopRightCorners;
        public Tile[] OuterBottomLeftCorners;
        public Tile[] OuterBottomRightCorners;
    }
}

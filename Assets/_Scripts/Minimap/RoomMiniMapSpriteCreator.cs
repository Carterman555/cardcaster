#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;

public class RoomMiniMapSpriteCreator {

    private Tilemap[] tilemaps;
    private int minTileX, maxTileX, minTileY, maxTileY;
    private Texture2D miniMapRoomImage;

    private string fileName;
    private string assetFilePath;

    private int pixelsPerUnit = 4;

    public void CreateMiniMapSprite(string filename, Tilemap[] tilemaps) {
        this.fileName = filename;
        this.tilemaps = tilemaps;

        SetMinAndMaxes();

        CreateImage();
    }

    private void SetMinAndMaxes() {

        // Find the minimum and maximum points
        foreach (Tilemap tilemap in tilemaps) {
            for (int x = 0; x < tilemap.size.x; x++) {
                for (int y = 0; y < tilemap.size.y; y++) {
                    Vector3Int pos = new(-x, -y, 0);
                    if (tilemap.GetSprite(pos) != null) {
                        if (minTileX > pos.x) {
                            minTileX = pos.x;
                        }
                        if (minTileY > pos.y) {
                            minTileY = pos.y;
                        }
                    }

                    pos = new Vector3Int(x, y, 0);
                    if (tilemap.GetSprite(pos) != null) {
                        if (maxTileX < pos.x) {
                            maxTileX = pos.x;
                        }
                        if (maxTileY < pos.y) {
                            maxTileY = pos.y;
                        }
                    }
                }
            }
        }
    }

    private void CreateImage() {

        int roomTileWidth = maxTileX - minTileX;
        int roomTileHeight = maxTileY - minTileY;

        int roomPixelWidth = roomTileWidth * pixelsPerUnit;
        int roomPixelHeight = roomTileHeight * pixelsPerUnit;

        Texture2D imageInProgress = new(roomPixelWidth, roomPixelHeight);

        // Assign entire image as invisible
        Color[] transparent = new Color[imageInProgress.width * imageInProgress.height];
        for (int i = 0; i < transparent.Length; i++) {
            transparent[i] = new Color(0f, 0f, 0f, 0f);
        }
        imageInProgress.SetPixels(0, 0, imageInProgress.width, imageInProgress.height, transparent);

        Color spriteColor = Color.white;
        Color[] colorArray = new Color[pixelsPerUnit * pixelsPerUnit];
        for (int i = 0; i < colorArray.Length; i++) {
            colorArray[i] = spriteColor;
        }

        // Assign respective pixels to each block
        for (int tileX = minTileX; tileX < maxTileX; tileX++) {
            for (int tileY = minTileY; tileY < maxTileY; tileY++) {
                if (TileAtPos(tileX, tileY)) {

                    // Map pixels so that minX = 0 and minY = 0
                    int pixelX = (tileX - minTileX) * pixelsPerUnit;
                    int pixelY = (tileY - minTileY) * pixelsPerUnit;

                    imageInProgress.SetPixels(pixelX, pixelY, pixelsPerUnit, pixelsPerUnit, colorArray);
                }
            }
        }
        imageInProgress.Apply();

        // Store the ready image texture
        miniMapRoomImage = imageInProgress;

        ExportAsPng(fileName);
    }

    // if any of the tilemaps have a tile at the given pos
    private bool TileAtPos(int x, int y) {
        Vector3Int pos = new Vector3Int(x, y);
        bool tileAtPos = tilemaps.Any(t => t.GetSprite(pos) != null);
        return tileAtPos;
    }

    // Method to export as PNG
    public void ExportAsPng(string name) {
        byte[] bytes = miniMapRoomImage.EncodeToPNG();
        var dirPath = "/Visual/Sprites/RoomMinimap/";

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        string filePath = dirPath + name + ".png";
        assetFilePath = "Assets" + filePath;

        File.WriteAllBytes(Application.dataPath + filePath, bytes);
        AssetDatabase.Refresh();

        SetupSprite(filePath);

        miniMapRoomImage = null;
    }

    private void SetupSprite(string filePath) {
        TextureImporter importer = AssetImporter.GetAtPath("Assets" + filePath) as TextureImporter;
        if (importer != null) {
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    public string GetFilePath() {
        return assetFilePath;
    }

    public Vector2 GetTileMapsCenter() {
        Vector2 center = new((minTileX + maxTileX) / 2, (minTileY + maxTileY) / 2);
        return center;
    }
}
#endif
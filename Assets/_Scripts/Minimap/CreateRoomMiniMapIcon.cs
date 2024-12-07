#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class CreateRoomMiniMapIcon : MonoBehaviour {
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private int minX, maxX, minY, maxY;
    private Texture2D image;

    public void Pack() {
        Sprite anySprite = null;

        // Find the minimum and maximum points
        foreach (Tilemap tilemap in tilemaps) {
            for (int x = 0; x < tilemap.size.x; x++) {
                for (int y = 0; y < tilemap.size.y; y++) {
                    Vector3Int pos = new(-x, -y, 0);
                    if (tilemap.GetSprite(pos) != null) {
                        // Select any sprite to determine dimensions later
                        anySprite = tilemap.GetSprite(pos);
                        if (minX > pos.x) {
                            minX = pos.x;
                        }
                        if (minY > pos.y) {
                            minY = pos.y;
                        }
                    }

                    pos = new Vector3Int(x, y, 0);
                    if (tilemap.GetSprite(pos) != null) {
                        if (maxX < pos.x) {
                            maxX = pos.x;
                        }
                        if (maxY < pos.y) {
                            maxY = pos.y;
                        }
                    }
                }
            }
        }
        

        // Determine sprite size in pixels
        float width = anySprite.rect.width;
        float height = anySprite.rect.height;

        // Create a texture with size multiplied by the number of cells
        Texture2D createdImage = new((int)width * tilemap.size.x, (int)height * tilemap.size.y);

        // Assign entire image as invisible
        Color[] transparent = new Color[createdImage.width * createdImage.height];
        for (int i = 0; i < transparent.Length; i++) {
            transparent[i] = new Color(0f, 0f, 0f, 0f);
        }
        createdImage.SetPixels(0, 0, createdImage.width, createdImage.height, transparent);

        // Assign respective pixels to each block
        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                if (tilemap.GetSprite(new Vector3Int(x, y, 0)) != null) {
                    // Map pixels so that minX = 0 and minY = 0
                    createdImage.SetPixels((x - minX) * (int)width, (y - minY) * (int)height, (int)width, (int)height,
                        GetCurrentSprite(tilemap.GetSprite(new Vector3Int(x, y, 0))).GetPixels());
                }
            }
        }
        createdImage.Apply();

        // Store the ready image texture
        image = createdImage;
    }

    // Method to obtain the cropped sprite as configured
    Texture2D GetCurrentSprite(Sprite sprite) {
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                         (int)sprite.textureRect.y,
                                         (int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);
        Texture2D texture = new((int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    // Method to export as PNG
    public void ExportAsPng(string name) {
        byte[] bytes = image.EncodeToPNG();
        var dirPath = Application.dataPath + "/Exported Tilemaps/";

        if (!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        File.WriteAllBytes(dirPath + name + ".png", bytes);
        AssetDatabase.Refresh();
        image = null;
    }
}
#endif
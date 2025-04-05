using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EffectsHelper {
    public static void CreateColoredParticles(this ParticleSystem particleSystemPrefab, Vector2 position, Color color) {

        ParticleSystem newParticleSystem = particleSystemPrefab.Spawn(position, Containers.Instance.Effects);

        ParticleSystem[] allParticles = newParticleSystem.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particles in allParticles) {
            var main = particles.main;
            main.startColor = color;
        }
    }

    public static Mesh GetMeshFromSprite(this Sprite sprite) {

        // Create a new mesh
        Mesh mesh = new Mesh();

        // Get the sprite's texture
        Texture2D texture = sprite.texture;

        // Get pixel data from the sprite
        Color[] pixels = new Color[0];
        try {
            pixels = texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
        }
        catch {
            Debug.LogError("Need to allow Read/Write on " + sprite.name);
            return null;
        }

        // Create lists to store mesh data
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int width = (int)sprite.textureRect.width;
        int height = (int)sprite.textureRect.height;

        // Get the pivot in texture space (in pixels)
        Vector2 pivot = sprite.pivot; // The pivot is in pixels, based on the sprite's texture
        float pixelSize = 1 / sprite.pixelsPerUnit; // The size of each pixel in world units

        // Convert the pivot to world space offset
        Vector2 pivotOffset = new Vector2(pivot.x / sprite.pixelsPerUnit, pivot.y / sprite.pixelsPerUnit);

        // Loop through each pixel in the texture
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Get the color of the current pixel
                Color pixelColor = pixels[x + y * width];

                // Only add opaque (non-transparent) pixels to the mesh
                if (pixelColor.a > 0f) // Check the alpha value to exclude transparent pixels
                {
                    // Create four vertices for this "pixel" (since we want to create a quad per pixel)
                    Vector3 bottomLeft = new Vector3(x * pixelSize, y * pixelSize, 0) - (Vector3)pivotOffset;
                    Vector3 bottomRight = new Vector3((x + 1) * pixelSize, y * pixelSize, 0) - (Vector3)pivotOffset;
                    Vector3 topLeft = new Vector3(x * pixelSize, (y + 1) * pixelSize, 0) - (Vector3)pivotOffset;
                    Vector3 topRight = new Vector3((x + 1) * pixelSize, (y + 1) * pixelSize, 0) - (Vector3)pivotOffset;

                    // Add vertices
                    int vertexIndex = vertices.Count;
                    vertices.Add(bottomLeft);
                    vertices.Add(bottomRight);
                    vertices.Add(topLeft);
                    vertices.Add(topRight);

                    // Add triangles (two triangles for each quad)
                    triangles.Add(vertexIndex);       // Bottom-left triangle
                    triangles.Add(vertexIndex + 2);   // Top-left
                    triangles.Add(vertexIndex + 1);   // Bottom-right

                    triangles.Add(vertexIndex + 1);   // Bottom-right triangle
                    triangles.Add(vertexIndex + 2);   // Top-left
                    triangles.Add(vertexIndex + 3);   // Top-right

                    // Add UVs (normalized texture coordinates)
                    Vector2 uvBottomLeft = new Vector2((float)x / width, (float)y / height);
                    Vector2 uvBottomRight = new Vector2((float)(x + 1) / width, (float)y / height);
                    Vector2 uvTopLeft = new Vector2((float)x / width, (float)(y + 1) / height);
                    Vector2 uvTopRight = new Vector2((float)(x + 1) / width, (float)(y + 1) / height);

                    uvs.Add(uvBottomLeft);
                    uvs.Add(uvBottomRight);
                    uvs.Add(uvTopLeft);
                    uvs.Add(uvTopRight);
                }
            }
        }

        // Assign the lists to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // Recalculate mesh properties for optimization
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    public static SpriteRenderer GetBiggestRenderer(this Transform transform) {
        SpriteRenderer[] spriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
        
        if (spriteRenderers.Length == 0) {
            Debug.LogError("Couldn't find any sprite renderers!");
            return null;
        }

        return spriteRenderers.OrderByDescending(spriteRenderer => spriteRenderer.sprite.bounds.size.magnitude).FirstOrDefault();
    }
}

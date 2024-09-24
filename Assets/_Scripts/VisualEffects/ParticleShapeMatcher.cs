using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class ParticleShapeMatcher : MonoBehaviour {

    private ParticleSystem[] particles;

    private float[] defaultEmissionRates;

    private void Awake() {
        particles = GetComponentsInChildren<ParticleSystem>();

        defaultEmissionRates = new float[particles.Length];
        for (int i = 0; i < particles.Length; i++) {
            var emissionModule = particles[i].emission;
            defaultEmissionRates[i] = emissionModule.rateOverTime.constant;
        }
        
    }

    void OnEnable() {

        SpriteRenderer[] spriteRenderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        Sprite biggestSprite = GetBiggestSprite(spriteRenderers);

        for (int i = 0; i < particles.Length; i++) {

            // Assign the mesh to the MeshFilter component
            var shapeModule = particles[i].shape;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = GetMeshFromSprite(biggestSprite);

            // change emission rate based on sprite size
            var emissionModule = particles[i].emission;
            emissionModule.rateOverTime = defaultEmissionRates[i] * biggestSprite.bounds.size.magnitude;
        }
        
    }
   
    private Sprite GetBiggestSprite(SpriteRenderer[] spriteRenderers) {
        return spriteRenderers.OrderBy(spriteRenderer => spriteRenderer.sprite.bounds.size).FirstOrDefault().sprite;
    }

    private Mesh GetMeshFromSprite(Sprite sprite) {

        // Create a new mesh
        Mesh mesh = new Mesh();

        // Get the sprite's texture
        Texture2D texture = sprite.texture;

        // Get pixel data from the sprite
        Color[] pixels = texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);

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
}

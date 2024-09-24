using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleShapeMatcher : MonoBehaviour {

    [SerializeField] private int samplingResolution = 32;
    [SerializeField] private float particleSize = 0.1f;

    private ParticleSystem particles;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.ShapeModule shapeModule;

    private SpriteRenderer spriteRenderer;
    private Texture2D shapeTexture;

    private void Awake() {
        particles = GetComponent<ParticleSystem>();
        mainModule = particles.main;
        shapeModule = particles.shape;
    }

    private void OnEnable() {
        spriteRenderer = GetBiggestRenderer();

        if (spriteRenderer == null) {
            Debug.LogError("Could not find sprite!");
            return;
        }
        UpdateParticleShape();
    }

    void UpdateParticleShape() {
        Sprite sprite = spriteRenderer.sprite;
        Texture2D spriteTexture = sprite.texture;

        // Create a new texture for the particle system shape
        shapeTexture = new Texture2D(samplingResolution, samplingResolution, TextureFormat.Alpha8, false);

        // Sample the sprite texture
        for (int y = 0; y < samplingResolution; y++) {
            for (int x = 0; x < samplingResolution; x++) {
                float u = (float)x / (samplingResolution - 1);
                float v = (float)y / (samplingResolution - 1);

                // Convert UV to sprite texture coordinates
                Vector2 pixelPos = new Vector2(
                    Mathf.Lerp(sprite.textureRect.x, sprite.textureRect.xMax, u),
                    Mathf.Lerp(sprite.textureRect.y, sprite.textureRect.yMax, v)
                );

                // Sample the sprite texture
                Color pixelColor = spriteTexture.GetPixelBilinear(pixelPos.x / spriteTexture.width, pixelPos.y / spriteTexture.height);

                // Set the alpha value in the shape texture
                shapeTexture.SetPixel(x, y, new Color(1, 1, 1, pixelColor.a));
            }
        }

        shapeTexture.Apply();

        // Apply the texture to the particle system shape
        shapeModule.texture = shapeTexture;
        shapeModule.textureClipChannel = ParticleSystemShapeTextureChannel.Alpha;

        // Adjust particle system properties
        shapeModule.scale = spriteRenderer.bounds.size;
        //mainModule.startSize = particleSize;
    }

    private SpriteRenderer GetBiggestRenderer() {
        SpriteRenderer[] renderers = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer biggestRenderer = renderers.OrderByDescending(r => r.bounds.size.magnitude).FirstOrDefault();
        return biggestRenderer;
    }
}
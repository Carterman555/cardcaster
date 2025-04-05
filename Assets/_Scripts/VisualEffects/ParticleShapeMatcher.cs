using UnityEngine;

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

        SpriteRenderer biggestRenderer = transform.parent.GetBiggestRenderer();

        for (int i = 0; i < particles.Length; i++) {

            // Assign the mesh to the MeshFilter component
            var shapeModule = particles[i].shape;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.meshShapeType = ParticleSystemMeshShapeType.Triangle;
            shapeModule.mesh = biggestRenderer.sprite.GetMeshFromSprite();

            // change emission rate based on sprite size
            var emissionModule = particles[i].emission;
            emissionModule.rateOverTime = defaultEmissionRates[i] * Mathf.Pow(biggestRenderer.sprite.bounds.size.magnitude, 2);
        }

        // parent self to visual in order to match the transform to match the sprite shape and make sure the particles emit
        // from the visual
        //transform.SetParent(biggestRenderer.transform);
        //transform.localPosition = Vector3.zero;
        //transform.localRotation = Quaternion.identity;
        //transform.localScale = Vector3.one;
    }
   
    
}

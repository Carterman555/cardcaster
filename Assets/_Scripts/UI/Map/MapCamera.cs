using UnityEngine;
using System.Linq;

public class MapCamera : MonoBehaviour {

    private Camera cam;

    private float minX, minY, maxX, maxY;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void OnEnable() {
        RoomGenerator.OnCompleteGeneration += ResizeAndPositionMap;
    }
    private void OnDisable() {
        RoomGenerator.OnCompleteGeneration -= ResizeAndPositionMap;
    }

    private void Update() {
        Vector2 mouseScreenPos = Camera.main.WorldToScreenPoint(MouseTracker.Instance.transform.position);
    }

    private void ResizeAndPositionMap() {
        SetPositions();
        CenterCamera();
        FitCameraSizeToLevel();
        UpdateMapTextureSize();
        MoveMapTextureToCenterPlayer();
    }

    private void SetPositions() {
        Transform[] rooms = FindObjectsOfType<Room>().Select(r => r.transform).ToArray();

        maxX = rooms.Max(r => r.position.x);
        maxY = rooms.Max(r => r.position.y);

        minX = rooms.Min(r => r.position.x);
        minY = rooms.Min(r => r.position.y);
    }

    private void CenterCamera() {
        
        float middleX = (maxX + minX) / 2;
        float middleY = (maxY + minY) / 2;

        Vector3 centerOfRooms = new Vector3(middleX, middleY, -10);

        transform.position = centerOfRooms;
    }

    private void FitCameraSizeToLevel() {

        float padding = 20f;
        float mult = 0.58f;

        float length = (maxX - minX + padding) * mult;
        float height = (maxY - minY + padding) * mult;

        float camSize = Mathf.Max(length, height);

        cam.orthographicSize = camSize;
    }

    [SerializeField] private RectTransform mapTextureTransform;

    private void UpdateMapTextureSize() {

        float textureSizeMult = 18f;

        // the bigger the cam size, the bigger the map texture, so the objects on the map remain the same size
        float textureSize =  cam.orthographicSize * textureSizeMult;
        mapTextureTransform.sizeDelta = new Vector2(textureSize, textureSize);
    }

    // This method repositions the map texture based on the player's normalized position to ensure the player is centered
    // within the visible map area.
    private void MoveMapTextureToCenterPlayer() {

        float length = maxX - minX;
        float height = maxY - minY;

        //... use heighest dimension because map texture is a square based on the heighest dimension
        float maxDimension = Mathf.Max(length, height);

        Vector2 playerPosRelToCenter = PlayerMovement.Instance.CenterPos - transform.position;

        // each coord ranges from -1 to 1 depending on where on the map the player is
        float normalizedPlayerXPosition = (Mathf.InverseLerp(-maxDimension, maxDimension, playerPosRelToCenter.x) * 4f) - 2f;
        float normalizedPlayerYPosition = (Mathf.InverseLerp(-maxDimension, maxDimension, playerPosRelToCenter.y) * 4f) - 2f;

        //... I don't know why -0.4 works, but it's the value that equalizes the normalized player pos and the normalized map
        //... render texture position. (normalized player pos) * (-0.4) = (normalized render texture position)
        //... It's negative because if the player is on the right side of the map, the map texture needs to move to the left to
        //... show the right side of the map where the player is
        float normalizeMult = -0.4f;

        float normalizedTextureXPos = normalizedPlayerXPosition * normalizeMult;
        float normalizedTextureYPos = normalizedPlayerYPosition * normalizeMult;

        float textureXPos = normalizedTextureXPos * mapTextureTransform.rect.width;
        float textureYPos = normalizedTextureYPos * mapTextureTransform.rect.height;

        mapTextureTransform.anchoredPosition = new Vector3(textureXPos, textureYPos);
    }
}

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers {

    public static void DestroyChildren(this Transform t) {
        foreach (Transform child in t) UnityEngine.Object.Destroy(child.gameObject);
    }

    public static void ReturnChildrenToPool(this Transform t) {
        foreach (Transform child in t) child.gameObject.ReturnToPool();
    }

    public static void SetActiveChildren(this Transform t, bool active) {
        foreach (Transform child in t) child.gameObject.SetActive(active);
    }

    public static bool GetAnyActiveChildren(this Transform t) {
        foreach (Transform child in t) {
            if (child.gameObject.activeSelf) return true;
        }
        return false;
    }

    public static void Fade(this SpriteRenderer spriteRenderer, float value) {
        Color color = spriteRenderer.color;
        color.a = value;
        spriteRenderer.color = color;
    }
    public static void Fade(this Image image, float value) {
        Color color = image.color;
        color.a = value;
        image.color = color;
    }
    public static void Fade(this TextMeshPro text, float value) {
        Color color = text.color;
        color.a = value;
        text.color = color;
    }

    public static void ChangeHue(this SpriteRenderer spriteRenderer, Color targetColor, float amount) {

        float alpha = spriteRenderer.color.a;

        // Convert the original color and target color to HSV
        Color.RGBToHSV(spriteRenderer.color, out float h1, out float s1, out float v1);
        Color.RGBToHSV(targetColor, out float h2, out float s2, out float v2);

        // Calculate the shortest direction towards the target hue
        float diff = Mathf.DeltaAngle(h1 * 360f, h2 * 360f) / 360f;

        // Adjust the hue by the specified amount towards the target hue
        float newHue = h1 + diff * amount;
        if (newHue < 0) newHue += 1;
        if (newHue > 1) newHue -= 1;

        // Convert the new HSV color back to RGB
        Color newColor = Color.HSVToRGB(newHue, s1, v1);
        newColor.a = alpha;
        spriteRenderer.color = newColor;
    }

    public static void RemoveWithCheck<T>(this List<T> list, T item) {
        if (list.Contains(item)) list.Remove(item);
    }

    public static T RandomItem<T>(this T[] list) {
        int randomIndex = UnityEngine.Random.Range(0, list.Length);
        return list[randomIndex];
    }
    public static T RandomItem<T>(this List<T> list) {
        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static bool IsMouseOverLayer(int layerMask) {
        // Cast a ray from the camera to the mouse position and check if null
        return Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, 1 << layerMask).collider != null;
    }

    public static Quaternion DirectionToRotation(this Vector2 direction) {
        float angleRadians = Mathf.Atan2(direction.y, direction.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angleDegrees);
    }

    public static Vector2 RotationToDirection(this float degrees) {
        return Quaternion.Euler(0, 0, degrees) * Vector2.up;
    }

    public static Vector2 RotateDirection(this Vector2 direction, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * direction;
    }

    public static float GetClosestDirection(Vector3 dir1, Vector3 dir2) {
        float _angle = Vector2.SignedAngle(dir1, dir2);
        return -Mathf.Sign(_angle);
    }

    public static float RotateAroundCenter(
        this Transform objectToRotate,
        Vector2 direction,
        float rotationOffset = 0f) {
        // Get the sprite renderer
        SpriteRenderer spriteRenderer = objectToRotate.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return 0f;

        // Calculate sprite's center in local coordinates
        Vector2 spriteCenter = spriteRenderer.sprite.bounds.center;

        // Get the current pivot offset in world space
        Vector2 pivotOffset = (Vector2)objectToRotate.TransformPoint(spriteCenter) - (Vector2)objectToRotate.position;

        // Calculate rotation angle from direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        // Calculate how the offset should change after rotation
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 rotatedOffset = new Vector2(
            pivotOffset.magnitude * Mathf.Cos(angleRad),
            pivotOffset.magnitude * Mathf.Sin(angleRad)
        );

        // Update rotation
        objectToRotate.rotation = Quaternion.Euler(0, 0, angle);

        // Update position to maintain the center point
        Vector2 newPosition = (Vector2)objectToRotate.position + (rotatedOffset - pivotOffset);
        objectToRotate.position = newPosition;

        

        return angle;
    }

    public static Transform GetClosest(this Transform self, Transform[] objects) {
        float closestDistance = float.PositiveInfinity;
        Transform closestOb = null;
        foreach (Transform ob in objects) {
            float distance = Vector3.Distance(self.position, ob.position);

            if (ob != self && distance < closestDistance) {
                closestOb = ob;
                closestDistance = distance;
            }
        }

        return closestOb;
    }

    public static int FacingInt(Vector3 current, Vector3 target) => (int)Mathf.Sign(target.x - current.x);

    public static float PercentToMult(this float percent) {
        float mult = 1 + (percent / 100);
        return mult;
    }

    public static Vector3 WorldToCanvasPosition(Canvas canvas, Vector3 worldPos) {
        RectTransform _canvasRect = canvas.GetComponent<RectTransform>();

        Vector2 _viewportPosition = Camera.main.WorldToViewportPoint(worldPos);
        return new Vector2(
        (_viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
        (_viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f));
    }

    public static bool IsMouseOverUI() {
        if (EventSystem.current == null) EventSystem.current = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).GetComponent<EventSystem>();

        PointerEventData pointerEventData = new(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for (int i = 0; i < raycastResultList.Count; i++) {
            int UILayer = 5;
            if (raycastResultList[i].gameObject.layer == UILayer) return true;
        }
        return false;
    }

    public static bool IsMouseOver(this GameObject gameObject) {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        return hits.Any(hit => hit.collider != null && hit.collider.gameObject == gameObject);
    }

    public static string ToPrettyString(this Enum value) {
        string original = value.ToString();
        return Regex.Replace(original, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1");
    }

    public static bool ContainsLayer(this LayerMask mask, int layer) {
        return (mask.value & (1 << layer)) != 0;
    }

    public static Tween ShrinkThenDestroy(this Transform transform, float duration = 0.3f) {

        Vector3 originalScale = transform.localScale;

        return transform.DOScale(Vector3.zero, duration).SetEase(Ease.InSine).OnComplete(() => {
            transform.DOKill();
            transform.gameObject.TryReturnToPool();
            transform.localScale = originalScale;
        });
    }

    public static bool GameStopping() {
#if UNITY_EDITOR
        return !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
        return false;
#endif
    }

    public static void DrawRectangle(Vector3 center, Vector2 size) {
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;

        Vector3 topLeft = center + new Vector3(-halfWidth, halfHeight, 0f);
        Vector3 topRight = center + new Vector3(halfWidth, halfHeight, 0f);
        Vector3 bottomRight = center + new Vector3(halfWidth, -halfHeight, 0f);
        Vector3 bottomLeft = center + new Vector3(-halfWidth, -halfHeight, 0f);

        Debug.DrawLine(topLeft, topRight, Color.white);
        Debug.DrawLine(topRight, bottomRight, Color.white);
        Debug.DrawLine(bottomRight, bottomLeft, Color.white);
        Debug.DrawLine(bottomLeft, topLeft, Color.white);
    }

    public static void Print<T>(this List<T> list) {
        string str = "";
        foreach (T item in list) {
            str += item + ", ";
        }
        Debug.Log(str);
    }

    public static void Print<T>(this T[] array) {
        string str = "";
        foreach (T item in array) {
            str += item + ", ";
        }
        Debug.Log(str);
    }

    // -------------- Game Specific --------------


}

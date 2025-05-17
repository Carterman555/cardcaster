using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapIconContainerResizer : MonoBehaviour {

    private RectTransform rectTransform;

    [SerializeField] private float padding;

    [ContextMenu("Resize")]
    public void ResizeAndPosition() {

        rectTransform = GetComponent<RectTransform>();

        Vector2 min = Vector2.one * float.MaxValue;
        Vector2 max = Vector2.one * float.MinValue;

        Image[] iconImages = GetComponentsInChildren<Image>().Where(i => i != GetComponent<Image>()).ToArray();
        foreach (Image image in iconImages) {

            if (image.color.a == 0) {
                continue;
            }

            Vector3[] imageCorners = new Vector3[4];
            image.rectTransform.GetWorldCorners(imageCorners);

            Vector3 bottomLeftOfImage = imageCorners[0];
            Vector3 topRightOfImage = imageCorners[2];

            if (bottomLeftOfImage.x < min.x) {
                min.x = bottomLeftOfImage.x;
            }
            if (bottomLeftOfImage.y < min.y) {
                min.y = bottomLeftOfImage.y;
            }

            if (topRightOfImage.x > max.x) {
                max.x = topRightOfImage.x;
            }
            if (topRightOfImage.y > max.y) {
                max.y = topRightOfImage.y;
            }
        }

        
        rectTransform.sizeDelta = new Vector2((max.x - min.x) + (padding * 2), (max.y - min.y) + (padding * 2));

        Vector3[] containerCorners = new Vector3[4];
        rectTransform.GetWorldCorners(containerCorners);

        Vector2 offset = new Vector2((containerCorners[0].x - min.x) + padding, (containerCorners[0].y - min.y) + padding);

        foreach (Image iconImage in iconImages) {
            iconImage.rectTransform.anchoredPosition += offset;
        }
    }
}

using UnityEngine;

public static class Extensions {
    public static void ResetLocalAndAnchoredPosition(this RectTransform rectTransform){
        rectTransform.localPosition = Vector3.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
using UnityEngine;
using UnityEngine.UI;

public static class Extensions {
    public static void ResetLocalAndAnchoredPosition(this RectTransform rectTransform){
        rectTransform.localPosition = Vector3.zero;
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public static bool FactionIsEverything(this Faction faction){
        return (faction.HasFlag(Faction.Empire) && faction.HasFlag(Faction.Rebellion)) || faction == (Faction) ~0;
    }
}
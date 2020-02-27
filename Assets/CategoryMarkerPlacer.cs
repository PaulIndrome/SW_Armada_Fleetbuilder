using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryMarkerPlacer : MonoBehaviour
{
    [Header("Scene references")]
    public RectTransform markerTemplate;
    
    public void PlaceMarkers(params float[] places){
        for(int i = 0; i < places.Length + 1; i++){
            RectTransform newMarker = Instantiate<RectTransform>(markerTemplate, Vector3.zero, Quaternion.identity, transform);
            
            newMarker.ResetLocalAndAnchoredPosition();

            newMarker.anchorMax = new Vector2(1f, i == 0 ? 1 : places[i-1]);
            newMarker.anchorMin = new Vector2(0f, i < places.Length ? places[i] : 0f);
            
            Image image = newMarker.GetComponent<Image>();
            image.color = (i % 2 == 0 ? new Color(1, 0, 0, 0.25f) : new Color(0, 0, 1, 0.25f));

            newMarker.gameObject.SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryMarkerPlacer : MonoBehaviour
{
    [Header("Scene references")]
    public RectTransform markerTemplate;

    [ReadOnly, SerializeField] private List<RectTransform> markers;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        markers = new List<RectTransform>();
    }

    public void PlaceMarkers(params float[] places){
        RectTransform marker;
        for(int i = 0; i < places.Length + 1; i++){
            if(i + 1 > markers.Count || markers[i] == null){
                marker = Instantiate<RectTransform>(markerTemplate, Vector3.zero, Quaternion.identity, transform);
                markers.Add(marker);
            } else {
                marker = markers[i];
            }
            
            marker.ResetLocalAndAnchoredPosition();

            marker.anchorMax = new Vector2(1f, i == 0 ? 1 : places[i-1]);
            marker.anchorMin = new Vector2(0f, i < places.Length ? places[i] : 0f);
            
            Image image = marker.GetComponent<Image>();
            image.color = (i % 2 == 0 ? new Color(1, 0, 0, 0.25f) : new Color(0, 0, 1, 0.25f));

            marker.gameObject.SetActive(true);
        }
    }
}

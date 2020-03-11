using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CategoryHeader : MonoBehaviour
{
    private float headerSize = 50;
    private RectTransform rectTransform;
    private Canvas headerCanvas;
    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Slider columnSlider; 

    public Button buttonCategoryLeft, buttonCategoryRight;
    private Button[] columnButtons;
    

    public float HeaderSize => headerSize;
    public Slider ColumnSlider => columnSlider;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(headerText == null)
            headerText = GetComponentInChildren<TextMeshProUGUI>();

        rectTransform = GetComponent<RectTransform>();
        headerCanvas = GetComponent<Canvas>();

        headerSize = rectTransform.sizeDelta.y;
    }

    public void SetHeaderText(string textToSet){
        headerText.text = textToSet;
        gameObject.name = $"{transform.GetSiblingIndex().ToString("000")}-Category-{textToSet}-header";
    }

    public void ToggleHeaderCanvas(bool onOff){
        headerCanvas.enabled = onOff;
    }

    public void ResetTransform(){
        rectTransform.ResetLocalAndAnchoredPosition();
    }
}

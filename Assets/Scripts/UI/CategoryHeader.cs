using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CategoryHeader : MonoBehaviour
{
    private float headerSize = 50;
    private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI headerText;
    

    public float HeaderSize => headerSize;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(headerText == null)
            headerText = GetComponentInChildren<TextMeshProUGUI>();

        rectTransform = GetComponent<RectTransform>();
        headerSize = rectTransform.sizeDelta.y;
    }

    public void SetHeaderText(string textToSet){
        headerText.text = textToSet;
        gameObject.name = $"{transform.GetSiblingIndex().ToString("000")}-Category-{textToSet}-header";
    }
}

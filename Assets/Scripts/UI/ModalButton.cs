using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ModalButton : MonoBehaviour
{
    [SerializeField] private Image fillImage, lineImage;
    [SerializeField] private TextMeshProUGUI modalText;
    [SerializeField] private Button mButton;

    [SerializeField] private Color defaultFillColor, defaultLineColor;

    public Button Button => mButton;

    public string ModalText {
        get { return modalText.text; }
        set { 
            modalText.text = value;
        }
    }

    public Color FillColor {
        get { return fillImage.color; }
        set {
            fillImage.color = value;
        }
    }

    public Color LineColor {
        get { return lineImage.color; }
        set {
            lineImage.color = value;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(!mButton)
            mButton = GetComponent<Button>();
    }

    public void RemoveListeners(){
        mButton.onClick.RemoveAllListeners();
    }

    public void SetupModalButton(ModalResult modalResult){
        // Debug.Log("Button with result " + modalResult + " set up");
        SetupModalButton(modalResult.ToString(), defaultFillColor, defaultLineColor);
    }

    public void SetupModalButton(ModalResult modalResult, Color fillColor, Color lineColor){
        SetupModalButton(modalResult.ToString(), fillColor, lineColor);
    }

    public void SetupModalButton(string modalText){
        SetupModalButton(modalText, defaultFillColor, defaultLineColor);
    }

    public void SetupModalButton(string modalText, Color fillColor, Color lineColor){
        gameObject.name = modalText + "_button";
        ModalText = modalText;
        FillColor = fillColor;
        LineColor = lineColor;
    }
    
}

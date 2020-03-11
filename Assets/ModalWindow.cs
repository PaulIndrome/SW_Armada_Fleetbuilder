using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalWindow : MonoBehaviour
{
    public event Action<ModalResult> OnResult;
    public event Action<string> OnGenericResult;

    [Header("Asset references")]
    [SerializeField] private ModalButton modalButtonPrefab;

    [Header("Scene references")]
    [SerializeField] private RectTransform buttonGridTransform;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Canvas modalWindowCanvas;


    private Dictionary<ModalResult, ModalButton> modalButtons;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        modalButtons = new Dictionary<ModalResult, ModalButton>();
        ModalWindowHandler.RegisterModalWindow(this);
    }

    public void ShowModalWindow(Action<ModalResult> resultHandlingMethod, string title, string description = "", params ModalResult[] possibleResults){
        // instantiate buttons for all possible results (add to dictionary if not existing for reuse)
        // set buttons relating to possible results 
        if(possibleResults.Length < 1) return;

        if(modalButtons == null)
            modalButtons = new Dictionary<ModalResult, ModalButton>();

        OnResult += resultHandlingMethod;

        titleText.text = title;
        descriptionText.text = description;

        SetupModalButtons(possibleResults);

        modalWindowCanvas.enabled = true;
    }

    public void ShowModalWindowWithSlider(Action<ModalResult> resultHandlingMethod, string title, string description = "", params ModalResult[] possibleResults){
        // instantiate buttons for all possible results (add to dictionary if not existing for reuse)
        // set buttons relating to possible results 
        if(possibleResults.Length < 1) return;

        if(modalButtons == null)
            modalButtons = new Dictionary<ModalResult, ModalButton>();

        OnResult += resultHandlingMethod;

        titleText.text = title;
        descriptionText.text = description;

        SetupModalButtons(possibleResults);

        

        modalWindowCanvas.enabled = true;
    }

    void SetupModalButtons(params ModalResult[] possibleResults){
        ModalButton mButton;

        for(int i = 0; i < possibleResults.Length; i++){
            if(!modalButtons.TryGetValue(possibleResults[i], out mButton)){
                mButton = Instantiate<ModalButton>(modalButtonPrefab, Vector3.zero, Quaternion.identity, buttonGridTransform);
                // Debug.Log("Button with result " + possibleResults[i] + " created");
                modalButtons.Add(possibleResults[i], mButton);
            }
            mButton.SetupModalButton(possibleResults[i]);
            mButton.Button.onClick.RemoveAllListeners();
            // Debug.Log($"Adding listener for result {i} ({possibleResults[i]} to button {mButton.name}", mButton);

            // passing in array values yields OutOfRangeException because the array does not exist past this method's scope
            // we pass the result as a static enum
            ModalResult result = possibleResults[i]; 
            mButton.Button.onClick.AddListener(() => ButtonResult(result));
            mButton.gameObject.SetActive(true);
        }
    }

    ///<summary>Modal window variant for generic buttons / results using strings</summary>
    public void ShowModalWindow(Action<ModalResult> resultHandlingMethod, string title, string description = "", ModalResult[] modalResults = null, params string[] possibleResults){
        
    }

    void DeactivateModalWindow(){
        foreach(ModalButton mb in modalButtons.Values){
            mb.gameObject.SetActive(false);
        }
        modalWindowCanvas.enabled = false;
    }

    public void ButtonResult(ModalResult buttonResult){
        // called by buttons with specific result
        // Debug.Log("Button with result " + buttonResult + " clicked");
        if(OnResult != null){
            OnResult(buttonResult);
        }
        OnResult = null; // delegate{};
        DeactivateModalWindow();
    }

    public void ButtonResult(string buttonResult){
        // called by buttons with generic result
    }

    public void ResultHandlingMethodTest(ModalResult result){
        Debug.Log("Handling result " + result);
    }

    [ContextMenu("Test modal window")]
    void TestModalWindow(){
        ShowModalWindow(ResultHandlingMethodTest, "T'is a test", "T'is a description", ModalResult.Yes, ModalResult.No, ModalResult.Cancel);
    }

    // IEnumerator ShowModal

}



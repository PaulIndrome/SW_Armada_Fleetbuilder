using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TMP_InputField))]
public class TMPIntegerInputClamp : MonoBehaviour
{

    private int parsedInt;
    private float parsedFloat;
    private string onSelectString;
    private TMP_InputField inputField;

    public Vector2 clampRange;
    public UnityEventString OnSubmit;
    public UnityEvent OnInvalid;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        if(inputField.contentType != TMP_InputField.ContentType.IntegerNumber && inputField.contentType != TMP_InputField.ContentType.DecimalNumber){
            this.enabled = false;
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if(inputField.contentType == TMP_InputField.ContentType.IntegerNumber){
            inputField.onValueChanged.AddListener(ClampInteger);
            inputField.onEndEdit.AddListener(OnEndEditConditionalSubmitInteger);
        } else if(inputField.contentType == TMP_InputField.ContentType.DecimalNumber){
            inputField.onValueChanged.AddListener(ClampDecimal);
            inputField.onEndEdit.AddListener(OnEndEditConditionalSubmitDecimal);
        } else {
            this.enabled = false;
            return;
        }
        // inputField.onSubmit.AddListener(Submit);
    }

    public void SetOnSelectString(string input){
        onSelectString = input;
    }

    private void ClampInteger(string input){
        if(input.Length < 1 || !int.TryParse(input, out parsedInt)) return;
        inputField.SetTextWithoutNotify(Mathf.Clamp(parsedInt, clampRange.x, clampRange.y).ToString());
    }
    private void ClampDecimal(string input){
        if(input.Length < 1 || !float.TryParse(input, out parsedFloat)) return;
        inputField.SetTextWithoutNotify(Mathf.Clamp(parsedFloat, clampRange.x, clampRange.y).ToString());
    }

    private void Submit(string input){
        if(OnSubmit != null)
            OnSubmit.Invoke(input);
    }

    private void OnEndEditConditionalSubmitInteger(string input){
        if(input.Length > 0 && int.TryParse(input, out parsedInt) && parsedInt >= CurrentDeck.deck.PointsCurrent){
            // Debug.Log(parsedInt + " >= " + CurrentDeck.deck.PointsCurrent);
            Submit(input);
        } else {
            // Debug.Log(parsedInt);
            if(OnInvalid != null){
                OnInvalid.Invoke();
            }
            inputField.SetTextWithoutNotify(onSelectString);
        }
    }

    private void OnEndEditConditionalSubmitDecimal(string input){
        if(input.Length > 0 && float.TryParse(input, out parsedFloat) && parsedFloat >= CurrentDeck.deck.PointsCurrent){
            Submit(input);
        } else {
            if(OnInvalid != null){
                OnInvalid.Invoke();
            }
            inputField.SetTextWithoutNotify(onSelectString);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if(inputField.contentType == TMP_InputField.ContentType.IntegerNumber){
            inputField.onValueChanged.RemoveListener(ClampInteger);
            inputField.onEndEdit.RemoveListener(OnEndEditConditionalSubmitInteger);
        } else if(inputField.contentType == TMP_InputField.ContentType.DecimalNumber){
            inputField.onValueChanged.RemoveListener(ClampDecimal);
            inputField.onEndEdit.RemoveListener(OnEndEditConditionalSubmitDecimal);
        } 
        // inputField.onSubmit.RemoveListener(Submit);
    }
}

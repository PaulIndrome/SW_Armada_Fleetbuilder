using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class DeckMessage : MonoBehaviour
{

    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI deckMessageText;
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetText(string text){
        deckMessageText.text = text;
    }

    public void ToggleMessage(bool onOff){
        canvasGroup.alpha = onOff ? 1 : 0;
    }

}

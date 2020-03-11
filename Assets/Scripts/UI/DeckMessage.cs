using UnityEngine;
using TMPro;

public class DeckMessage : MonoBehaviour
{

    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI deckMessageText;

    public void SetText(string text){
        deckMessageText.text = text;
    }

}

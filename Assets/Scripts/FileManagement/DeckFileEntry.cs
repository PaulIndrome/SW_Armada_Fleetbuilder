using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckFileEntry : MonoBehaviour
{

    public delegate void DeckFileSelectedDelegate(DeckFileEntry selected, SerializableDeck sDeck);
    public static event DeckFileSelectedDelegate OnDeckFileSelected;


    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI deckNameText;
    [SerializeField] private Image factionIcon;

    [Header("Asset references")]
    [SerializeField] private Sprite defaultIcon;
    [SerializeField] private Sprite rebellionIcon;
    [SerializeField] private Sprite empireIcon;
    [SerializeField] private Sprite collaborationIcon;

    [ReadOnly, SerializeField] private SerializableDeck serializableDeck;
    public SerializableDeck SerializableDeck => serializableDeck;

    [ReadOnly, SerializeField] private string fileName;
    public string FileName => fileName;

    public string DeckNameText => deckNameText.text;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(!deckNameText) deckNameText = GetComponentInChildren<TextMeshProUGUI>();
        if(!factionIcon) factionIcon = GetComponentInChildren<Image>();
    }

    public void SetupDeckFileEntry(SerializableDeck sDeck, string fileName){
        this.fileName = fileName;

        serializableDeck = sDeck;

        deckNameText.text = sDeck.deckName;

        switch(sDeck.deckFaction){
            case Faction.Empire:
                factionIcon.sprite = empireIcon;
                break;
            case Faction.Rebellion:
                factionIcon.sprite = rebellionIcon;
                break;
            case (Faction) ~0:
                factionIcon.sprite = collaborationIcon;
                break;
            default:
                factionIcon.sprite = defaultIcon;
                break;
        }
    }

    public void SelectDeckFile(){
        if(OnDeckFileSelected != null)
            OnDeckFileSelected(this, serializableDeck);
    }


}

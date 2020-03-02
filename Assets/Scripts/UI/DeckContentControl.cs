using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckContentControl : MonoBehaviour
{

    public delegate int AddedToDeckDelegate(CardUI cardUI, int amountToAdd = 1);
    public static event AddedToDeckDelegate OnAddedToDeck;

    public delegate int RemovedFromDeckDelegate(CardUI cardUI, int amountToRemove = 1);
    public static event RemovedFromDeckDelegate OnRemovedFromDeck;


    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI deckMaxPointsText;
    [SerializeField] private TextMeshProUGUI deckCurrentPointsText;

    [Header("Set via script")]
    [SerializeField] private Deck currentDeck;

    public int DeckPointsMax {
        get { return currentDeck.PointsMax; }
        private set {
            if(value > 0 && value < 9999){
                currentDeck.SetPointsMax(value);
                deckMaxPointsText.text = currentDeck.PointsMax.ToString();
            }
        }
    }

    public int DeckPointsCurrent {
        get { return currentDeck.CurrentPoints; }
        private set {
            Debug.Log("Setting deckCurrentPointsText to " + value, deckCurrentPointsText);
            if(value < DeckPointsMax && value > -1){
                deckCurrentPointsText.text = currentDeck.CurrentPoints.ToString();
            }
        }
    }


    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        CardSelectionSlot.OnTryAddToDeck += AddCardToCurrentDeck;
        CardSelectionSlot.OnReturnToCollection += RemoveCardFromCurrentDeck;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        NewDeck("Empty Deck", 400);
    }

    private int AddCardToCurrentDeck(CardUI cardUI, int amountToAdd = 1){
        DeckEntry newEntry; 
        int result = currentDeck.AddCardToDeck(cardUI.Card, out newEntry);
        if(result > 0){
            DeckPointsCurrent = currentDeck.CurrentPoints;
            OnAddedToDeck(cardUI, amountToAdd);
        }
        return result;
    }

    private int RemoveCardFromCurrentDeck(CardUI cardUI, int amountToAdd = 1){
        if(currentDeck.AllCardsInDeck.Find(dE => dE.Identifier == cardUI.Card.ID) == null) {
            Debug.LogError($"DeckEntry for {cardUI.Card.ID} not found");
            return 0;
        }

        Debug.Log("Removing card from deck");

        int result;

        if(cardUI.Card is CardUnityShip){
            DeckEntryShip entryShip = currentDeck.shipCards.FindLast(eShip => eShip.Identifier == cardUI.Card.ID);
            result = currentDeck.RemoveEntry(entryShip);
        } else if (cardUI.Card is CardUnitySquadron){
            DeckEntrySquadron entrySquadron = currentDeck.squadronCards.FindLast(eSquadron => eSquadron.Identifier == cardUI.Card.ID);
            result = currentDeck.RemoveEntry(entrySquadron);
        } else {
            DeckEntryUpgrade entryUpgrade = currentDeck.upgradeCards.FindLast(eUpgrade => eUpgrade.Identifier == cardUI.Card.ID);
            result = currentDeck.RemoveEntry(entryUpgrade);
        }

        if(result > 0){
            DeckPointsCurrent = currentDeck.CurrentPoints;
            Debug.Log("Removed card from deck");
            OnRemovedFromDeck(cardUI, amountToAdd);
            return 1;
        } else {
            return 0;
        }
        
    }

    public void NewDeck(string deckName, int maxPoints = 400){
        currentDeck = new Deck(deckName, maxPoints);
        DeckPointsMax = maxPoints;
    }

    public void SaveCurrentDeck(){
        DeserializeCardsFromJSON.SerializeDeck(currentDeck);
    }

    public void LoadDeckFromFile(string deckName){
        currentDeck = DeserializeCardsFromJSON.DeserializeDeck(deckName);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardSelectionSlot.OnTryAddToDeck -= AddCardToCurrentDeck;
        CardSelectionSlot.OnReturnToCollection -= RemoveCardFromCurrentDeck;
    }

}

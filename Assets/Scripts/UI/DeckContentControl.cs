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

    public delegate void StartNewDeckDelegate(Deck newDeck);
    public static event StartNewDeckDelegate OnNewDeckStarted;

    public delegate void DeckLoadedDelegate(Deck loadedDeck);
    public static event DeckLoadedDelegate OnDeckLoaded;


    [Header("Scene references")]
    [SerializeField] private TextMeshProUGUI deckMaxPointsText;
    [SerializeField] private TextMeshProUGUI deckCurrentPointsText;
    [SerializeField] private DeckMessageControl deckMessageControl;

    [Header("Set via script")]
    [SerializeField] private Deck currentDeck;

    public Deck CurrentDeck => currentDeck;

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
        get { return currentDeck.PointsCurrent; }
        private set {
            if(value <= DeckPointsMax && value > -1){
                deckCurrentPointsText.text = currentDeck.PointsCurrent.ToString();
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
        CardUI.OnCardSelected += ShowDeckMessages;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        NewDeck("", 400);
    }


    private int AddCardToCurrentDeck(CardUI cardUI, int amountToAdd = 1){
        DeckEntry newEntry = currentDeck.AddCardToDeck(cardUI.Card);
        if(newEntry != null){
            DeckPointsCurrent = currentDeck.PointsCurrent;
            OnAddedToDeck(cardUI, amountToAdd);
        }
        
        ShowDeckMessages(cardUI);

        return newEntry != null ? 1 : 0;
    }

    private int RemoveCardFromCurrentDeck(CardUI cardUI, int amountToAdd = 1){
        if(currentDeck.AllCardsInDeck.Find(dE => dE.Identifier == cardUI.Card.ID) == null) {
            Debug.LogError($"DeckEntry for {cardUI.Card.ID} not found");
            return 0;
        }

        int result = 0;

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
            DeckPointsCurrent = currentDeck.PointsCurrent;
            OnRemovedFromDeck(cardUI, amountToAdd);
        }

        ShowDeckMessages(cardUI);

        return result;
    }

    public void NewDeck(string deckName, int maxPoints = 400, Faction faction = (Faction) ~0){
        currentDeck = new Deck(deckName, maxPoints, faction);
        DeckPointsMax = maxPoints;
        DeckPointsCurrent = 0;
        if(OnNewDeckStarted != null)
            OnNewDeckStarted(currentDeck);
    }

    public void SaveCurrentDeck(){
        DeserializeCardsFromJSON.SerializeDeck(currentDeck);
    }

    public void LoadDeckFromFile(SerializableDeck sDeck){
        currentDeck = new Deck(sDeck);
        DeckPointsCurrent = currentDeck.PointsCurrent;
        DeckPointsMax = currentDeck.PointsMax;
        if(OnDeckLoaded != null)
            OnDeckLoaded(currentDeck);
    }

    void ShowDeckMessages(CardUI cardUI){
        if(cardUI == null){
            deckMessageControl.ToggleAllMessages(false);
        }
        deckMessageControl.ToggleMaxPointsExceeded(DeckPointsCurrent + cardUI.Card.cost > DeckPointsMax);
        deckMessageControl.ToggleThirdSquadronPointsExceeded(cardUI.Card is CardUnitySquadron && (CurrentDeck.SquadronPoints + cardUI.Card.cost) > CurrentDeck.MaxSquadronPoints, CurrentDeck.SquadronPoints, CurrentDeck.MaxSquadronPoints);
        deckMessageControl.ToggleCollectionAmountZero(cardUI.CurrentAmountInCollection < 1, cardUI.Card.isUnique);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardSelectionSlot.OnTryAddToDeck -= AddCardToCurrentDeck;
        CardSelectionSlot.OnReturnToCollection -= RemoveCardFromCurrentDeck;
        CardUI.OnCardSelected -= ShowDeckMessages;
    }

}

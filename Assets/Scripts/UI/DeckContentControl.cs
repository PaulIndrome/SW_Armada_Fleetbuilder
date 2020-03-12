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
    [SerializeField] private TMP_InputField deckMaxPointsText;
    [SerializeField] private TextMeshProUGUI deckCurrentPointsText;
    [SerializeField] private DeckMessageControl deckMessageControl;

    [Header("Set via script")]
    [SerializeField] private Deck currentDeckContent;
    [ReadOnly, SerializeField] private CardUI currentSelectedCard;

    public Deck CurrentDeckContent => currentDeckContent;

    public int DeckPointsMax {
        get { return currentDeckContent.PointsMax; }
        private set {
            if(value > -1 && value <= 9999){
                // Debug.Log("DeckContentControl max points to " + value);
                currentDeckContent.SetPointsMax(value);
                deckMaxPointsText.SetTextWithoutNotify(currentDeckContent.PointsMax.ToString());
                if(currentSelectedCard != null){
                    currentSelectedCard.UpdateDeckMessagesOnCardByStaticDeck();
                    ShowDeckMessages(currentSelectedCard);
                }
            }
        }
    }

    public int DeckPointsCurrent {
        get { return currentDeckContent.PointsCurrent; }
        private set {
            if(value <= DeckPointsMax && value > -1){
                deckCurrentPointsText.text = currentDeckContent.PointsCurrent.ToString();
            }
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        deckMaxPointsText.contentType = TMP_InputField.ContentType.IntegerNumber;
        deckMaxPointsText.pointSize = 100;
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
        DeckEntry newEntry = currentDeckContent.AddCardToDeck(cardUI.Card);
        if(newEntry != null){
            DeckPointsCurrent = currentDeckContent.PointsCurrent;
            OnAddedToDeck(cardUI, amountToAdd);
        }
        
        ShowDeckMessages(cardUI);

        return newEntry != null ? 1 : 0;
    }

    private int RemoveCardFromCurrentDeck(CardUI cardUI, int amountToAdd = 1){
        if(currentDeckContent.AllCardsInDeck.Find(dE => dE.Identifier == cardUI.Card.ID) == null) {
            Debug.LogError($"DeckEntry for {cardUI.Card.ID} not found");
            return 0;
        }

        int result = 0;

        if(cardUI.Card is CardUnityShip){
            DeckEntryShip entryShip = currentDeckContent.shipCards.FindLast(eShip => eShip.Identifier == cardUI.Card.ID);
            result = currentDeckContent.RemoveEntry(entryShip);
        } else if (cardUI.Card is CardUnitySquadron){
            DeckEntrySquadron entrySquadron = currentDeckContent.squadronCards.FindLast(eSquadron => eSquadron.Identifier == cardUI.Card.ID);
            result = currentDeckContent.RemoveEntry(entrySquadron);
        } else {
            DeckEntryUpgrade entryUpgrade = currentDeckContent.upgradeCards.FindLast(eUpgrade => eUpgrade.Identifier == cardUI.Card.ID);
            result = currentDeckContent.RemoveEntry(entryUpgrade);
        }

        if(result > 0){
            DeckPointsCurrent = currentDeckContent.PointsCurrent;
            OnRemovedFromDeck(cardUI, amountToAdd);
        }

        ShowDeckMessages(cardUI);

        return result;
    }

    public void NewDeck(string deckName, int maxPoints = 400, Faction faction = (Faction) ~0){
        currentDeckContent = new Deck(deckName, maxPoints, faction);
        DeckPointsMax = maxPoints;
        DeckPointsCurrent = 0;
        ShowDeckMessages(null);
        if(OnNewDeckStarted != null)
            OnNewDeckStarted(currentDeckContent);
    }

    public void SaveCurrentDeck(){
        DeserializeCardsFromJSON.SerializeDeck(currentDeckContent);
    }

    public void LoadDeckFromFile(SerializableDeck sDeck){
        currentDeckContent = new Deck(sDeck);
        DeckPointsCurrent = currentDeckContent.PointsCurrent;
        DeckPointsMax = currentDeckContent.PointsMax;
        ShowDeckMessages(null);
        if(OnDeckLoaded != null)
            OnDeckLoaded(currentDeckContent);
    }

    void ShowDeckMessages(CardUI cardUI){
        if(cardUI == null){
            deckMessageControl.ToggleAllMessages(false);
            currentSelectedCard = null;
            return;
        }

        currentSelectedCard = cardUI;

        deckMessageControl.ToggleMaxPointsExceeded(DeckPointsCurrent + cardUI.Card.cost > DeckPointsMax);
        deckMessageControl.ToggleThirdSquadronPointsExceeded(cardUI.Card is CardUnitySquadron && (CurrentDeckContent.SquadronPoints + cardUI.Card.cost) > CurrentDeckContent.MaxSquadronPoints, CurrentDeckContent.SquadronPoints, CurrentDeckContent.MaxSquadronPoints);
        deckMessageControl.ToggleCollectionAmountZero(cardUI.CurrentAmountInCollection < 1, cardUI.Card.isUnique);
    }

    public void SetPointsMax(string newPointsMax){
        int newMaxParsed;
        if(int.TryParse(newPointsMax, out newMaxParsed)){
            DeckPointsMax = newMaxParsed;
        }
    }

    [ContextMenu("Debug CurrentDeck Static")]
    public void DebugCurrentDeckStatic(){
        Debug.Log("Name: " + CurrentDeck.deck.DeckName);
        Debug.Log("Faction: " + CurrentDeck.deck.DeckFaction);
        Debug.Log("Points current: " + CurrentDeck.deck.PointsCurrent);
        Debug.Log("Points max: " + CurrentDeck.deck.PointsMax);
        Debug.Log("No. cards in deck: " + CurrentDeck.deck.AllCardsInDeck.Count);
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

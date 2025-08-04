using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeckContentControl : MonoBehaviour
{

    public delegate int CardMovedDelegate(CardUnityBase card);
    public static event CardMovedDelegate OnAddedToDeck, OnRemovedFromDeck;

    public delegate void DeckStateDelegate(Deck newDeck);
    public static event DeckStateDelegate OnNewDeckStarted, OnDeckLoaded;


    [Header("Scene references")]
    [SerializeField] private TMP_InputField deckMaxPointsText;
    [SerializeField] private TextMeshProUGUI deckCurrentPointsText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform shipContentTransform, squadronContentTransform, upgradeContentTransform;

    [Header("Set via script")]
    [SerializeField] private Deck currentDeckContent;
    [ReadOnly, SerializeField] private CardUI currentSelectedCard;
    private List<DeckEntry> entriesToRemove;

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
                    // ShowDeckMessages(currentSelectedCard);
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
        CardSelection.OnTryAddToDeck += AddCardToCurrentDeck;
        CardSelection.OnTryRemoveFromDeck += RemoveCardFromCurrentDeck;
        // CardUI.OnCardSelected += ShowDeckMessages;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        NewDeck("", 400);
    }


    private DeckEntry AddCardToCurrentDeck(CardUnityBase card){
        DeckEntry newEntry = null;
        newEntry = currentDeckContent.AddCardToDeck(card);
        if(newEntry != null){
            DeckPointsCurrent = currentDeckContent.PointsCurrent;
            OnAddedToDeck(card);
        }

        return newEntry;
    }

    private int RemoveCardFromCurrentDeck(CardUnityBase card, int amountToAdd = 1){
        if(currentDeckContent.AllCardsInDeck.Find(dE => dE.Identifier == card.ID) == null) {
            Debug.LogError($"DeckEntry for {card.ID} not found");
            return 0;
        }

        int result = 0;

        if(card is CardUnityShip){
            DeckEntryShip entryShip = currentDeckContent.shipCards.FindLast(eShip => eShip.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entryShip);
        } else if (card is CardUnitySquadron){
            DeckEntrySquadron entrySquadron = currentDeckContent.squadronCards.FindLast(eSquadron => eSquadron.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entrySquadron);
        } else {
            DeckEntryUpgrade entryUpgrade = currentDeckContent.upgradeCards.FindLast(eUpgrade => eUpgrade.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entryUpgrade);
        }

        if(result > 0){
            DeckPointsCurrent = currentDeckContent.PointsCurrent;
            OnRemovedFromDeck(card);
        }

        // ShowDeckMessages(currentSelectedCard);

        return result;
    }

    private int RemoveCardFromCurrentDeck(CardUnityBase card){
        if(!currentDeckContent.cardIDs.Contains(card.ID)) {
            Debug.LogError($"DeckEntry for {card.ID} not found");
            return 0;
        }

        int result = 0;

        if(card is CardUnityShip){
            DeckEntryShip entryShip = currentDeckContent.shipCards.FindLast(eShip => eShip.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entryShip);
        } else if (card is CardUnitySquadron){
            DeckEntrySquadron entrySquadron = currentDeckContent.squadronCards.FindLast(eSquadron => eSquadron.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entrySquadron);
        } else {
            DeckEntryUpgrade entryUpgrade = currentDeckContent.upgradeCards.FindLast(eUpgrade => eUpgrade.Identifier == card.ID);
            result = currentDeckContent.RemoveEntry(entryUpgrade);
        }

        if(result > 0){
            DeckPointsCurrent = currentDeckContent.PointsCurrent;
            OnRemovedFromDeck(card);
        }

        // ShowDeckMessages(currentSelectedCard);

        return result;
    }

    public void NewDeck(string deckName, int maxPoints = 400, Faction faction = (Faction) ~0){
        currentDeckContent = new Deck(deckName, maxPoints, faction);
        DeckPointsMax = maxPoints;
        DeckPointsCurrent = 0;
        // ShowDeckMessages(null);
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
        // ShowDeckMessages(null);
        if(OnDeckLoaded != null)
            OnDeckLoaded(currentDeckContent);
    }

    // void ShowDeckMessages(CardUI cardUI){
    //     if(cardUI == null){
    //         deckMessageControl.ToggleAllMessages(false);
    //         currentSelectedCard = null;
    //         return;
    //     }

    //     currentSelectedCard = cardUI;

    //     deckMessageControl.ToggleMaxPointsExceeded(DeckPointsCurrent + cardUI.Card.cost > DeckPointsMax);
    //     deckMessageControl.ToggleThirdSquadronPointsExceeded(cardUI.Card is CardUnitySquadron && (CurrentDeckContent.SquadronPoints + cardUI.Card.cost) > CurrentDeckContent.MaxSquadronPoints, CurrentDeckContent.SquadronPoints, CurrentDeckContent.MaxSquadronPoints);
    //     deckMessageControl.ToggleCollectionAmountZero(cardUI.CurrentAmountInCollection < 1, cardUI.Card.isUnique);
    // }

    public void SetupDeckCards(){
        for(int i = 0; i < CurrentDeckContent.shipCards.Count; i++){
            
        }
        for(int i = 0; i < CurrentDeckContent.squadronCards.Count; i++){
            
        }
        for(int i = 0; i < CurrentDeckContent.upgradeCards.Count; i++){
            
        }
    }

    public void SetPointsMax(string newPointsMax){
        int newMaxParsed;
        if(int.TryParse(newPointsMax, out newMaxParsed)){
            DeckPointsMax = newMaxParsed;
        }
    }

    public Faction InvertFaction(Faction toInvert){
        return (Faction.Empire | Faction.Rebellion) & ~toInvert;
    }

    [ContextMenu("Switch faction to Empire")]
    public void SwitchFactionToEmpire(){
        SwitchFaction(Faction.Empire);
    }

    [ContextMenu("Switch faction to Rebellion")]
    public void SwitchFactionToRebellion(){
        SwitchFaction(Faction.Rebellion);
    }
    
    [ContextMenu("Switch faction to Everything")]
    public void SwitchFactionToEverything(){
        SwitchFaction((Faction) ~0);
    }

    public void SwitchFactionInvert(){
        SwitchFaction(InvertFaction(CurrentDeckContent.DeckFaction));
    }

    public void SwitchFaction(Faction to){
        if(to == CurrentDeckContent.DeckFaction) return;
        if(to == (Faction) ~0){
            CurrentDeckContent.SetFaction((Faction) ~0);
            OnDeckLoaded(CurrentDeckContent);
            return;
        }

        entriesToRemove = CurrentDeckContent.FindAllByFaction(InvertFaction(to));
        if(entriesToRemove.Count > 0){
            // ModalWindowHandler.ShowModalWindow( ModalSwitchFaction, 
            //                                     "Switch faction to " + to, 
            //                                     $"Switching faction will remove {entriesToRemove.Count} cards from current deck.\nAre you sure you want to change the deck's faction?",
            //                                     ModalResult.Yes, ModalResult.No);
            ModalWindowHandler.ShowModalWindowAction( () => ModalSwitchFaction(to), 
                                                "Switch faction to " + to,
                                                $"Switching faction will remove {entriesToRemove.Count} cards from current deck.\nAre you sure you want to change the deck's faction?",
                                                ModalResult.Yes, ModalResult.No);
        } else {
            ModalSwitchFaction(to);
        }
    }

    public void ModalSwitchFaction(Faction newFaction){
        // Debug.Log("Switching faction to " + newFaction);
        for(int i = 0; i < entriesToRemove.Count; i++){
            // Debug.Log("Removing card from deck: " + entriesToRemove[i].Card.ID, entriesToRemove[i].Card);
            RemoveCardFromCurrentDeck(entriesToRemove[i].Card);
        }
        entriesToRemove.Clear();
        CurrentDeckContent.SetFaction(newFaction);
        OnDeckLoaded(CurrentDeckContent);
    }

    [ContextMenu("Debug CurrentDeck Static")]
    public void DebugCurrentDeckStatic(){
        Debug.Log("Name: " + CurrentDeck.Deck.DeckName);
        Debug.Log("Faction: " + CurrentDeck.Deck.DeckFaction);
        Debug.Log("Points current: " + CurrentDeck.Deck.PointsCurrent);
        Debug.Log("Points max: " + CurrentDeck.Deck.PointsMax);
        Debug.Log("No. cards in deck: " + CurrentDeck.Deck.AllCardsInDeck.Count);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardSelection.OnTryAddToDeck -= AddCardToCurrentDeck;
        CardSelection.OnTryRemoveFromDeck -= RemoveCardFromCurrentDeck;
        // CardUI.OnCardSelected -= ShowDeckMessages;
    }

}

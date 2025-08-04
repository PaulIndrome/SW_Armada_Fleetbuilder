using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.UI;

public class CardSelection : MonoBehaviour {

    public delegate DeckEntry AddToDeckFromCollectionDelegate(CardUnityBase card);
    public static event AddToDeckFromCollectionDelegate OnTryAddToDeck;

    public delegate int RemoveFromDeckToCollectionDelegate(CardUnityBase card);
    public static event RemoveFromDeckToCollectionDelegate OnTryRemoveFromDeck;

    public delegate void UpdateDeckMessagesDelegate(CardUI cardUI);
    public static event UpdateDeckMessagesDelegate OnUpdateDeckMessages;

    public delegate void DeckEntrySelectionDelegate(DeckEntry entry);
    public static event DeckEntrySelectionDelegate OnDeckEntrySelectionChange, OnUpgradeSlotted;

    [Header("Scene references")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cardBackgroundImage;
    [SerializeField] private Image cardAnimationImage;

    [SerializeField] private FlickGesture flickGesture;
    [SerializeField] private Animator cardSelectionAnimator;

    [SerializeField] private Button previousDeckEntryButton, nextDeckEntryButton;

    [Header("Set via script")]
    [ReadOnly, SerializeField] private bool isCardSelected = false;
    private bool horizontalFlickEnabled = false;
    private int moveToDeckHash, moveToCollectionHash;
    [ReadOnly(true), SerializeField, ContextMenuItem("Add to deck", "AddToDeck"), ContextMenuItem("Remove from deck", "RemoveFromDeck")] private CardUI currentSelectedCard;
    [SerializeField] private DeckEntryShip currentDeckEntryShip;
    public DeckEntryShip CurrentDeckEntryShip {
        get { return currentDeckEntryShip; }
        set {
            currentDeckEntryShip = value;
            OnDeckEntrySelectionChange(currentDeckEntryShip);
        }
    }
    [SerializeField] private DeckEntryUpgrade currentDeckEntryUpgrade;
    public DeckEntryUpgrade CurrentDeckEntryUpgrade {
        get { return currentDeckEntryUpgrade; }
        set {
            currentDeckEntryUpgrade = value;
            OnDeckEntrySelectionChange(currentDeckEntryUpgrade);
        }
    }

    [ReadOnly, SerializeField] private int deckEntryShipIndex = 0, deckEntryUpgradeIndex = 0;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        moveToDeckHash = Animator.StringToHash("MoveToDeck");
        moveToCollectionHash = Animator.StringToHash("MoveToCollection");

        Image[] images = GetComponentsInChildren<Image>();
        if(cardImage == null){
            cardImage = images[2];
        }
        
        cardImage.preserveAspect = true;    
        
        if(cardBackgroundImage == null){
            cardBackgroundImage = images[1];
        }

        cardBackgroundImage.preserveAspect = true;
        cardBackgroundImage.color = Color.clear;

        ToggleDeckEntryScrubbing(false);
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        CardUI.OnCardSelected += SetCurrentSelectedCard;
        Deck.OnNewDeckConstructor += ClearSelectedCard;
        flickGesture.Flicked += FlickedHandler;
    }

    void FlickedHandler(object sender, EventArgs e){
        // Debug.Log(flickGesture.PreviousNormalizedScreenPosition.y + " -> " + flickGesture.NormalizedScreenPosition.y);
        // Debug.Log(flickGesture.ScreenFlickVector);
        if(currentSelectedCard){
            if(currentSelectedCard.CurrentAmountInDeck > 1 && Mathf.Abs(flickGesture.ScreenFlickVector.x) > Mathf.Abs(flickGesture.ScreenFlickVector.y)){
                if(Mathf.Sign(flickGesture.ScreenFlickVector.x) > 0){ // flicked right
                    Debug.Log("flicked right");
                } else { // flicked left
                    Debug.Log("flicked left");
                }
            } else {
                if(Mathf.Sign(flickGesture.ScreenFlickVector.y) > 0){ // flicked up
                    cardSelectionAnimator.SetTrigger(moveToDeckHash);
                    AddToDeck(currentSelectedCard);
                } else { // flicked down
                    cardSelectionAnimator.SetTrigger(moveToCollectionHash);
                    RemoveFromDeck(currentSelectedCard);
                }
            }
        }
    }

    private void ClearSelectedCard(){
        SetCurrentSelectedCard(null);
    }

    public void DeselectCard(){
        currentSelectedCard.DeselectCard();
    }

    public void SetCurrentSelectedCard(CardUI cardToSet){
        if(UpgradeSlotButtonsControl.UPGRADE_SET_MODE){
            Debug.Log("Tryin to set upgrade to " + currentDeckEntryShip.Identifier);
            DeckEntryUpgrade deckEntryUpgrade = CurrentDeck.Deck.FindFirstUpgradeOfIDAndUnslotted(cardToSet.Card.ID) ?? AddToDeck(cardToSet) as DeckEntryUpgrade;
            if(deckEntryUpgrade == null){
                Debug.Log("DeckEntryUpgrade remained null");
                return;
            } else if(!currentDeckEntryShip.SlotUpgrade(deckEntryUpgrade)){
                Debug.Log("DeckEntryUpgrade could not be slotted");
                RemoveFromDeck(cardToSet);
                // CurrentDeck.Deck.RemoveEntry(deckEntryUpgrade);
                return;
            } else {
                Debug.Log("DeckEntryUpgrade was valid");
                if(OnUpgradeSlotted != null){
                    OnUpgradeSlotted(deckEntryUpgrade);
                }
            }
        } else {
            if(currentSelectedCard != null){
                DeselectCard();
            }

            currentSelectedCard = cardToSet;
            
            if(cardToSet == null){
                isCardSelected = false;
                cardImage.sprite = cardAnimationImage.sprite = null;
                cardImage.color = Color.clear;
            } else {
                isCardSelected = true;
                cardImage.sprite = cardAnimationImage.sprite = cardToSet.Card.sprite;
                cardImage.color = Color.white;
                SetCurrentDeckEntry(cardToSet);
            }

            OnUpdateDeckMessages(currentSelectedCard);
        }
        
    }

    [ContextMenu("Test DeckEntry")]
    public void TestDeckEntry(){
        if(CurrentDeckEntryShip.SlotUpgrade(CurrentDeckEntryUpgrade)){
            Debug.Log($"Slotted upgrade {CurrentDeckEntryUpgrade.Card.ID}", CurrentDeckEntryUpgrade.Card);
        } else {
            Debug.Log($"Couldn't slot upgrade {CurrentDeckEntryUpgrade.Card.ID}", CurrentDeckEntryUpgrade.Card);
        }

        Debug.Log($"to ship {CurrentDeckEntryShip.Card.ID}", CurrentDeckEntryShip.Card);
    }

    public DeckEntry AddToDeck(CardUI cardUIToAddToDeck){
        DeckEntry result = null;
        if(OnTryAddToDeck != null){
            result = OnTryAddToDeck(cardUIToAddToDeck.Card);
            if(result != null){
                OnUpdateDeckMessages(cardUIToAddToDeck);
                ToggleDeckEntryScrubbing(cardUIToAddToDeck.CurrentAmountInDeck > 1);
                if(!UpgradeSlotButtonsControl.UPGRADE_SET_MODE) SetCurrentDeckEntry(cardUIToAddToDeck);
            }
        }
        return result;
    }

    public int RemoveFromDeck(CardUI cardUIToRemoveFromDeck){
        int result = -1;
        if(cardUIToRemoveFromDeck.CurrentAmountInDeck < 1) return result;
        if(OnTryRemoveFromDeck != null){
            result = OnTryRemoveFromDeck(cardUIToRemoveFromDeck.Card);
            if(result > 0){
                OnUpdateDeckMessages(cardUIToRemoveFromDeck);
                ToggleDeckEntryScrubbing(cardUIToRemoveFromDeck.CurrentAmountInDeck > 1);
                if(!UpgradeSlotButtonsControl.UPGRADE_SET_MODE) SetCurrentDeckEntry(cardUIToRemoveFromDeck);
            }
        }
        return result;
    }

    void SetCurrentDeckEntry(CardUI cardUI){
        if(cardUI.Card is CardUnitySquadron){
            CurrentDeckEntryShip = null;
            CurrentDeckEntryUpgrade = null;
            ToggleDeckEntryScrubbing(false);
            return;
        }

        if(currentSelectedCard.Card is CardUnityShip){
            CurrentDeckEntryShip = currentSelectedCard.CurrentAmountInDeck > 0 ? CurrentDeck.Deck.FindAllShipsOfID(currentSelectedCard.Card.ID)[deckEntryShipIndex] : null;
            // OnDeckEntrySelectionChange(currentDeckEntryShip);
        } else if(currentSelectedCard.Card is CardUnityUpgrade) {
            CurrentDeckEntryUpgrade = currentSelectedCard.CurrentAmountInDeck > 0 ? CurrentDeck.Deck.FindAllUpgradesOfID(currentSelectedCard.Card.ID)[deckEntryUpgradeIndex] : null;
            // OnDeckEntrySelectionChange(currentDeckEntryUpgrade);
        }

        ToggleDeckEntryScrubbing(currentSelectedCard.CurrentAmountInDeck > 1);
    }

    void ToggleDeckEntryScrubbing(bool onOff){
        horizontalFlickEnabled = onOff;
        previousDeckEntryButton.gameObject.SetActive(onOff);
        nextDeckEntryButton.gameObject.SetActive(onOff);
        if(onOff){

        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        Deck.OnNewDeckConstructor -= ClearSelectedCard;
        CardUI.OnCardSelected -= SetCurrentSelectedCard;
        flickGesture.Flicked -= FlickedHandler;
    }

}
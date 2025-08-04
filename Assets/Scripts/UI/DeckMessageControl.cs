using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMessageControl : MonoBehaviour
{
    [Header("Asset references")]
    [SerializeField] private DeckMessage deckMessagePrefab;

    [Header("Scene references")]
    [SerializeField] private DeckMessage maxPointsExceeded;
    [SerializeField] private DeckMessage thirdSquadronPointsExceeded;
    [SerializeField] private DeckMessage collectionAmountZero;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        CardSelection.OnUpdateDeckMessages += ShowDeckMessages;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        ToggleAllMessages(false);
    }

    void ShowDeckMessages(CardUI cardUI){
        if(UpgradeSlotButtonsControl.UPGRADE_SET_MODE) return;

        if(cardUI == null){
            ToggleAllMessages(false);
            return;
        }
        
        ToggleMaxPointsExceeded(CurrentDeck.Deck.PointsCurrent + cardUI.Card.cost > CurrentDeck.Deck.PointsMax);
        ToggleThirdSquadronPointsExceeded(cardUI.Card is CardUnitySquadron && (CurrentDeck.Deck.SquadronPoints + cardUI.Card.cost) > CurrentDeck.Deck.MaxSquadronPoints, CurrentDeck.Deck.SquadronPoints, CurrentDeck.Deck.MaxSquadronPoints);
        ToggleCollectionAmountZero(cardUI.CurrentAmountInCollection < 1, cardUI.Card.isUnique);
    }

    public void ToggleMaxPointsExceeded(bool onOff){
        maxPointsExceeded.ToggleMessage(onOff);
    }
    public void ToggleThirdSquadronPointsExceeded(bool onOff, int currentSquadronPoints = 0, int maxSquadronPoints = 0){
        thirdSquadronPointsExceeded.ToggleMessage(onOff);
        thirdSquadronPointsExceeded.SetText($"squadron exceeds one third of max points ({currentSquadronPoints} / {maxSquadronPoints})");
    }
    public void ToggleCollectionAmountZero(bool onOff, bool isUnique = false){
        collectionAmountZero.ToggleMessage(onOff);
        collectionAmountZero.SetText(onOff && isUnique ? $"this item is unique per deck" : "no item of this card remaining" );
    }

    public void ToggleAllMessages(bool onOff){
        ToggleMaxPointsExceeded(onOff);
        ToggleThirdSquadronPointsExceeded(onOff);
        ToggleCollectionAmountZero(onOff);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardSelection.OnUpdateDeckMessages -= ShowDeckMessages;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionSlot : SwipableComponent
{
    public delegate int AddToDeckFromCollectionDelegate(CardUI cardUI, int amountToAdd = 1);
    public static event AddToDeckFromCollectionDelegate OnTryAddToDeck;

    public delegate int ReturnToCollectionFromDeckDelegate(CardUI cardUI, int amountToAdd = 1);
    public static event ReturnToCollectionFromDeckDelegate OnReturnToCollection;
    
    

    [ReadOnly(true), SerializeField, ContextMenuItem("Add to deck", "AddToDeck"), ContextMenuItem("Remove from deck", "RemoveFromDeck")] private CardUI currentSelectedCard;

    [Header("Scene references")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image cardBackgroundImage;
    [SerializeField] private DeckContentControl deckContentControl;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
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

        if(!deckContentControl){
            deckContentControl = GetComponentInParent<DeckContentControl>();
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Debug.Log("SingleSelectedCard OnEnable()", this);
        CardUI.OnCardSelected -= SetCurrentSelectedCard;
        CardUI.OnCardSelected += SetCurrentSelectedCard;
    }

    public void SetCurrentSelectedCard(CardUI cardToSet){
        if(currentSelectedCard != null){
            currentSelectedCard.DeselectCard();
        }

        currentSelectedCard = cardToSet;
        
        if(cardToSet == null){
            cardImage.sprite = null;
            cardImage.color = Color.clear;
            return;
        } 
        
        cardImage.sprite = cardToSet.Card.sprite;
        cardImage.color = Color.white;
    }

    public void AddToDeck(){
        if(currentSelectedCard.CurrentAmountInCollection < 1 || (deckContentControl.DeckPointsCurrent + currentSelectedCard.Card.cost) > deckContentControl.DeckPointsMax) return;
        if(OnTryAddToDeck != null){
            OnTryAddToDeck(currentSelectedCard, 1);
        }
    }

    public void RemoveFromDeck(){
        if(currentSelectedCard.CurrentAmountInDeck < 1) return;
        if(OnReturnToCollection != null){
            OnReturnToCollection(currentSelectedCard, 1);
        }
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData){
        if(currentSelectedCard){
            if(dragActivationUp){
                AddToDeck();
            } 
            if(dragActivationDown){
                RemoveFromDeck();
            }
        }

        base.OnEndDrag(eventData);
    }

    public override void DragActivationVisuals(bool dragging)
    {
        if(dragging){
            if(dragActivationUp) {
                cardBackgroundImage.color = Color.green;
            } else if(dragActivationDown){
                cardBackgroundImage.color = Color.red;
            } else {
                cardBackgroundImage.color = Color.clear;
            }
        } else {
            cardBackgroundImage.color = Color.clear;
        }

    }


    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardUI.OnCardSelected -= SetCurrentSelectedCard;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardSelectionSlot : MonoBehaviour
{

    public delegate int AddToDeckFromCollectionDelegate(string cardID, string cardName, int amountToAdd = 1);
    public static event AddToDeckFromCollectionDelegate OnAddToDeck;

    public delegate int ReturnToCollectionFromDeckDelegate(string cardID, string cardName, int amountToAdd = 1);
    public static event ReturnToCollectionFromDeckDelegate OnReturnToCollection;
    
    private Image image;

    [ReadOnly(true), SerializeField, ContextMenuItem("Add to deck", "AddToDeck"), ContextMenuItem("Remove from deck", "RemoveFromDeck")] private CardUI currentSelectedCard;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        image = GetComponent<Image>();
        image.preserveAspect = true;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Debug.Log("SingleSelectedCard OnEnable()", this);
        CardUI.onCardSelected -= SetCurrentSelectedCard;
        CardUI.onCardSelected += SetCurrentSelectedCard;
    }

    public void SetCurrentSelectedCard(CardUI cardToSet){
        currentSelectedCard = cardToSet;
        if(cardToSet != null){
            image.sprite = cardToSet.Card.sprite;
            image.color = Color.white;
        } else {
            image.sprite = null;
            image.color = Color.clear;
        }
    }

    public void AddToDeck(){
        if(currentSelectedCard.CurrentAmountInCollection < 1) return;
        if(OnAddToDeck != null){
            OnAddToDeck(currentSelectedCard.Card.ID, currentSelectedCard.Card.cardName, 1);
            currentSelectedCard.MoveCard(true, 1);
        }
    }

    public void RemoveFromDeck(){
        if(currentSelectedCard.CurrentAmountInDeck < 1) return;
        if(OnReturnToCollection != null){
            OnReturnToCollection(currentSelectedCard.Card.ID, currentSelectedCard.Card.cardName, 1);
            currentSelectedCard.MoveCard(false, 1);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardUI.onCardSelected -= SetCurrentSelectedCard;
    }
}

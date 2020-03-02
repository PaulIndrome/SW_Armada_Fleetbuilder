using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntrySquadron : DeckEntry, IComparable<DeckEntrySquadron> {

    // private string identifier = "";
    // public string Identifier => identifier;

    // [SerializeField] CardUnityBase card = null;
    // public CardUnityBase Card => card;

    // [SerializeField] private int amountInDeck = -1;
    // public int AmountInDeck => amountInDeck;

    // [SerializeField] private DeckEntryShip assignedToShip;
    // public DeckEntryShip AssignedToShip => assignedToShip;

    [SerializeField] private CardUnitySquadron card = null;
    public CardUnitySquadron Card => card;


    public DeckEntrySquadron(CardUnitySquadron squadronCard){
        card = squadronCard;
        // amountInDeck = newAmount;
        identifier = squadronCard.ID;
    }

    // public void Add(int amountToAdd = 1){
    //     amountInDeck = Mathf.Clamp(amountInDeck + amountToAdd, 0, card.isUnique ? 1 : 999);
    // }

    // public void Remove(int amountToRemove = 1){
    //     if(amountInDeck < 1) return;
    //     amountInDeck = Mathf.Clamp(amountInDeck - amountToRemove, 0, card.isUnique ? 1 : amountInDeck);
    // }

    public int CompareTo(DeckEntrySquadron other)
    {
        return card.cardName.CompareTo(other.Card.cardName);
    }
}
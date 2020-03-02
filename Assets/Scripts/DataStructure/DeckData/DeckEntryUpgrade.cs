using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntryUpgrade : DeckEntry, IComparable<DeckEntryUpgrade> {

    // private string identifier = "";
    // public string Identifier => identifier;

    // [SerializeField] CardUnityBase card = null;
    // public CardUnityBase Card => card;

    // [SerializeField] private int amountInDeck = -1;
    // public int AmountInDeck => amountInDeck;

    public bool IsAssigned {
        get { return assignedToShipID != null && assignedToShipID.Length > 0; }
    }

    [SerializeField] private CardUnityUpgrade card = null;
    public CardUnityUpgrade Card => card;

    [SerializeField] private string assignedToShipID;
    public string AssignedToShipID => assignedToShipID;


    public DeckEntryUpgrade(CardUnityUpgrade upgradeCard){
        card = upgradeCard;
        // amountInDeck = newAmount;
        identifier = upgradeCard.ID;
    }

    public bool AssignToShip(DeckEntryShip ship = null){
        assignedToShipID = ship.Card.ID;
        return IsAssigned;
    }

    // public void Add(int amountToAdd = 1){
    //     amountInDeck = Mathf.Clamp(amountInDeck + amountToAdd, 0, card.isUnique ? 1 : 999);
    // }

    // public void Remove(int amountToRemove = 1){
    //     if(amountInDeck < 1) return;
    //     amountInDeck = Mathf.Clamp(amountInDeck - amountToRemove, 0, card.isUnique ? 1 : amountInDeck);
    // }

    public int CompareTo(DeckEntryUpgrade other)
    {
        return card.cardName.CompareTo(other.Card.cardName);
    }
}
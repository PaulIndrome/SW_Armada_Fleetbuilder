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
        get { return assignedToShip != null; }
    }

    [SerializeField] new private CardUnityUpgrade card = null;
    new public CardUnityUpgrade Card => card;

    [SerializeField] private DeckEntryShip assignedToShip;
    public DeckEntryShip AssignedToShip => assignedToShip;


    public DeckEntryUpgrade(CardUnityUpgrade upgradeCard, int newAmount){
        card = upgradeCard;
        amountInDeck = newAmount;
        identifier = upgradeCard.ID;
    }

    public bool AssignToShip(DeckEntryShip ship = null){
        assignedToShip = ship;
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
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntryUpgrade : DeckEntry, IComparable<DeckEntryUpgrade> {

    // private string identifier = "";
    // public string Identifier => identifier;

    // [SerializeField] private CardUnityBase card = null;
    // public CardUnityBase Card => card;

    // [SerializeField] private int amountInDeck = -1;
    // public int AmountInDeck => amountInDeck;

    // [SerializeField] private CardUnityUpgrade card = null;
    // public CardUnityUpgrade Card => card;

    public bool IsAssigned {
        get { return assignedToShipID != null && assignedToShipID.Length > 0; }
    }

    [SerializeField] private UpgradeType[] upgradeTypes;
    public UpgradeType[] UpgradeTypes => upgradeTypes;

    [SerializeField] private string assignedToShipID = null;
    public string AssignedToShipID => assignedToShipID;

    // [SerializeField] private DeckEntryShip assignedToShip;
    // public DeckEntryShip AssignedToShip => assignedToShip;


    public DeckEntryUpgrade(CardUnityUpgrade upgradeCard){
        card = upgradeCard;
        // amountInDeck = newAmount;
        identifier = upgradeCard.ID;
        upgradeTypes = upgradeCard.upgradeTypes;
    }

    public bool AssignToShip(DeckEntryShip ship = null){
        if(ship == null){
            assignedToShipID = "";
            // assignedToShip = null;
            return false;
        }
        assignedToShipID = ship.Card.ID;
        // assignedToShip = ship;
        return IsAssigned;
    }

    public bool UnAssign(){
        return AssignToShip(null);
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
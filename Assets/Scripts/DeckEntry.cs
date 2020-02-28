using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DeckEntry {

    protected string identifier = "";
    public string Identifier => identifier;
    [SerializeField] protected CardUnityBase card = null;
    public CardUnityBase Card => card;

    [SerializeField] protected int amountInDeck = -1;
    public int AmountInDeck => amountInDeck;

    // public DeckEntry(CardUnityShip shipCard, int newAmount){
    //     card = shipCard;
    //     amountInDeck = newAmount;
    //     identifier = shipCard.ID;
    // }
    // public DeckEntry(CardUnitySquadron squadronCard, int newAmount){
    //     card = squadronCard;
    //     amountInDeck = newAmount;
    //     identifier = squadronCard.ID;
    // }

    // public DeckEntry(CardUnityUpgrade upgradeCard, int newAmount){
    //     card = upgradeCard;
    //     amountInDeck = newAmount;
    //     identifier = upgradeCard.ID;
    // }

    public void Add(int amountToAdd = 1){
        amountInDeck = Mathf.Clamp(amountInDeck + amountToAdd, 0, card.isUnique ? 1 : 999);
    }

    public void Remove(int amountToRemove = 1){
        if(amountInDeck < 1) return;
        amountInDeck = Mathf.Clamp(amountInDeck - amountToRemove, 0, card.isUnique ? 1 : amountInDeck);
    }

    // public int CompareTo(DeckEntry other)
    // {
    //     return card.cardName.CompareTo(other.Card.cardName);
    // }
}
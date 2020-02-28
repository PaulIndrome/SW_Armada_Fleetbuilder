using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck : ScriptableObject {

    [ReadOnly, SerializeField] private int pointsCurrent;
    public int CurrentPoints => pointsCurrent;

    [ReadOnly, SerializeField] private int pointsMax;
    public int PointsMax => pointsMax;

    [ReadOnly(true), SerializeField] private string deckName;
    public string DeckName => deckName;

    [ReadOnly(true), SerializeField] private Faction deckFaction;
    public Faction DeckFaction => deckFaction;

    // split lists or not? 

    public List<DeckEntryShip> shipCards = new List<DeckEntryShip>();
    public List<DeckEntrySquadron> squadronCards = new List<DeckEntrySquadron>();
    public List<DeckEntryUpgrade> upgradeCards = new List<DeckEntryUpgrade>();

    public List<DeckEntry> AllCardsInDeck {
        get {
            List<DeckEntry> all = new List<DeckEntry>();
            all.AddRange(shipCards);
            all.AddRange(squadronCards);
            all.AddRange(upgradeCards);
            return all;
        }
    }

    

    public void SetupDeck(string name, int points){
        deckName = name;
        pointsMax = points;

        if(shipCards == null)
            shipCards = new List<DeckEntryShip>();
        if(squadronCards == null)
            squadronCards = new List<DeckEntrySquadron>();
        if(upgradeCards == null)
            upgradeCards = new List<DeckEntryUpgrade>();
    }

    public void SetName(string deckName){
        this.deckName = deckName;
        name = deckName;
    }

    public void SetPointsMax(int points){
        pointsMax = points;
    }

    /// <summary>Adds an amount of the supplied card.</summary>
    /// <returns>   0 if the amount of cards to be added exceeds the point Max.
    ///             1 if a new DeckEntry of the cards to be added was created. 
    ///             2 if the amount of cards was added to an existing entry.
    ///             -1 if the card type was invalid and unable to be cast to a derived type.</returns>
    public int AddCardToDeck(CardUnityBase card, int amountToAdd = 1){
        if(pointsCurrent + (card.cost * amountToAdd) > pointsMax) return 0;

        if(card is CardUnityShip){
            DeckEntry entry = shipCards.Find(de => de.Card.ID == card.ID);
            if(entry == null){
                shipCards.Add(new DeckEntryShip(card as CardUnityShip, amountToAdd));
                return 1;
            } else {
                entry.Add(amountToAdd);
                return 2;
            }
        } else if(card is CardUnitySquadron){
            DeckEntry entry = squadronCards.Find(de => de.Card.ID == card.ID);
            if(entry == null){
                squadronCards.Add(new DeckEntrySquadron(card as CardUnitySquadron, amountToAdd));
                return 1;
            } else {
                entry.Add(amountToAdd);
                return 2;
            }
        } else if(card is CardUnityUpgrade){
            DeckEntry entry = upgradeCards.Find(de => de.Card.ID == card.ID);
            if(entry == null){
                upgradeCards.Add(new DeckEntryUpgrade(card as CardUnityUpgrade, amountToAdd));
                return 1;
            } else {
                entry.Add(amountToAdd);
                return 2;
            }
        } else {
            Debug.LogError($"Unrecognized card type of card \"{card.ID}\". Card will not be added.", card);
            return -1;
        }
    }

    /// <summary>Removes an amount of the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCard(CardUnityBase cardToRemove, int remove = 1){
        DeckEntry entry;
        if(cardToRemove is CardUnityShip){
            entry = FindShipByName(cardToRemove.cardName);
        } else if (cardToRemove is CardUnitySquadron){
            entry = FindSquadronByName(cardToRemove.cardName);
        } else {
            entry = FindUpgradeByName(cardToRemove.cardName);
        }
        if(entry != null){
            entry.Remove(remove);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>Removes an amount the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCardByID(string cardID, int remove = 1){
        DeckEntry entry = FindFirstByID(cardID);
        if(entry != null){
            entry.Remove(remove);
            return true;
        } else {
            return false;
        }
    }

    public DeckEntry FindShipByName(string cardName){
        return shipCards.Find(de => de.Card.cardName == cardName);
    }

    public DeckEntry FindSquadronByName(string cardName){
        return squadronCards.Find(de => de.Card.cardName == cardName);
    }

    public DeckEntry FindUpgradeByName(string cardName){
        return upgradeCards.Find(de => de.Card.cardName == cardName);
    }

    // public DeckEntry FindFirstByName(string cardName){
    //     return (shipCards.Find(de => de.Card.cardName == cardName) ?? 
    //             squadronCards.Find(de => de.Card.cardName == cardName) ?? 
    //             upgradeCards.Find(de => de.Card.cardName == cardName));
    // }

    public List<DeckEntry> FindAllByName(string cardName){
        List<DeckEntry> identicalNamedEntries = new List<DeckEntry>();
        identicalNamedEntries.AddRange(shipCards.FindAll(de => de.Card.cardName == cardName));
        identicalNamedEntries.AddRange(squadronCards.FindAll(de => de.Card.cardName == cardName));
        identicalNamedEntries.AddRange(upgradeCards.FindAll(de => de.Card.cardName == cardName));
        return identicalNamedEntries;
    }

    public DeckEntry FindFirstByID(string cardID){
        return (shipCards.Find(de => de.Card.ID == cardID) ?? 
                (DeckEntry) squadronCards.Find(de => de.Card.ID == cardID) ?? 
                upgradeCards.Find(de => de.Card.ID == cardID));
    }

    public List<DeckEntry> FindAllByID(string cardID){
        List<DeckEntry> identicalIDEntries = new List<DeckEntry>();
        identicalIDEntries.AddRange(shipCards.FindAll(de => de.Card.ID == cardID));
        identicalIDEntries.AddRange(squadronCards.FindAll(de => de.Card.ID == cardID));
        identicalIDEntries.AddRange(upgradeCards.FindAll(de => de.Card.ID == cardID));
        return identicalIDEntries;
    }

    // public List<DeckEntry> FindAllOfCardSize(CardSize cardSize){
    //     switch(cardSize){
    //         case CardSize.Large:
    //             return shipCards;
    //         case CardSize.Normal:
    //             return squadronCards;
    //         case CardSize.Small:
    //             return upgradeCards;
    //         default:
    //             throw new ArgumentException("FindAllOfCardType was called with an invalid CardSize.");
    //     }
    // }

    public List<DeckEntryShip> FindAllShipsOfType(ShipType shipType){
        return shipCards.FindAll(de => {
            CardUnityShip shipCard = de.Card as CardUnityShip;
            return shipCard.shipType == shipType;
        });
    }

    public List<DeckEntrySquadron> FindAllSquadronsOfType(SquadronType squadronType){
        return squadronCards.FindAll(de => {
            CardUnitySquadron squadronCard = de.Card as CardUnitySquadron;
            // Debug.Log($"Checking {squadronCard.ID}, {squadronType} == {squadronCard.squadronType} -> {squadronCard.squadronType == squadronType}", squadronCard);
            return squadronCard.squadronType == squadronType;
        });
    }

    public List<DeckEntryUpgrade> FindAllUpgradesOfType(UpgradeType upgradeType){
        return upgradeCards.FindAll(de => {
            CardUnityUpgrade upgradeCard = de.Card as CardUnityUpgrade;
            return upgradeCard.upgradeType == upgradeType;
        });
    }

}

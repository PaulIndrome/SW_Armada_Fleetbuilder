using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck {

    [ReadOnly, SerializeField] private int pointsCurrent;
    public int CurrentPoints => pointsCurrent;

    [ReadOnly, SerializeField] private int pointsMax;
    public int PointsMax => pointsMax;

    [ReadOnly(true), SerializeField] private string deckName;
    public string DeckName => deckName;

    [ReadOnly(true), SerializeField] private Faction deckFaction;
    public Faction DeckFaction => deckFaction;

    [SerializeField] private int shipPoints = 0;
    [SerializeField] private int squadronPoints = 0;
    [SerializeField] private int upgradePoints = 0;

    public int ShipPoints => shipPoints;
    public int SquadronPoints => squadronPoints;
    public int UpgradePoints => upgradePoints;

    

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

    public Deck(SerializableDeck sDeck){
        deckName = sDeck.deckName;
        pointsMax = sDeck.deckPointsMax;
        pointsCurrent = sDeck.deckPointsUsed;

        deckFaction = sDeck.deckFaction;

        for(int i = 0; i < sDeck.upgradesInDeck.Length; i++){
            DeckEntryUpgrade entryUpgrade = new DeckEntryUpgrade(Resources.Load<CardUnityUpgrade>($"CardUnity/{sDeck.upgradesInDeck[i].cardID}"));
            upgradeCards.Add(entryUpgrade);
            upgradePoints += entryUpgrade.Card.cost;
        }
        for(int i = 0; i < sDeck.shipsInDeck.Length; i++){
            DeckEntryShip entryShip = new DeckEntryShip(Resources.Load<CardUnityShip>($"CardUnity/{sDeck.shipsInDeck[i].cardID}"));
            shipCards.Add(entryShip);
            shipPoints += entryShip.Card.cost;
            for(int u = 0; u < sDeck.shipsInDeck[i].upgradeCardIDs.Length; u++){
                if(sDeck.shipsInDeck[i].upgradeCardIDs[u] == null || sDeck.shipsInDeck[i].upgradeCardIDs[u].Length < 1) continue;
                DeckEntryUpgrade entryU =  upgradeCards.Find(uc => uc.Card.ID == sDeck.shipsInDeck[i].upgradeCardIDs[u] && !uc.IsAssigned);
                if(!entryShip.SlotUpgrade(entryU, u)){
                    Debug.LogError($"Upgrade {sDeck.shipsInDeck[i].upgradeCardIDs[u]} ({sDeck.shipsInDeck[i].upgradeCardIDs[u].Length}) | Slot {u} ({entryShip.UpgradeSlots[u].UpgradeType}) | Ship {i} ({entryShip.Card.ID}) | Deck {sDeck.deckName} could not be filled. No non-assigned upgrade availabe.");
                }
            }
        }
        for(int i = 0; i < sDeck.squadronsInDeck.Length; i++){
            DeckEntrySquadron entrySquadron = new DeckEntrySquadron(Resources.Load<CardUnitySquadron>($"CardUnity/{sDeck.squadronsInDeck[i].cardID}"));
            squadronCards.Add(entrySquadron);
            squadronPoints += entrySquadron.Card.cost;
        }
    }

    public Deck(string name, int points){
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
    }

    public void SetPointsMax(int points){
        pointsMax = points;
    }

    /// <summary>Adds an amount of the supplied card.</summary>
    /// <returns>   
    ///             -2 if adding a squadron card would exceed 1/3 of the points max in squadron points.
    ///             -1 if the card type was invalid and unable to be cast to a derived type.
    ///             0 if the amount of cards to be added exceeds the point Max.
    ///             1 if a DeckEntry of the card was added.
    /// </returns>
    public int AddCardToDeck(CardUnityBase card, out DeckEntry newEntry){
        if(pointsCurrent + (card.cost) > pointsMax) {
            newEntry = null;
            return 0;
        }

        DeckEntry entryToAdd;

        if(card is CardUnityShip){
            // entryToAdd = ScriptableObject.CreateInstance<DeckEntryShip>();
            entryToAdd = new DeckEntryShip(card as CardUnityShip);
            shipPoints += card.cost;
            shipCards.Add(entryToAdd as DeckEntryShip);
        } else if(card is CardUnitySquadron){
            if((squadronPoints + card.cost) * 3 > pointsMax){ 
                newEntry = null;
                return -2;
            }
            // entryToAdd = ScriptableObject.CreateInstance<DeckEntrySquadron>();
            entryToAdd = new DeckEntrySquadron(card as CardUnitySquadron);
            squadronPoints += card.cost;
            squadronCards.Add(entryToAdd as DeckEntrySquadron);
        } else if(card is CardUnityUpgrade){
            // entryToAdd = ScriptableObject.CreateInstance<DeckEntryUpgrade>();
            entryToAdd = new DeckEntryUpgrade(card as CardUnityUpgrade);
            upgradePoints += card.cost;
            upgradeCards.Add(entryToAdd as DeckEntryUpgrade);
        } else {
            Debug.LogError($"Unrecognized card type of card \"{card.ID}\". Card will not be added.", card);
            newEntry = null;
            return -1;
        }

        newEntry = entryToAdd;
        pointsCurrent += card.cost;
        return 1;
    }

    public int RemoveEntry(DeckEntry entry){
        Debug.Log("Removing entry " + entry.Identifier);
        if(entry is DeckEntryShip){
            if(shipCards.Remove(entry as DeckEntryShip)){
                pointsCurrent -= ((DeckEntryShip) entry).Card.cost;
                shipPoints -= ((DeckEntryShip) entry).Card.cost;
                return 1;
            } else {
                return 0;
            }
        } else if (entry is DeckEntrySquadron){
            if(squadronCards.Remove(entry as DeckEntrySquadron)){
                pointsCurrent -= ((DeckEntrySquadron) entry).Card.cost;
                squadronPoints -= ((DeckEntrySquadron) entry).Card.cost;
                return 1;
            } else {
                return 0;
            }
        } else if(entry is DeckEntryUpgrade){
            if(upgradeCards.Remove(entry as DeckEntryUpgrade)){
                pointsCurrent -= ((DeckEntryUpgrade) entry).Card.cost;
                upgradePoints -= ((DeckEntryUpgrade) entry).Card.cost;
                return 1;
            } else {
                return 0;
            }
        } else 
            return 0;
    }

    // /// <summary>Removes an amount of the supplied card.</summary>
    // /// <returns>True if a card was removed, false if not.</returns>
    // public bool RemoveCard(CardUnityBase cardToRemove, int remove = 1){
    //     DeckEntry entry;
    //     if(cardToRemove is CardUnityShip){
    //         entry = FindShipByName(cardToRemove.cardName);
    //     } else if (cardToRemove is CardUnitySquadron){
    //         entry = FindSquadronByName(cardToRemove.cardName);
    //     } else {
    //         entry = FindUpgradeByName(cardToRemove.cardName);
    //     }
    //     if(entry != null){
    //         entry.Remove(remove);
    //         return true;
    //     } else {
    //         return false;
    //     }
    // }

    // /// <summary>Removes an amount the supplied card.</summary>
    // /// <returns>True if a card was removed, false if not.</returns>
    // public bool RemoveCardByID(string cardID, int remove = 1){
    //     DeckEntry entry = FindFirstByID(cardID);
    //     if(entry.Card is CardUnityShip){
            
    //     } else if (entry.Card is CardUnitySquadron){

    //     } else {

    //     }
    //     // if(entry != null){
    //     //     entry.Remove(remove);
    //     //     return true;
    //     // } else {
    //     //     return false;
    //     // }
    // }

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

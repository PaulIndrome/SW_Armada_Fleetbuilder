using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCollection : ScriptableObject
{
    [SerializeField] List<CardCollectionEntry> cardCollectionEntries = new List<CardCollectionEntry>();

    [SerializeField] List<CardCollectionEntry> shipCollectionEntries        = new List<CardCollectionEntry>();
    [SerializeField] List<CardCollectionEntry> squadronCollectionEntries    = new List<CardCollectionEntry>();
    [SerializeField] List<CardCollectionEntry> upgradeCollectionEntries     = new List<CardCollectionEntry>();


    // public List<CardCollectionEntry> CardCollectionEntries => cardCollectionEntries;

    public List<CardCollectionEntry> ShipCollectionEntries => shipCollectionEntries;
    public List<CardCollectionEntry> SquadronCollectionEntries => squadronCollectionEntries;
    public List<CardCollectionEntry> UpgradeCollectionEntries => upgradeCollectionEntries;
    

    [ContextMenu("Split collection into card types")]
    public void SplitCollectionIntoCardTypes(){
        cardCollectionEntries.Sort();

        for(int i = 0; i < cardCollectionEntries.Count; i++){
            CardCollectionEntry entry = cardCollectionEntries[i];
            if(entry.card is CardUnityShip && !shipCollectionEntries.Contains(entry)){
                shipCollectionEntries.Add(entry);
            } else if(entry.card is CardUnitySquadron && !squadronCollectionEntries.Contains(entry)){
                squadronCollectionEntries.Add(entry);
            } else if(entry.card is CardUnityUpgrade && !upgradeCollectionEntries.Contains(entry)) {
                upgradeCollectionEntries.Add(entry);
            } else {
                Debug.LogError($"{cardCollectionEntries[i].card.ID} is not of a valid CardUnityBase derived type. Skipping.");
                continue;
            }
        }
    }

    public void AddCard(CardUnityBase cardToAdd, int add = 1){
        // Debug.Log($"Trying to add card {cardToAdd.ID} to collection");
        CardCollectionEntry entry = FindByName(cardToAdd.cardName);
        if(entry != null){
            entry.AmountMax += add;
        } else {
            cardCollectionEntries.Add(new CardCollectionEntry(cardToAdd, add));
        }
    }

    // public void AddCard(string cardName, int add = 1){
    //     CardCollectionEntry entry = FindByName(cardName);
    //     if(entry != null){
    //         entry.amount += add;
    //     } else {
    //         cardCollectionEntries.Add(new CardCollectionEntry(Resources.Load<CardUnity>($"CardData/{cardName}"), add));
    //     }
    // }

    /// <summary>Removes an amount of the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCard(CardUnityBase cardToRemove, int remove = 1){
        CardCollectionEntry entry = FindByName(cardToRemove.cardName);
        if(entry != null){
            entry.AmountMax = Mathf.Clamp(entry.AmountMax - remove, 0, entry.AmountMax);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>Removes an amount the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCard(string cardName, int remove = 1){
        CardCollectionEntry entry = FindByName(cardName);
        if(entry != null){
            entry.AmountMax = Mathf.Clamp(entry.AmountMax - remove, 0, entry.AmountMax);
            return true;
        } else {
            return false;
        }
    }

    public void ModifyCollection(CardUnityBase card = null, /* string cardName = "", */ bool add = true, int change = 1){
        if((card == null /* && cardName.Length < 1 */) || change < 1) return;

        // if(cardName.Length > 0){
        //     if(add)
        //         AddCard(cardName, change);
        //     else 
        //         RemoveCard(cardName, change);
        // } else {
        if(add)
            AddCard(card, change);
        else 
            RemoveCard(card, change);
        // }
    }

    public CardCollectionEntry FindByName(string cardName){
        return cardCollectionEntries.Find(cce => cce.card.cardName == cardName);
    }

    public List<CardCollectionEntry> FindAllByName(string cardName){
        List<CardCollectionEntry> identicalNamedEntries = new List<CardCollectionEntry>();
        identicalNamedEntries.AddRange(shipCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        identicalNamedEntries.AddRange(squadronCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        identicalNamedEntries.AddRange(upgradeCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        return identicalNamedEntries;
    }

    public CardCollectionEntry FindByID(string cardID){
        return cardCollectionEntries.Find(cce => cce.card.ID == cardID);
    }

    public List<CardCollectionEntry> FindAllByID(string cardID){
        return cardCollectionEntries.FindAll(cce => cce.card.ID == cardID);
    }

    public List<CardCollectionEntry> FindAllOfCardType(CardSize cardSize){
        switch(cardSize){
            case CardSize.Large:
                return shipCollectionEntries;
            case CardSize.Normal:
                return squadronCollectionEntries;
            case CardSize.Small:
                return upgradeCollectionEntries;
            default:
                throw new ArgumentException("FindAllOfCardType was called with an invalid CardSize.");
        }
        // return cardCollectionEntries.FindAll(cce => cce.card.cardSize == cardSize);
    }



    public List<CardCollectionEntry> FindAllShipsOfType(ShipType shipType){
        return shipCollectionEntries.FindAll(cce => {
            CardUnityShip shipCard = cce.card as CardUnityShip;
            return shipCard.shipType == shipType;
        });
    }

    public List<CardCollectionEntry> FindAllSquadronsOfType(SquadronType squadronType){
        return squadronCollectionEntries.FindAll(cce => {
            CardUnitySquadron squadronCard = cce.card as CardUnitySquadron;
            // Debug.Log($"Checking {squadronCard.ID}, {squadronType} == {squadronCard.squadronType} -> {squadronCard.squadronType == squadronType}", squadronCard);
            return squadronCard.squadronType == squadronType;
        });
    }

    public List<CardCollectionEntry> FindAllUpgradesOfType(UpgradeType upgradeType){
        return upgradeCollectionEntries.FindAll(cce => {
            CardUnityUpgrade upgradeCard = cce.card as CardUnityUpgrade;
            return upgradeCard.upgradeType == upgradeType;
        });
    }

    [ContextMenu("Reset Card Amounts")]
    public void ResetCardAmounts(){
        foreach(CardCollectionEntry cce in cardCollectionEntries){
            cce.AmountRemaining = cce.AmountMax;
        }
        foreach(CardCollectionEntry cce in shipCollectionEntries){
            cce.AmountRemaining = cce.AmountMax;
        }
        foreach(CardCollectionEntry cce in squadronCollectionEntries){
            cce.AmountRemaining = cce.AmountMax;
        }
        foreach(CardCollectionEntry cce in upgradeCollectionEntries){
            cce.AmountRemaining = cce.AmountMax;
        }
    }

    [ContextMenu("Sort all collections by identifier")]
    public void SortAllCollectionsByIdentifier(){
        foreach(CardCollectionEntry cce in cardCollectionEntries){
            if(cce.Identifier != cce.card.ID){
                cce.UpdateIdentifier(cce.card.ID);
            }
        }
        foreach(CardCollectionEntry cce in shipCollectionEntries){
            if(cce.Identifier != cce.card.ID){
                cce.UpdateIdentifier(cce.card.ID);
            }
        }
        foreach(CardCollectionEntry cce in squadronCollectionEntries){
            if(cce.Identifier != cce.card.ID){
                cce.UpdateIdentifier(cce.card.ID);
            }
        }
        foreach(CardCollectionEntry cce in upgradeCollectionEntries){
            if(cce.Identifier != cce.card.ID){
                cce.UpdateIdentifier(cce.card.ID);
            }
        }
        cardCollectionEntries.Sort();
        shipCollectionEntries.Sort();
        squadronCollectionEntries.Sort();
        upgradeCollectionEntries.Sort();
    }


    /// <summary>Called when a card is added from a collection to a deck. </summary>
    /// <remarks>   Reduces the amountRemaining of all cards with identical ID by the supplied amount.
    ///             Checks if cards with an identical name exist and reduces their amountRemaing as well. </remarks>
    /// <returns>   
    ///             0 if no amount change took place. 
    ///             1 if only entries with identical ID were changed. 
    ///             2 if entries with identical names were changed as well.</returns>
    public int PickFromCollection(CardUI cardUI, int amountToAdd = 1){
        if(amountToAdd < 1) return 0;

        // collection of all cards with the same ship/squad/upgrade type
        List<CardCollectionEntry> cces;
        int maxAmountOfTypeRemaining = -1;

        if(cardUI.Card is CardUnityShip){
            cces = FindAllShipsOfType(((CardUnityShip)cardUI.Card).shipType);
        } else if(cardUI.Card is CardUnitySquadron){
            cces = FindAllSquadronsOfType(((CardUnitySquadron)cardUI.Card).squadronType);
        } else {
            // upgrades are cards without miniatures tied to them
            // decrementing other cards of the same type is not necessary in this case
            cces = FindAllUpgradesOfType(((CardUnityUpgrade)cardUI.Card).upgradeType);
            // cces = new List<CardCollectionEntry>();
        }
        for(int i = 0; i < cces.Count; i++){
            maxAmountOfTypeRemaining = Mathf.Max(maxAmountOfTypeRemaining, cces[i].AmountRemaining);
        }
            
        // cardUI.Card.MoveCard(true, amountToAdd);

        for(int i = 0; i < cces.Count; i++){
            if(cardUI.Card is CardUnityUpgrade && ((CardUnityUpgrade) cardUI.Card).upgradeType != UpgradeType.commander && cces[i].card != cardUI.Card) continue;
            // if(cces[i].card != cardUI.Card && cces[i].card.isUnique && maxAmountOfTypeRemaining > 1) continue;
            if(cces[i].card != cardUI.Card && cces[i].card.isUnique){
                if(maxAmountOfTypeRemaining > 1 || cces[i].AmountRemaining < 1) continue;
                else {
                    //Debug.Log($"Entry {cces[i].Identifier} will become unavailable.");
                    cces[i].card.ToggleCardAvailability(false);
                    continue;
                }
            }
            
            cces[i].AmountRemaining -= amountToAdd;
            cces[i].card.MoveCard(cardUI.Card.ID, true, amountToAdd);
        }
        
        cces.Clear();
        cces = FindAllByName(cardUI.Card.cardName);
        if(cces.Count > 1){
            foreach(CardCollectionEntry cce in cces){
                // Debug.Log(cce.Identifier);
                // cce.AmountRemaining -= amountToAdd;
                if(cce.card == cardUI.Card) continue;
                cce.card.ToggleCardAvailability(false);
            }
        } else {
            return 1;
        }
        
        return 2;
    }

    /// <summary>Called when a card is returned from a deck to a collection. </summary>
    /// <remarks>   Increases the amountRemaining of all cards with identical ID by the supplied amount.
    ///             Checks if cards with an identical name exist and increases their amountRemaing as well. </remarks>
    /// <returns>   
    ///             0 if no amount change took place. 
    ///             1 if only entries with identical ID were changed. 
    ///             2 if entries with identical names were changed as well.</returns>
    public int ReturnToCollection(CardUI cardUI, int amountToAdd = 1){
        if(amountToAdd < 1) return 0;
        
        // List<CardCollectionEntry> cces = FindAllByID(cardID);
        // foreach(CardCollectionEntry cce in cces){
        //     cce.AmountRemaining += amountToAdd;
        // }

        // alternative: 
        // - isUnique cards can only be returned to collection by moving on the specific ID ui card

        // TODO deck data structure and link up

        List<CardCollectionEntry> cces;

        if(cardUI.Card is CardUnityShip){
            cces = FindAllShipsOfType(((CardUnityShip)cardUI.Card).shipType);
        } else if(cardUI.Card is CardUnitySquadron){
            cces = FindAllSquadronsOfType(((CardUnitySquadron)cardUI.Card).squadronType);
        } else {
            // upgrades are cards without miniatures tied to them
            // these cards are not decremented when an identical type card is added to the deck
            // we don't need to re-increment these when a card is returned from the deck
            cces = FindAllUpgradesOfType(((CardUnityUpgrade)cardUI.Card).upgradeType);
            // cces = new List<CardCollectionEntry>();

        }

        for(int i = 0; i < cces.Count; i++){
            if(cardUI.Card is CardUnityUpgrade && ((CardUnityUpgrade) cardUI.Card).upgradeType != UpgradeType.commander && cces[i].card != cardUI.Card ) continue;
            //if(cces[i].card.isUnique && cardUI.Card.ID != cces[i].card.ID/*  && cces[i].AmountRemaining > 0 */) continue;
            if(cces[i].card.isUnique && cardUI.Card != cces[i].card){
                if(cces[i].AmountRemaining > 0){
                    cces[i].card.ToggleCardAvailability(true);
                } else {
                    continue;
                }
            }
            cces[i].AmountRemaining += amountToAdd;
            cces[i].card.MoveCard(cardUI.Card.ID, false, amountToAdd);
        }

        cces.Clear();
        cces = FindAllByName(cardUI.Card.cardName);
        if(cces.Count > 1){
            foreach(CardCollectionEntry cce in cces){
                // cce.AmountRemaining += amountToAdd;
                if(cce.card == cardUI.Card) continue;
                cce.card.ToggleCardAvailability(true);
            }
        } else {
            return 1;
        }
        
        return 2;
    }

}

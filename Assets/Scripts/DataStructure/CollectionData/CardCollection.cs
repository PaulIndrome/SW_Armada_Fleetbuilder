using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCollection : ScriptableObject
{
    List<CardCollectionEntry> allCollectionEntries = new List<CardCollectionEntry>();

    [SerializeField] List<CardCollectionEntry> shipCollectionEntries        = new List<CardCollectionEntry>();
    [SerializeField] List<CardCollectionEntry> squadronCollectionEntries    = new List<CardCollectionEntry>();
    [SerializeField] List<CardCollectionEntry> upgradeCollectionEntries     = new List<CardCollectionEntry>();

    public List<CardCollectionEntry> ShipCollectionEntries => shipCollectionEntries;
    public List<CardCollectionEntry> SquadronCollectionEntries => squadronCollectionEntries;
    public List<CardCollectionEntry> UpgradeCollectionEntries => upgradeCollectionEntries;

    [ContextMenu("Split collection into card types")]
    public void SplitCollectionIntoCardTypes(){
        allCollectionEntries.Sort();

        for(int i = 0; i < allCollectionEntries.Count; i++){
            CardCollectionEntry entry = allCollectionEntries[i];
            if(entry.card is CardUnityShip && !shipCollectionEntries.Contains(entry)){
                shipCollectionEntries.Add(entry);
            } else if(entry.card is CardUnitySquadron && !squadronCollectionEntries.Contains(entry)){
                squadronCollectionEntries.Add(entry);
            } else if(entry.card is CardUnityUpgrade && !upgradeCollectionEntries.Contains(entry)) {
                upgradeCollectionEntries.Add(entry);
            } else {
                Debug.LogError($"{allCollectionEntries[i].card.ID} is not of a valid CardUnityBase derived type. Skipping.");
                continue;
            }
        }
    }

    public void AddCard(CardUnityBase cardToAdd, int add = 1){
        CardCollectionEntry entry;
        if(cardToAdd is CardUnityShip){
            entry = FindShipByName(cardToAdd.cardName);
            if(entry != null)
                entry.AmountMax += add;
            else 
                shipCollectionEntries.Add(new CardCollectionEntry(cardToAdd, add));
        } else if (cardToAdd is CardUnitySquadron){
            entry = FindSquadronByName(cardToAdd.cardName);
            if(entry != null)
                entry.AmountMax += add;
            else
                squadronCollectionEntries.Add(new CardCollectionEntry(cardToAdd, add));
        } else {
            entry = FindUpgradeByName(cardToAdd.cardName);
            if(entry != null)
                entry.AmountMax += add;
            else
                upgradeCollectionEntries.Add(new CardCollectionEntry(cardToAdd, add));
        }
    }

    /// <summary>Removes an amount of the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCard(CardUnityBase cardToRemove, int remove = 1){
        CardCollectionEntry entry;
        if(cardToRemove is CardUnityShip){
            entry = FindShipByName(cardToRemove.cardName);
        } else if (cardToRemove is CardUnitySquadron){
            entry = FindSquadronByName(cardToRemove.cardName);
        } else {
            entry = FindUpgradeByName(cardToRemove.cardName);
        }
        if(entry != null){
            entry.AmountMax = Mathf.Clamp(entry.AmountMax - remove, 0, entry.AmountMax);
            return true;
        } else {
            return false;
        }
    }

    /// <summary>Removes an amount the supplied card.</summary>
    /// <returns>True if a card was removed, false if not.</returns>
    public bool RemoveCardByID(string cardID, int remove = 1){
        CardCollectionEntry entry = FindFirstByID(cardID);
        if(entry != null){
            entry.AmountMax = Mathf.Clamp(entry.AmountMax - remove, 0, entry.AmountMax);
            return true;
        } else {
            return false;
        }
    }

    public void ModifyCollection(CardUnityBase card = null, /* string cardName = "", */ bool add = true, int change = 1){
        if((card == null /* && cardName.Length < 1 */) || change < 1) return;
        if(add)
            AddCard(card, change);
        else 
            RemoveCard(card, change);
    }

    public CardCollectionEntry FindShipByName(string cardName){
        return shipCollectionEntries.Find(cce => cce.card.cardName == cardName);
    }

    public CardCollectionEntry FindSquadronByName(string cardName){
        return squadronCollectionEntries.Find(cce => cce.card.cardName == cardName);
    }

    public CardCollectionEntry FindUpgradeByName(string cardName){
        return upgradeCollectionEntries.Find(cce => cce.card.cardName == cardName);
    }

    public CardCollectionEntry FindFirstByName(string cardName){
        return (shipCollectionEntries.Find(cce => cce.card.cardName == cardName) ?? 
                squadronCollectionEntries.Find(cce => cce.card.cardName == cardName) ?? 
                upgradeCollectionEntries.Find(cce => cce.card.cardName == cardName));
    }

    public List<CardCollectionEntry> FindAllByName(string cardName){
        List<CardCollectionEntry> identicalNamedEntries = new List<CardCollectionEntry>();
        identicalNamedEntries.AddRange(shipCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        identicalNamedEntries.AddRange(squadronCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        identicalNamedEntries.AddRange(upgradeCollectionEntries.FindAll(cce => cce.card.cardName == cardName));
        return identicalNamedEntries;
    }

    public CardCollectionEntry FindFirstByID(string cardID){
        return (shipCollectionEntries.Find(cce => cce.card.ID == cardID) ?? 
                squadronCollectionEntries.Find(cce => cce.card.ID == cardID) ?? 
                upgradeCollectionEntries.Find(cce => cce.card.ID == cardID));
    }

    public List<CardCollectionEntry> FindAllByID(string cardID){
        List<CardCollectionEntry> identicalIDEntries = new List<CardCollectionEntry>();
        identicalIDEntries.AddRange(shipCollectionEntries.FindAll(cce => cce.card.ID == cardID));
        identicalIDEntries.AddRange(squadronCollectionEntries.FindAll(cce => cce.card.ID == cardID));
        identicalIDEntries.AddRange(upgradeCollectionEntries.FindAll(cce => cce.card.ID == cardID));
        return identicalIDEntries;
    }

    public List<CardCollectionEntry> FindAllOfCardSize(CardSize cardSize){
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
        foreach(CardCollectionEntry cce in allCollectionEntries){
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
        foreach(CardCollectionEntry cce in allCollectionEntries){
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
        allCollectionEntries.Sort();
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
        // collection of all cards with the same ship/squad/upgrade type
        List<CardCollectionEntry> cces;
        int maxAmountOfTypeRemaining = -1;

        if(cardUI.Card is CardUnityShip){
            cces = FindAllShipsOfType(((CardUnityShip)cardUI.Card).shipType);
        } else if(cardUI.Card is CardUnitySquadron){
            cces = FindAllSquadronsOfType(((CardUnitySquadron)cardUI.Card).squadronType);
        } else {
            cces = FindAllUpgradesOfType(((CardUnityUpgrade)cardUI.Card).upgradeType);
        }
        for(int i = 0; i < cces.Count; i++){
            maxAmountOfTypeRemaining = Mathf.Max(maxAmountOfTypeRemaining, cces[i].AmountRemaining);
        }
            
        for(int i = 0; i < cces.Count; i++){
            // upgrades are cards without miniatures tied to them
            // decrementing other cards of the same type is not necessary in this case
            if(cardUI.Card is CardUnityUpgrade && ((CardUnityUpgrade) cardUI.Card).upgradeType != UpgradeType.commander && cces[i].card != cardUI.Card) continue;
            if(cces[i].card != cardUI.Card && cces[i].card.isUnique){
                if(maxAmountOfTypeRemaining > 1 || cces[i].AmountRemaining < 1) continue;
                else {
                    cces[i].card.ToggleCardAvailability(cardUI.Card.cardName, false);
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
                if(cce.card == cardUI.Card) continue;
                // cce.AmountRemaining -= amountToAdd;
                // cce.card.MoveCard(cardUI.Card.ID, true, amountToAdd);
                cce.card.ToggleCardAvailability(cardUI.Card.cardName, false);
            }
            return 2;
        } else {
            return 1;
        }
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

        // TODO deck data structure and link up

        List<CardCollectionEntry> cces;

        if(cardUI.Card is CardUnityShip){
            cces = FindAllShipsOfType(((CardUnityShip)cardUI.Card).shipType);
        } else if(cardUI.Card is CardUnitySquadron){
            cces = FindAllSquadronsOfType(((CardUnitySquadron)cardUI.Card).squadronType);
        } else {
            cces = FindAllUpgradesOfType(((CardUnityUpgrade)cardUI.Card).upgradeType);
        }

        for(int i = 0; i < cces.Count; i++){
            // upgrades are cards without miniatures tied to them
            // these cards are not decremented when an identical type card is added to the deck
            // we don't need to re-increment these when a card is returned from the deck
            if(cardUI.Card is CardUnityUpgrade && ((CardUnityUpgrade) cardUI.Card).upgradeType != UpgradeType.commander && cces[i].card != cardUI.Card && !cces[i].card.isUnique) continue;
            // skip cards that are upgrades, not commanders and not the moved card
            
            if(cces[i].card.isUnique && cardUI.Card != cces[i].card){
                // card is unique, card is not the moved card
                // Debug.Log($"{cces[i].AmountRemaining} remaining of {cces[i].card.ID}. Toggling {cces[i].AmountRemaining > 0}.");
                if(cces[i].AmountRemaining > 0){
                    // card is still available
                    cces[i].card.ToggleCardAvailability(cardUI.Card.cardName, true);
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
                if(cce.card == cardUI.Card) continue;
                // cce.AmountRemaining += amountToAdd;
                // cce.card.MoveCard(cardUI.Card.ID, false, amountToAdd);
                cce.card.ToggleCardAvailability(cardUI.Card.cardName, true);
            }
        } else {
            return 1;
        }
        
        return 2;
    }

}

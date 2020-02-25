using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardCollection : ScriptableObject
{
    [SerializeField] List<CardCollectionEntry> cardCollectionEntries = new List<CardCollectionEntry>();

    public List<CardCollectionEntry> CardCollectionEntries => cardCollectionEntries;

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
        return cardCollectionEntries.FindAll(cce => cce.card.cardName == cardName);
    }

    public CardCollectionEntry FindByID(string cardID){
        return cardCollectionEntries.Find(cce => cce.card.ID == cardID);
    }

    public List<CardCollectionEntry> FindAllByID(string cardID){
        return cardCollectionEntries.FindAll(cce => cce.card.ID == cardID);
    }

    public List<CardCollectionEntry> FindAllOfCardType(CardSize cardSize){
        return cardCollectionEntries.FindAll(cce => cce.card.cardSize == cardSize);
    }

    [ContextMenu("Reset Card Amounts")]
    public void ResetCardAmounts(){
        foreach(CardCollectionEntry cce in cardCollectionEntries){
            cce.AmountRemaining = cce.AmountMax;
        }
    }

    /// <summary>Called when a card is added from a collection to a deck. </summary>
    /// <remarks>   Reduces the amountRemaining of all cards with identical ID by the supplied amount.
    ///             Checks if cards with an identical name exist and reduces their amountRemaing as well. </remarks>
    /// <returns>   
    ///             0 if no amount change took place. 
    ///             1 if only entries with identical ID were changed. 
    ///             2 if entries with identical names were changed as well.</returns>
    public int PickFromCollection(string cardID, string cardName, int amountToAdd = 1){
        if(amountToAdd < 1) return 0;
        
        List<CardCollectionEntry> cces = FindAllByID(cardID);
        foreach(CardCollectionEntry cce in cces){
            cce.AmountRemaining -= amountToAdd;
        }
        cces.Clear();
        cces = FindAllByName(cardName);
        if(cces.Count > 1){
            foreach(CardCollectionEntry cce in cces){
                cce.AmountRemaining -= amountToAdd;
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
    public int ReturnToCollection(string cardID, string cardName, int amountToAdd = 1){
        if(amountToAdd < 1) return 0;
        
        List<CardCollectionEntry> cces = FindAllByID(cardID);
        foreach(CardCollectionEntry cce in cces){
            cce.AmountRemaining += amountToAdd;
        }
        cces.Clear();
        cces = FindAllByName(cardName);
        if(cces.Count > 1){
            foreach(CardCollectionEntry cce in cces){
                cce.AmountRemaining += amountToAdd;
            }
        } else {
            return 1;
        }
        
        return 2;
    }

}

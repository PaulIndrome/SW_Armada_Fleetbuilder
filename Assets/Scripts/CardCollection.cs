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
            entry.amount += add;
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
            entry.amount = Mathf.Clamp(entry.amount - remove, 0, entry.amount);
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
            entry.amount = Mathf.Clamp(entry.amount - remove, 0, entry.amount);
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

    public List<CardCollectionEntry> FindAllOfCardType(CardSize cardSize){
        return cardCollectionEntries.FindAll(cce => cce.card.cardSize == cardSize);
    }

}

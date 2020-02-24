using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CardCollectionEntry {
    [SerializeField, HideInInspector] private string identifier = "";
    public string Identifier => identifier;
    public CardUnityBase card = null;
    public int amount = 1;

    public CardCollectionEntry(CardUnityBase slotCard, int newAmount){
        card = slotCard;
        amount = newAmount;
        identifier = card.ID;
    }
}
using System;
using UnityEngine;

[System.Serializable]
public class CardCollectionEntry : IComparable<CardCollectionEntry>{
    [ReadOnly(true), SerializeField] private string identifier = "";
    public string Identifier => identifier;
    public CardUnityBase card = null;

    [SerializeField] private int amountMax = 1;
    [SerializeField] private int amountRemaining = 1;


    public int AmountMax {
        get { return amountMax; }
        set {
            amountMax = Mathf.Clamp(value, 0, card.isUnique || card.isCommander ? 1 : 999);
        }
    }   

    public int AmountRemaining {
        get { return amountRemaining; }
        set {
            amountRemaining = Mathf.Clamp(value, 0, amountMax);
        }
    }

    public CardCollectionEntry(CardUnityBase slotCard, int newAmount){
        card = slotCard;
        amountMax = newAmount;
        identifier = card.ID;
    }

    public void UpdateIdentifier(string newIdentifier){
        identifier = newIdentifier;
    }

    public int CompareTo(CardCollectionEntry other)
    {
        return identifier.CompareTo(other.Identifier);
    }
}
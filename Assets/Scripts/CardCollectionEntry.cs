using UnityEngine;

[System.Serializable]
public class CardCollectionEntry {
    [SerializeField, HideInInspector] private string identifier = "";
    public string Identifier => identifier;
    public CardUnityBase card = null;

    [SerializeField] private int amountMax = 1;
    [SerializeField] private int amountRemaining = 1;


    public int AmountMax {
        get { return amountMax; }
        set {
            amountMax = Mathf.Clamp(value, 0, 999);
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

    
}
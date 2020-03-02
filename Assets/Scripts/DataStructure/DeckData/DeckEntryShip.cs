using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeckEntryShip : DeckEntry, IComparable<DeckEntryShip> {

    // private string identifier = "";
    // public string Identifier => identifier;
    // [SerializeField] CardUnityBase card = null;
    // public CardUnityBase Card => card;

    // [SerializeField] private int amountInDeck = -1;
    // public int AmountInDeck => amountInDeck;

    [SerializeField] private List<UpgradeSlot> upgradeSlots;
    public List<UpgradeSlot> UpgradeSlots => upgradeSlots;

    [SerializeField] private CardUnityShip card = null;
    public CardUnityShip Card => card;

    public DeckEntryShip(CardUnityShip shipCard){
        card = shipCard;
        // amountInDeck = newAmount;
        identifier = shipCard.ID;

        upgradeSlots = new List<UpgradeSlot>();
        for(int i = 0; i < shipCard.upgradeTypes.Length; i++){
            upgradeSlots.Add(new UpgradeSlot(shipCard.upgradeTypes[i]));
        }
    }

    // public void Add(int amountToAdd = 1){
    //     amountInDeck = Mathf.Clamp(amountInDeck + amountToAdd, 0, card.isUnique ? 1 : 999);
    // }

    // public void Remove(int amountToRemove = 1){
    //     if(amountInDeck < 1) return;
    //     amountInDeck = Mathf.Clamp(amountInDeck - amountToRemove, 0, card.isUnique ? 1 : amountInDeck);
    // }

    public bool SlotUpgrade(DeckEntryUpgrade upgradeToSlot, int index = -1){
        if(upgradeToSlot == null) return false;
        // Debug.Log($"Slotting {upgradeToSlot.Card.cardName}", upgradeToSlot.Card);
        // Debug.Log($"upgradeToSlot == null? -> {upgradeToSlot == null}");
        // Debug.Log($"upgradeToSlot.Card == null? -> {upgradeToSlot.Card == null}");
        
        if(index < 0){
            List<UpgradeSlot> availableSlots = upgradeSlots.FindAll(us => us.UpgradeType == upgradeToSlot.Card.upgradeType);
            for(int i = 0; i < availableSlots.Count; i++){
                if(availableSlots[i].Filled) continue;
                if(availableSlots[i].SlotCard(upgradeToSlot)){
                    upgradeToSlot.AssignToShip(this);
                    return true;
                }
            }
        } else {
            if(upgradeSlots[index].SlotCard(upgradeToSlot)){
                upgradeToSlot.AssignToShip(this);
                return true;
            }
        }
        // no open or fitting upgradeslots
        return false;
    }

    public bool UnslotUpgrade(DeckEntryUpgrade upgradeToUnslot){
        UpgradeSlot slot = upgradeSlots.Find(us => us.SlottedUpgrade == upgradeToUnslot);
        if(slot != null){
            upgradeSlots.Remove(slot);
            return true;
        }
        // no upgradeslot to remove
        return false;
    }



    public int CompareTo(DeckEntryShip other)
    {
        return card.cardName.CompareTo(other.Card.cardName);
    }
}
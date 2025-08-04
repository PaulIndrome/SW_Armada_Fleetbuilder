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
    [SerializeField] private Dictionary<UpgradeType, (int, int)> upgradeSlotsOfType;

    // [SerializeField] private CardUnityShip card = null;
    // public CardUnityShip Card => card;

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

    public bool SlotUpgrade(DeckEntryUpgrade upgradeToSlot){
        if(upgradeToSlot == null) return false;
        // Debug.Log($"Slotting {upgradeToSlot.Card.cardName}", upgradeToSlot.Card);
        // Debug.Log($"upgradeToSlot == null? -> {upgradeToSlot == null}");
        // Debug.Log($"upgradeToSlot.Card == null? -> {upgradeToSlot.Card == null}");
        
        bool freeSlotAvailable = false;
        List<UpgradeSlot>[] availableSlotsOnShip = new List<UpgradeSlot>[upgradeToSlot.UpgradeTypes.Length];
        if(availableSlotsOnShip.Length < 1) return false;

        List<UpgradeSlot> slotsToFill = new List<UpgradeSlot>();

        bool[] slotAvailableForType = new bool[upgradeToSlot.UpgradeTypes.Length];

        for(int i = 0; i < availableSlotsOnShip.Length; i++){
            Debug.Log("Finding available slots for " + upgradeToSlot.UpgradeTypes[i]);
            availableSlotsOnShip[i] = upgradeSlots.FindAll(us => us.UpgradeSlotType == ((CardUnityUpgrade)upgradeToSlot.Card).upgradeTypes[i]);

            for(int s = 0; s < availableSlotsOnShip[i].Count; s++){
                Debug.Log("Checking to see if slot " + i + " can be slotted into");
                freeSlotAvailable = freeSlotAvailable ? true : !availableSlotsOnShip[i][s].Filled;
                if(freeSlotAvailable) {
                    slotsToFill.Add(availableSlotsOnShip[i][s]);
                    break;
                    // slotsToFill[i] = availableSlotsOnShip[i][s];
                    // if(upgradeToSlot.UpgradeTypes.Length < 2){
                    //     break;
                    // }
                }
            }
            // if there is no free slot available, overwrite the last slot of given type
            if(!freeSlotAvailable) slotsToFill.Add(availableSlotsOnShip[i][availableSlotsOnShip[i].Count - 1]);
        }

        for(int i = 0; i < slotsToFill.Count; i++){
            Debug.Log("Trying to slot upgrade into slot " + i + $"({slotsToFill[i].UpgradeSlotType})");
            if(slotsToFill[i].SlotCard(upgradeToSlot, i, true)){
                Debug.Log("Was able to slot the card into slot " + i);
                upgradeToSlot.AssignToShip(this);
            } else {
                Debug.LogError($"Can not fill checked upgrade slot for type {slotsToFill[i].UpgradeSlotType} on ship {card.ID} with upgrade {upgradeToSlot.Card.ID}");
                return false;
            }
        }
        // return freeSlotAvailable;
        return true;
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

    public int FindSlotIndexForType(UpgradeType type) {
        return upgradeSlots.FindIndex(slot => slot.UpgradeSlotType == type);
    }



    public int CompareTo(DeckEntryShip other)
    {
        return String.Compare(card.cardName, other.Card.cardName, StringComparison.Ordinal);
    }
}
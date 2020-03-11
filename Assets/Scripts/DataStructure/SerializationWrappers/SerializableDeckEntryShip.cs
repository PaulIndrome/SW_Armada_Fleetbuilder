
using Newtonsoft.Json;
using UnityEngine; 

[System.Serializable]
public class SerializableDeckEntryShip : SerializableDeckEntry {
    
    public string[] upgradeCardIDs = new string[0];

    [JsonConstructor]
    public SerializableDeckEntryShip(){}

    public SerializableDeckEntryShip(DeckEntryShip entry){
        // if(entry == null) return;
        // Debug.Log("Inside SerializableDeckEntryShip(DeckEntryShip entry)");
        if(entry.Card is CardUnityShip){
            CardUnityShip shipCard = entry.Card as CardUnityShip;
            cardID = entry.Card.ID;

            upgradeCardIDs = new string[entry.UpgradeSlots.Count];
            for(int i = 0; i < entry.UpgradeSlots.Count; i++){
                upgradeCardIDs[i] = entry.UpgradeSlots[i].SlottedUpgrade?.Card.ID;
            }
        } else {
            throw new System.ArgumentException($"{entry.Identifier} trying to create a serializable entry ship from wrong card type.");
        }
    }

}
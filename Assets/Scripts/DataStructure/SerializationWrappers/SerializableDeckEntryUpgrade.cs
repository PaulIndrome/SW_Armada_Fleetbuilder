
using Newtonsoft.Json;

[System.Serializable]
public class SerializableDeckEntryUpgrade : SerializableDeckEntry {

    public string assignedShipID = "";

    [JsonConstructor]
    public SerializableDeckEntryUpgrade(){}
    
    public SerializableDeckEntryUpgrade(DeckEntryUpgrade entry){
        // if(entry == null) return;
        if(entry.Card is CardUnityUpgrade){
            CardUnityUpgrade upgradeCard = entry.Card as CardUnityUpgrade;
            cardID = entry.Card.ID;
            if(entry.IsAssigned){
                assignedShipID = entry.AssignedToShipID;
            }
        } else {
            throw new System.ArgumentException($"{entry.Identifier} trying to create a serializable entry upgrade from wrong card type.");
        }
    }

}
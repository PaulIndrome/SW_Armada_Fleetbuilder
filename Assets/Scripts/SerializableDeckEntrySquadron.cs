
[System.Serializable]
public class SerializableDeckEntrySquadron : SerializableDeckEntry {
    public SerializableDeckEntrySquadron(DeckEntrySquadron entry){
        if(entry.Card is CardUnitySquadron){
            CardUnitySquadron squadronCard = entry.Card as CardUnitySquadron;
            cardID = entry.Card.ID;
        } else {
            throw new System.ArgumentException($"{entry.Identifier} trying to create a serializable entry squadron from wrong card type.");
        }
    }

}
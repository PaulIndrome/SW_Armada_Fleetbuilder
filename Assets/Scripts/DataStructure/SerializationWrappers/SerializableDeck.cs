using Newtonsoft.Json;
using UnityEngine;
// using System.Collections.Generic;

[System.Serializable]
public class SerializableDeck {
    public string deckName = "";
    public int deckPointsMax = -1;
    public int deckPointsUsed = -1;
    public Faction deckFaction;
    public SerializableDeckEntryShip[] shipsInDeck = new SerializableDeckEntryShip[0];
    public SerializableDeckEntrySquadron[] squadronsInDeck = new SerializableDeckEntrySquadron[0];
    public SerializableDeckEntryUpgrade[] upgradesInDeck = new SerializableDeckEntryUpgrade[0];

    [JsonConstructor]
    public SerializableDeck(){}

    public void SetUpSerializableDeck(Deck deck){
        Debug.Log("Inside SerializableDeck(Deck deck)");

        deckName = deck.DeckName;
        deckPointsMax = deck.PointsMax;
        deckPointsUsed = deck.CurrentPoints;
        deckFaction = deck.DeckFaction;

        shipsInDeck = new SerializableDeckEntryShip[deck.shipCards.Count];
        squadronsInDeck = new SerializableDeckEntrySquadron[deck.squadronCards.Count];
        upgradesInDeck = new SerializableDeckEntryUpgrade[deck.upgradeCards.Count];
        
        for(int i = 0; i < shipsInDeck.Length; i++){
            shipsInDeck[i] = new SerializableDeckEntryShip(deck.shipCards[i]);
        }
        for(int i = 0; i < squadronsInDeck.Length; i++){
            squadronsInDeck[i] = new SerializableDeckEntrySquadron(deck.squadronCards[i]);
        }
        for(int i = 0; i < upgradesInDeck.Length; i++){
            upgradesInDeck[i] = new SerializableDeckEntryUpgrade(deck.upgradeCards[i]);
        }
    }

}
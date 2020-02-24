using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable, CreateAssetMenu(menuName="Create/Card Types Lookup")]
public class CardTypesLookup : ScriptableObject {

    public TextAsset cardTypesRaw;
    public CardTypesLookupRaw cardTypesLookupRaw;
    public List<CardTypesLookupSlotShip> ships = new List<CardTypesLookupSlotShip>();
    public List<CardTypesLookupSlotSquadron> squadrons = new List<CardTypesLookupSlotSquadron>();
    public List<CardTypesLookupSlotUpgrade> upgrades = new List<CardTypesLookupSlotUpgrade>();

    public Type GetCardLookupTypeFromRaw(string cardTypeRaw){
        // Debug.Log($"Trying to find internal card type for \"{cardTypeRaw}\"");
        if(ships.Find(shs => shs.cardTypeRaw == cardTypeRaw) != null){
            return typeof(ShipType);
        } else if(squadrons.Find(sqs => sqs.cardTypeRaw == cardTypeRaw) != null){
            return typeof(SquadronType);
        } else if (upgrades.Find(ups => ups.cardTypeRaw == cardTypeRaw) != null){
            return typeof(UpgradeType);
        } else {
            throw new TypeAccessException($"No type found for \"{cardTypeRaw}\".");
        }
    }

    public ShipType GetShipType(string shipTypeRaw){
        // Debug.Log($"Trying to find ship type for \"{shipTypeRaw}\"");
        return ships.Find(st => st.cardTypeRaw == shipTypeRaw).shipType;
    }

    public SquadronType GetSquadronType(string squadronTypeRaw){
        // Debug.Log($"Trying to find squadron type for \"{squadronTypeRaw}\"");
        return squadrons.Find(sqt => sqt.cardTypeRaw == squadronTypeRaw).squadronType;
    }

    public UpgradeType GetUpgradeType(string upgradeTypeRaw){
        // UpgradeType value = upgrades.Find(ut => ut.cardTypeRaw == upgradeTypeRaw).upgradeType;
        // Debug.Log($"Trying to find upgrade type for \"{upgradeTypeRaw}\"");
        return upgrades.Find(ut => ut.cardTypeRaw == upgradeTypeRaw).upgradeType;
    }

    // public T GetCardTypeFromRaw<T>(string cardTypeRawToFind) where T : System.Enum {
    //     if(typeof(T) != typeof(ShipType) && typeof(T) != typeof(ShipType) && typeof(T) != typeof(ShipType)){
    //         throw new TypeAccessException("GetCardTypeFromRaw called with invalid type.");
    //     } 
    //     if(typeof(T) == typeof(ShipType)){
    //         return ships.Find(st => st.cardTypeRaw == cardTypeRawToFind).shipType;
    //     } else if (typeof(T) == typeof(SquadronType)){

    //     } else {

    //     }

    // }

    //[ContextMenu("Populate lookup lists from text asset")]
    void FillLookupListFromTextasset(){
        if(cardTypesRaw == null) return;
        
        GenerateCardTypesLookupRaw();

        for(int i = 0; i < cardTypesLookupRaw.large.Count; i++){
            ships.Add(new CardTypesLookupSlotShip() {cardTypeRaw = cardTypesLookupRaw.large[i]});
        }

        for(int i = 0; i < cardTypesLookupRaw.normal.Count; i++){
            squadrons.Add(new CardTypesLookupSlotSquadron() {cardTypeRaw = cardTypesLookupRaw.normal[i]});
        }

        for(int i = 0; i < cardTypesLookupRaw.small.Count; i++){
            upgrades.Add(new CardTypesLookupSlotUpgrade() {cardTypeRaw = cardTypesLookupRaw.small[i]});
        }
    }

    [ContextMenu("Generate cardTypesLookupRaw")]
    void GenerateCardTypesLookupRaw(){
        cardTypesLookupRaw = JsonConvert.DeserializeObject<CardTypesLookupRaw>(cardTypesRaw.text);
    }

    public CardSize GetCardSize(string cardTypeRaw){
        return cardTypesLookupRaw.GetSizeByTypeRaw(cardTypeRaw);
    }
}

[System.Serializable]
public class CardTypesLookupRaw {
    public List<string> large = new List<string>();
    public List<string> normal = new List<string>();
    public List<string> small = new List<string>();

    public CardSize GetSizeByTypeRaw(string cardTypeRaw){
        if(large.Contains(cardTypeRaw)) return CardSize.Large;
        else if(normal.Contains(cardTypeRaw)) return CardSize.Normal;
        else return CardSize.Small;
    }
}

[System.Serializable]
public class CardTypesLookupSlotShip {
    public string cardTypeRaw = "";

    public ShipType shipType;
}

[System.Serializable]
public class CardTypesLookupSlotSquadron {
    public string cardTypeRaw = "";

    public SquadronType squadronType;
}

[System.Serializable]
public class CardTypesLookupSlotUpgrade {
    public string cardTypeRaw = "";

    public UpgradeType upgradeType;
}
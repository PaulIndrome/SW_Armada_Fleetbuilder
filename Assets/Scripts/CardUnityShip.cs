using UnityEngine;

[System.Serializable]
public class CardUnityShip : CardUnityBase {
    public ShipType shipType;
    public UpgradeType[] upgradeTypes;
    public override void SetupCard(Card card, CardTypesLookup cardTypesLookup){
        base.SetupCard(card, cardTypesLookup);
        CardTypesLookupSlotShip cardTypesLookupSlotShip = cardTypesLookup.ships.Find(st => st.cardTypeRaw == card.cardType);
        if(cardTypesLookupSlotShip == null) {
            Debug.LogError($"No internal ship type found for raw type \"{card.cardType}\". Ship card not set up completely.");
            return;
        }
        shipType = cardTypesLookupSlotShip.shipType;

        if(card.upgradeSlots != null && card.upgradeSlots.Length > 0){
            upgradeTypes = new UpgradeType[card.upgradeSlots.Length];
            for(int i = 0; i < card.upgradeSlots.Length; i++){
                upgradeTypes[i] = cardTypesLookup.GetUpgradeType(card.upgradeSlots[i]);
            }
        }
    }
}
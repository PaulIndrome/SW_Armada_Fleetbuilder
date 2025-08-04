using UnityEngine;

[System.Serializable]
public class CardUnityShip : CardUnityBase {
    public ShipType shipType;
    public ShipSize shipSize;
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

    [ContextMenu("Add Commander Upgrade Slot")]
    public void AddCommanderUpgradeSlot(){
        UpgradeType[] newArray = new UpgradeType[upgradeTypes.Length + 1];
        newArray[0] = UpgradeType.commander;
        for(int i = 1; i < newArray.Length; i++){
            newArray[i] = upgradeTypes[i-1];
        }
        upgradeTypes = newArray;
    }
}
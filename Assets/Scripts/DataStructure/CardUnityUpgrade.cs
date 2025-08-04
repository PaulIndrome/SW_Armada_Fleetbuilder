using UnityEngine;
[System.Serializable]
public class CardUnityUpgrade : CardUnityBase {
    // public UpgradeType upgradeType {
    //     get { return upgradeTypes[0]; }
    // }
    public bool requiresFlagship = false;
    public UpgradeType[] upgradeTypes;
    public UpgradeCompatibility upgradeIncompatibility;

    public override void SetupCard(Card card, CardTypesLookup cardTypesLookup){
        base.SetupCard(card, cardTypesLookup);
        CardTypesLookupSlotUpgrade cardTypesLookupSlotUpgrade = cardTypesLookup.upgrades.Find(ut => ut.cardTypeRaw == card.cardType);
        if(cardTypesLookupSlotUpgrade == null) {
            Debug.LogError($"No internal upgrade type found for raw type \"{card.cardType}\". Upgrade card not set up completely.");
            return;
        }
        if(upgradeTypes == null){
            upgradeTypes = new UpgradeType[1];
        }
        upgradeTypes[0] = cardTypesLookupSlotUpgrade.upgradeType;
    }

    public bool HasUpgradeType(UpgradeType type){
        for(int i = 0; i < upgradeTypes.Length; i++){
            if(upgradeTypes[i] == type) return true;
        }
        return false;
    }

    // [ContextMenu("Copy single upgradeType to array")]
    // public void UpgradeTypeToArray(){
    //     if(upgradeTypes == null){
    //         upgradeTypes = new UpgradeType[1];
    //     }
    //     upgradeTypes[0] = upgradeType;
    // }

    // [ContextMenu("Copy compatibleShipSizes to Incompatibility")]
    // public void CopyCompatibleShipsizesToIncompatibility(){
    //     upgradeIncompatibility.compatibleShipsizes = compatibleShipSizes;
    //     upgradeIncompatibility.incompatibleUpgradeTypes = new UpgradeType[]{preventedByUpgradeType};
    // }
}
using UnityEngine;
[System.Serializable]
public class CardUnityUpgrade : CardUnityBase {
    public UpgradeType upgradeType {
        get { return upgradeTypes[0]; }
    }
    public UpgradeType[] upgradeTypes;

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

    [ContextMenu("Copy single upgradeType to array")]
    public void UpgradeTypeToArray(){
        if(upgradeTypes == null){
            upgradeTypes = new UpgradeType[1];
        }
        upgradeTypes[0] = upgradeType;
    }
}
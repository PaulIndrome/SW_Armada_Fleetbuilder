using UnityEngine;
[System.Serializable]
public class CardUnityUpgrade : CardUnityBase {
    public UpgradeType upgradeType;

    public override void SetupCard(Card card, CardTypesLookup cardTypesLookup){
        base.SetupCard(card, cardTypesLookup);
        CardTypesLookupSlotUpgrade cardTypesLookupSlotUpgrade = cardTypesLookup.upgrades.Find(ut => ut.cardTypeRaw == card.cardType);
        if(cardTypesLookupSlotUpgrade == null) {
            Debug.LogError($"No internal upgrade type found for raw type \"{card.cardType}\". Upgrade card not set up completely.");
            return;
        }
        upgradeType = cardTypesLookupSlotUpgrade.upgradeType;
    }
}
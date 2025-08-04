using UnityEngine;

[System.Serializable]
public class UpgradeSlot {

    public bool Filled {
        get { return slottedUpgrade != null && slottedUpgrade.Card != null; }
    }

    [SerializeField] private DeckEntryUpgrade slottedUpgrade;
    public DeckEntryUpgrade SlottedUpgrade => slottedUpgrade;

    [SerializeField] private UpgradeType upgradeSlotType;
    public UpgradeType UpgradeSlotType => upgradeSlotType;

    public UpgradeSlot(UpgradeType type){
        upgradeSlotType = type;
    }

    public bool SlotCard(DeckEntryUpgrade entry, int typeIndex = 0, bool overwrite = false){
        if((entry.Card as CardUnityUpgrade).upgradeTypes[typeIndex] != upgradeSlotType || (Filled && !overwrite)){
            return false;
        } else {    
            slottedUpgrade  = entry;
            return true;
        }
    }

    public bool UnslotCard(){
        if(Filled){
            slottedUpgrade = null;
            return true;
        } else {
            return false;
        }
    }

}
using UnityEngine;

[System.Serializable]
public class UpgradeSlot {

    public bool Filled {
        get { return slottedUpgrade != null; }
    }

    [SerializeField] private DeckEntryUpgrade slottedUpgrade;
    public DeckEntryUpgrade SlottedUpgrade => slottedUpgrade;

    [SerializeField] private UpgradeType upgradeType;
    public UpgradeType UpgradeType => upgradeType;

    public UpgradeSlot(UpgradeType type){
        upgradeType = type;
    }

    public bool SlotCard(DeckEntryUpgrade entry){
        if((entry.Card as CardUnityUpgrade).upgradeType != upgradeType || Filled){
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
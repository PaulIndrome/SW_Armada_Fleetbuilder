using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSlotButtonsControl : MonoBehaviour
{

    public delegate void UpgradeSetModeDelegate(UpgradeType type, int slotIndex, bool onOff);
    public static event UpgradeSetModeDelegate OnUpgradeSetMode;
    
    [Header("Asset references")]
    [SerializeField] private UpgradeSlotButton upgradeSlotButtonPrefab;
    [SerializeField] private List<UpgradeTypeIcon> upgradeTypeIcons;
    private Dictionary<UpgradeType, UpgradeSlotButton> upgradeSlotButtonDictionary;
    private Dictionary<UpgradeType, Sprite> upgradeIcons;

    [Header("Set via script")]
    public static bool UPGRADE_SET_MODE = false;
    [ReadOnly, SerializeField] private int activeSlotIndex = -1;
    [ReadOnly, SerializeField] private UpgradeType activeUpgradeType = UpgradeType.None;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        upgradeSlotButtonDictionary = new Dictionary<UpgradeType, UpgradeSlotButton>();
        upgradeIcons = new Dictionary<UpgradeType, Sprite>();
        foreach(UpgradeTypeIcon uti in upgradeTypeIcons){
            upgradeIcons.Add(uti.upgradeType, uti.icon);
        }
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        CardSelection.OnDeckEntrySelectionChange += SetupUpgradeSlotButtons;
        CardSelection.OnUpgradeSlotted += DeactivateUpgradeSetMode;
    }

    // [ContextMenu("Populate sprite dictionary")]
    // public void PopulateSpriteDictionary(){
    //     upgradeTypeIcons = new List<UpgradeTypeIcon>();
    //     System.Array allUpgradeTypes = System.Enum.GetValues(typeof(UpgradeType));
    //     for(int i = 0; i < allUpgradeTypes.Length; i++){
    //         upgradeTypeIcons.Add(new UpgradeTypeIcon(){
    //             upgradeType = (UpgradeType) allUpgradeTypes.GetValue(i),
    //             icon = null
    //         });
    //     }
    // }

    void SetupUpgradeSlotButtons(DeckEntry entry){
        foreach(UpgradeSlotButton usb in upgradeSlotButtonDictionary.Values){
            usb.gameObject.SetActive(false);
            usb.UnlinkButton();
        }

        if(entry == null || entry.Card is CardUnitySquadron) return;

        if(entry is DeckEntryShip){
            UpgradeSlotButton currentButton;
            DeckEntryShip shipEntry = entry as DeckEntryShip;
            for(int i = 0; i < shipEntry.UpgradeSlots.Count; i++){
                UpgradeType slotType = shipEntry.UpgradeSlots[i].UpgradeSlotType;
                if(upgradeSlotButtonDictionary.TryGetValue(slotType, out currentButton)){
                    currentButton.gameObject.SetActive(true);
                } else {
                    currentButton = Instantiate<UpgradeSlotButton>(upgradeSlotButtonPrefab, Vector3.zero, Quaternion.identity, transform);
                    currentButton.gameObject.name = $"{slotType}_slotButton";
                    currentButton.SetIcon(upgradeIcons[slotType]);
                    upgradeSlotButtonDictionary.Add(slotType, currentButton);
                }
                currentButton.LinkButtonToUpgradeSlot(shipEntry, i);

                currentButton.Button.onClick.RemoveAllListeners();
                // Debug.Log(currentButton.SlotIndexOnShip);
                int index = i; // need pass by value
                currentButton.Button.onClick.AddListener(() => ActivateUpgradeSetMode(slotType, index));
                // Debug.Log($"Set up {currentButton.name} with type {slotType} and index {i} ({currentButton.SlotIndexOnShip})");
            }
            currentButton = null;
        } else if(entry is DeckEntryUpgrade){
            // TODO
        }
    }

    public void ActivateUpgradeSetMode(UpgradeType type, int slotIndex){
        // Debug.Log("Activating upgrade set for " + type + " (" + slotIndex + ")");
        if(slotIndex == activeSlotIndex){
            DeactivateUpgradeSetMode(null);
            return;
        } else {
            activeSlotIndex = slotIndex;
            UPGRADE_SET_MODE = true;
            activeUpgradeType = type;
            upgradeSlotButtonDictionary[activeUpgradeType].SetState(UpgradeSlotState.Assigning);
        }

        if(OnUpgradeSetMode != null){
            OnUpgradeSetMode(type, slotIndex, UPGRADE_SET_MODE);
        }
    }

    public void DeactivateUpgradeSetMode(DeckEntry entry){
        upgradeSlotButtonDictionary[activeUpgradeType].SetState(upgradeSlotButtonDictionary[activeUpgradeType].ControlledUpgradeSlot.Filled ? UpgradeSlotState.Assigned : UpgradeSlotState.Unassigned);

        activeSlotIndex = -1;
        UPGRADE_SET_MODE = false;
        activeUpgradeType = UpgradeType.None;
        
        if(OnUpgradeSetMode != null){
            OnUpgradeSetMode(UpgradeType.None, -1, UPGRADE_SET_MODE);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardSelection.OnDeckEntrySelectionChange -= SetupUpgradeSlotButtons;
        CardSelection.OnUpgradeSlotted -= DeactivateUpgradeSetMode;
    }

}

[System.Serializable]
public struct UpgradeTypeIcon {
    public UpgradeType upgradeType;
    public Sprite icon;
}
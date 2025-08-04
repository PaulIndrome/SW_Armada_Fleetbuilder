using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;

[RequireComponent(typeof(Button))]
public class UpgradeSlotButton : MonoBehaviour
{

    public UpgradeSlot ControlledUpgradeSlot {
        get {
            if(slotIndexOnShip < 0 || currentDeckEntryShip == null) return null;
            return currentDeckEntryShip.UpgradeSlots[slotIndexOnShip];
        }
    }

    [Header("Settings")]
    [SerializeField] private Color unassignedColor;
    [SerializeField] private Color assigningColor;
    [SerializeField] private Color assignedColor;


    [Header("Scene references")]
    [SerializeField] private Button button;
    public Button Button => button;
    [SerializeField] private CanvasGroup filledIconCanvasGroup;
    [SerializeField] private Image slotTypeIconImage;

    [Header("Set via script")]
    [SerializeField] private int slotIndexOnShip;
    public int SlotIndexOnShip => slotIndexOnShip;
    [ReadOnly, SerializeField] private UpgradeSlotState currentState;
    [SerializeField] private DeckEntryShip currentDeckEntryShip;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        button = GetComponent<Button>();
    }

    public void SetIcon(Sprite icon){
        button.image.sprite = icon;
    }

    public void SetState(UpgradeSlotState state){
        if(state == currentState) return;
        currentState = state;
        switch(currentState){
            case UpgradeSlotState.Unassigned:
                slotTypeIconImage.color = unassignedColor;
                filledIconCanvasGroup.alpha = 0f;
                break;
            case UpgradeSlotState.Assigning:
                slotTypeIconImage.color = assigningColor;
                filledIconCanvasGroup.alpha = 0f;
                break;
            case UpgradeSlotState.Assigned:
                slotTypeIconImage.color = assignedColor;
                filledIconCanvasGroup.alpha = 1f;
                break;
        }
    }

    public void LinkButtonToUpgradeSlot(DeckEntryShip deckEntryShip, int slotIndex){
        currentDeckEntryShip = deckEntryShip;
        slotIndexOnShip = slotIndex;
        SetState(ControlledUpgradeSlot.Filled ? UpgradeSlotState.Assigned : UpgradeSlotState.Unassigned);
    }

    // public void ToggleFilledIcon(bool filled){
    //     filledIconCanvasGroup.alpha = filled ? 1 : 0;
    //     slotTypeIconImage.color = filled ? assignedColor : unassignedColor;
    // }

    public void UnlinkButton(){
        currentDeckEntryShip = null;
        slotIndexOnShip = -1;
        SetState(UpgradeSlotState.Unassigned);
    }

    public void DebugControlledUpgradeSlot(){
        Debug.Log($"Controlling {slotIndexOnShip}: {ControlledUpgradeSlot.UpgradeSlotType} on {currentDeckEntryShip.Identifier}");
    }

}

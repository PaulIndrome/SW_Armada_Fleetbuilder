using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckMessageControl : MonoBehaviour
{
    [Header("Asset references")]
    [SerializeField] private DeckMessage deckMessagePrefab;

    [Header("Scene references")]
    [SerializeField] private DeckMessage maxPointsExceeded;
    [SerializeField] private DeckMessage thirdSquadronPointsExceeded;
    [SerializeField] private DeckMessage collectionAmountZero;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        ToggleAllMessages(false);
    }

    public void ToggleMaxPointsExceeded(bool onOff){
        maxPointsExceeded.ToggleMessage(onOff);
    }
    public void ToggleThirdSquadronPointsExceeded(bool onOff, int currentSquadronPoints = 0, int maxSquadronPoints = 0){
        thirdSquadronPointsExceeded.ToggleMessage(onOff);
        thirdSquadronPointsExceeded.SetText($"squadron exceeds one third of max points ({currentSquadronPoints} / {maxSquadronPoints})");
    }
    public void ToggleCollectionAmountZero(bool onOff, bool isUnique = false){
        collectionAmountZero.ToggleMessage(onOff);
        collectionAmountZero.SetText(onOff && isUnique ? $"this item is unique per deck" : "no item of this card remaining" );
    }

    public void ToggleAllMessages(bool onOff){
        ToggleMaxPointsExceeded(onOff);
        ToggleThirdSquadronPointsExceeded(onOff);
        ToggleCollectionAmountZero(onOff);
    }

}

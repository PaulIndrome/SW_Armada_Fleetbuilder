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

    public void ToggleMaxPointsExceeded(bool onOff){
        maxPointsExceeded.gameObject.SetActive(onOff);
    }
    public void ToggleThirdSquadronPointsExceeded(bool onOff, int currentSquadronPoints = 0, int maxSquadronPoints = 0){
        thirdSquadronPointsExceeded.gameObject.SetActive(onOff);
        thirdSquadronPointsExceeded.SetText($"squadron exceeds one third of max points \n({currentSquadronPoints} / {maxSquadronPoints})");
    }
    public void ToggleCollectionAmountZero(bool onOff, bool isUnique = false){
        collectionAmountZero.gameObject.SetActive(onOff);
        collectionAmountZero.SetText(onOff && isUnique ? $"this item is unique per deck" : "no item of this card remaining" );
    }

    public void ToggleAllMessages(bool onOff){
        ToggleMaxPointsExceeded(onOff);
        ToggleThirdSquadronPointsExceeded(onOff);
        ToggleCollectionAmountZero(onOff);
    }

}

using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class CardUnityBase : ScriptableObject, IComparable<CardUnityBase> {

    private delegate void CardMovedDelegate(string id, bool toDeck, int amountChangedBy);
    private event CardMovedDelegate OnCardMoved;

    private delegate void CardToggledDelegate(string id, bool onOff);
    private event CardToggledDelegate OnCardToggled;

    public string ID = "";
    public Sprite sprite;
    public string size = "";
    public Faction faction;
    public string cardName = "";
    public int cost = -1;
    // [HideInInspector] public List<CardUnityBase> contradictories = new List<CardUnityBase>();
    public bool isCommander = false;
    public bool isUnique = false;
    public bool isSquadron = false;
    public string cardTypeRaw = "";
    public CardSize cardSize;

    // private char[] IDEndTrim = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'K', 'L', 'M', 'N', 'O', 'P'};

    public virtual void SetupCard(Card card, CardTypesLookup cardTypesLookup){
        ID = "card" + card.name.Replace(" ", "").Replace("'", "").Replace("-", "");
        sprite = Resources.Load<Sprite>($"ScannedImages/{card.imageURL.Split('.')[0]}");
        size = card.size;

        if(card.faction.Length < 1){
            faction = Faction.Empire | Faction.Rebellion;
        } else if(card.faction == "emp"){
            faction = Faction.Empire;
        } else if(card.faction == "reb"){
            faction = Faction.Rebellion;
        } else {
            // No faction, error will be logged
            Debug.LogError($"Base card {card.name} ({card.ID}) defines an invalid faction: {card.faction}");
            faction = (Faction) ~0;
        }

        cardName = card.name;
        cost = card.cost;

        isCommander = card.isCommander;
        isUnique = card.isUnique;
        isSquadron = card.isSquadron;

        cardTypeRaw = card.cardType;

        cardSize = cardTypesLookup.GetCardSize(card.cardType);
    }

    public void MoveCard(string id, bool toDeck, int amount){
        // Debug.Log("Moving card " + ID + $" {(toDeck ? "to deck" : "from deck")}", this);
        if(OnCardMoved != null)
            OnCardMoved(id, toDeck, amount);
    }

    public void ToggleCardAvailability(string blockingName, bool onOff){
        // Debug.Log("Toggling card " + ID + $" {(onOff.HasValue ? "" + onOff : "reverse")}", this);
        if(OnCardToggled != null)
            OnCardToggled(blockingName, onOff);
    }

    public void LinkUICard(CardUI cardUI){
        OnCardMoved += cardUI.MoveCard;
        OnCardToggled += cardUI.ToggleCardAvailability;
    }

    public void UnlinkUICard(CardUI cardUI){
        OnCardMoved -= cardUI.MoveCard;
        OnCardToggled -= cardUI.ToggleCardAvailability;
        
    }

    public void ResetLinkedUICards(){
        OnCardMoved = null;
        OnCardToggled = null;
    }

    // public void LinkContradictories(params CardUnityBase[] contradictoryCards){
    //     for(int i = 0; i < contradictoryCards.Length; i++){
    //         if(contradictoryCards[i] == null || contradictories.Contains(contradictoryCards[i])) continue;
    //         else {
    //             contradictories.Add(contradictoryCards[i]);
    //         }
    //     }
    // }

    // public void RemoveContradictories(params CardUnityBase[] contradictoryCards) {
    //     for(int i = 0; i < contradictoryCards.Length; i++){
    //         if(contradictoryCards[i] == null || !contradictories.Contains(contradictoryCards[i])) continue;
    //         else {
    //             contradictories.Remove(contradictoryCards[i]);
    //         }
    //     }
    // }

    public int CompareTo(CardUnityBase other)
    {
        return cardName.CompareTo(other.cardName);
    }

    [ContextMenu("Debug Log Faction Enum")]
    void DebugLogFactionEnum(){
        Debug.Log($"{cardName} faction: {faction.ToString()} ({(int) faction})");
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        ResetLinkedUICards();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(CardUnityBase), true), CanEditMultipleObjects]
public class CardUnityBaseEditor : Editor {
    public override void OnInspectorGUI(){
        CardUnityBase target = serializedObject.targetObject as CardUnityBase;
        DrawDefaultInspector();
        Rect textureRect = EditorGUILayout.GetControlRect(false, target.sprite.texture.height + 20f);
        textureRect.y += 16f;
        textureRect.x = 16f;
        EditorGUI.DrawPreviewTexture(textureRect, target.sprite.texture, null, ScaleMode.ScaleToFit);
    }
}
#endif
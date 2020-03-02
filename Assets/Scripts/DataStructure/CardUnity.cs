using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CardUnity : ScriptableObject
{
    public string ID = "";
    public Sprite sprite;
    public string size = "";
    public Faction faction;
    public string cardName = "";
    public int cost = -1;
    public List<CardUnity> contradictories = new List<CardUnity>();
    public bool isCommander = false;
    public bool isUnique = false;
    public bool isSquadron = false;
    public string cardTypeRaw = "";
    public CardSize cardSize;
    public UpgradeType[] upgradeSlots;

    public void SetupCard(Card card, CardSize cardSize){
        ID = card.ID;
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
    }

    public void LinkContradictories(params CardUnity[] contradictoryCards){
        for(int i = 0; i < contradictoryCards.Length; i++){
            if(contradictoryCards[i] == null || contradictories.Contains(contradictoryCards[i])) continue;
            else {
                contradictories.Add(contradictoryCards[i]);
            }
        }
    }

    public void RemoveContradictories(params CardUnity[] contradictoryCards) {
        for(int i = 0; i < contradictoryCards.Length; i++){
            if(contradictoryCards[i] == null || !contradictories.Contains(contradictoryCards[i])) continue;
            else {
                contradictories.Remove(contradictoryCards[i]);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CardUnity)), CanEditMultipleObjects]
public class CardUnityEditor : Editor {
    public override void OnInspectorGUI(){
        CardUnity target = serializedObject.targetObject as CardUnity;
        DrawDefaultInspector();
        Rect textureRect = EditorGUILayout.GetControlRect(false, target.sprite.texture.height);
        textureRect.x = 0f;
        EditorGUI.DrawPreviewTexture(textureRect, target.sprite.texture, null, ScaleMode.ScaleToFit);
    }
}
#endif
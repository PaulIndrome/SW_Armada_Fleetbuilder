using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class DeserializeCardsFromJSON : MonoBehaviour
{
    [System.Serializable]
    public class Cards {
        public Card[] allCards = new Card[0];
    }

    // [Tooltip("Set to smaller than 0 to create all cards")]
    // public int amountOfCardsToCreate = -1;

    public bool createUniqueCardCollection = true;

    public SquadronType squadronType;
    public ShipType shipType;
    public UpgradeType upgrade;
    
    public TextAsset cardsJson;
    public TextAsset cardTypesJson;
    public List<Card> allCards;
    public List<CardUnityBase> allCardsUnity;
    public CardTypesLookup cardTypesLookup;

    // private char[] IDEndTrim = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'K', 'L', 'M', 'N', 'O', 'P'};

    public void Start(){

    }

#if UNITY_EDITOR
    [ContextMenu("Build Collection")]
    public void BuildCollection(){
        Debug.Log("Building Collection");
        // cards = JsonConvert.DeserializeObject(cardsJson.text, typeof(Cards)) as Cards;
        allCards.Clear();
        allCardsUnity.Clear();

        allCards = new List<Card>((JsonConvert.DeserializeObject(cardsJson.text, typeof(Cards)) as Cards).allCards);
        
        // remove duplicates of identically named cards
        List<Card> allCardsUnique = createUniqueCardCollection ? new List<Card>() : allCards;

        if(createUniqueCardCollection){
            List<string> names = new List<string>();
            for(int i = 0; i < allCards.Count; i++){
                if(names.Contains(allCards[i].name)) {
                    continue;
                } else {
                    allCardsUnique.Add(allCards[i]);
                    names.Add(allCards[i].name);
                }
            }
        }

        bool cancelled = EditorUtility.DisplayCancelableProgressBar("Generating card collection", "Creating card assets", 0f);

        allCardsUnity = new List<CardUnityBase>();
        for(int i = 0; i < allCardsUnique.Count; i++){
            bool existing = false;

            cancelled = EditorUtility.DisplayCancelableProgressBar("Generating card collection", "Creating card assets", (float) i / allCardsUnique.Count);
            if(cancelled) {
                Debug.LogWarning($"Creation of card assets cancelled by user after {i} cards. Contradictories will not be linked.");
                EditorUtility.ClearProgressBar();
                return;
            }
            
            Type cardTypeInternal = cardTypesLookup.GetCardLookupTypeFromRaw(allCardsUnique[i].cardType);
            CardUnityBase newCard;

            if(cardTypeInternal == typeof(ShipType)){
                newCard = Resources.Load<CardUnityShip>("CardUnity/" + allCardsUnique[i].ID);
                if(newCard != null){
                    existing = true;
                } else {
                    newCard = ScriptableObject.CreateInstance<CardUnityShip>();
                }
            } else if (cardTypeInternal == typeof(SquadronType)){
                newCard = Resources.Load<CardUnitySquadron>("CardUnity/" + allCardsUnique[i].ID);
                if(newCard != null){
                    existing = true;
                } else {
                    newCard = ScriptableObject.CreateInstance<CardUnitySquadron>();
                }
            } else if (cardTypeInternal == typeof(UpgradeType)){
                newCard = Resources.Load<CardUnityUpgrade>("CardUnity/" + allCardsUnique[i].ID);
                if(newCard != null){
                    existing = true;
                } else {
                    newCard = ScriptableObject.CreateInstance<CardUnityUpgrade>();
                }
            } else {
                Debug.LogError($"No valid type found for card type \"{allCardsUnique[i].cardType}\" ({cardTypeInternal.ToString()}). Skipping card {allCardsUnique[i].ID}.");
                continue;
            }
           
            newCard.SetupCard(allCardsUnique[i], cardTypesLookup);
            newCard.name = newCard.ID;

            if(!existing){
                UnityEditor.AssetDatabase.CreateAsset(newCard, $"Assets/Resources/CardUnity/" + newCard.name + ".asset");
            } 
            
            // allCardsUnity.Add(Resources.Load<CardUnity>("CardData/" + newCard.name + ".asset"));
        }
        
        EditorUtility.ClearProgressBar();

        allCardsUnity = new List<CardUnityBase>(Resources.LoadAll<CardUnityBase>("CardUnity/"));
    }

    // [ContextMenu("Link Contradictories")]
    // public void LinkContradictories(){
    //     if(allCardsUnity.Count < 1) return; 

    //     CardUnityBase[] contradictoryCards;
    //     Card jsonCard;

    //     bool cancelled = EditorUtility.DisplayCancelableProgressBar("Linking contradictories", "Linking contradictories", 0f);

    //     for(int i = 0; i < allCardsUnity.Count; i++){
    //         jsonCard = allCards.Find(jc => jc.ID == allCardsUnity[i].ID);

    //         cancelled = EditorUtility.DisplayCancelableProgressBar("Linking contradictories", "Linking contradictories", (float) i / allCardsUnity.Count);
    //         if(cancelled){
    //             Debug.LogWarning($"Linking of contradictories cancelled by user after {i} cards.");
    //             EditorUtility.ClearProgressBar();
    //             return;
    //         }

    //         contradictoryCards = new CardUnityBase[allCards.Find(c => "card" + c.name.Replace(" ", "") == allCardsUnity[i].ID).contradictory.Length];

    //         if(contradictoryCards.Length < 1) continue;

    //         for(int c = 0; c < contradictoryCards.Length; c++){
    //             // contradictoryCards[c] = allCardsUnity.Find(cU => cU.name == allCards[i].contradictory[c]);
    //             Debug.Log($"Trying to load contradictory card for \"{allCardsUnity[i].ID}\""); // by ID \"{jsonCard.contradictory[c].TrimEnd(IDEndTrim)}\"");
    //             contradictoryCards[c] = Resources.Load<CardUnityBase>($"CardUnity/{jsonCard.contradictory[c].TrimEnd(IDEndTrim)}");
    //             if(contradictoryCards[c] == null) {
    //                 Debug.LogError($"{jsonCard.contradictory[c].TrimEnd(IDEndTrim)} not found for {allCardsUnity[i].ID}. Cancelling.", allCardsUnity[i]);
    //                 EditorUtility.ClearProgressBar();
    //                 return;
    //             }
    //         }
    //         allCardsUnity[i].LinkContradictories(contradictoryCards);

    //         EditorUtility.SetDirty(allCardsUnity[i]);
    //     }
    //     EditorUtility.ClearProgressBar();
    //     AssetDatabase.SaveAssets();
    // }

    [ContextMenu("Create Collection from current JSON")]
    public void CreateCollection(){
        CardCollection collection = ScriptableObject.CreateInstance<CardCollection>();
        collection.name = cardsJson.name;

        bool cancelled = EditorUtility.DisplayCancelableProgressBar("Creating collection", "Creating new collection from current JSON file", 0f);

        for(int i = 0; i < allCards.Count; i++){
            cancelled = EditorUtility.DisplayCancelableProgressBar("Creating collection", $"Adding {allCards[i].name}", (float) i / allCards.Count);
            if(cancelled){
                Debug.LogWarning($"Creation of collection cancelled by user after {i} of {allCards.Count} cards. Asset will not be created.");
                EditorUtility.ClearProgressBar();
                return;
            }
            // Debug.Log($"Trying to add card {allCards[i].name} to collection");

            collection.AddCard(allCardsUnity.Find(cu => cu.cardName == allCards[i].name));
        }
        
        EditorUtility.ClearProgressBar();

        UnityEditor.AssetDatabase.CreateAsset(collection, $"Assets/Resources/Collections/{collection.name}_collection.asset");
    }

    // [ContextMenu("Debug all Card Types")]
    // public void DebugLogAllCardTypes(){
    //     List<string> cardTypes = new List<string>();
    //     int i = 0;
    //     foreach(CardUnity cU in allCardsUnity){
    //         if(cardTypes.Contains(cU.cardType)) continue;
    //         else {
    //             cardTypes.Add(cU.cardType);
    //         }
    //     }
    //     cardTypes.Sort();
    //     foreach(string s in cardTypes){
    //         Debug.Log(i++.ToString("000")+ ": " + s);
    //     }
    // }

#endif

}

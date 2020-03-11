using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class DeserializeCardsFromJSON : MonoBehaviour
{

    public static string DECKFILEPATH;

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
    [ContextMenuItem("Sort all cards by name", "SortAllCardsByName")]
    public List<Card> allCards;
    public List<CardUnityBase> allCardsUnity;
    public CardTypesLookup cardTypesLookup;

    public Deck currentDeck;
    public SerializableDeck currentSerializableDeck;

    // private char[] IDEndTrim = new char[]{'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'K', 'L', 'M', 'N', 'O', 'P'};

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(DeserializeCardsFromJSON.DECKFILEPATH != null && DeserializeCardsFromJSON.DECKFILEPATH.Length < 1) DeserializeCardsFromJSON.DECKFILEPATH = $"{Application.persistentDataPath}/LocalDecks/";
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

    public void SortAllCardsByName(){
        allCards.Sort(delegate(Card c1, Card c2) { return c1.name.CompareTo(c2.name); });
    }

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

            collection.AddCard(allCardsUnity.Find(cu => cu.ID == allCards[i].name));
        }
        
        EditorUtility.ClearProgressBar();

        UnityEditor.AssetDatabase.CreateAsset(collection, $"Assets/Resources/Collections/{collection.name}_collection.asset");
    }

    [ContextMenu("Serialize deck")]
    public void SerializeDeck(){
        Deck deck = new Deck("", 400, (Faction) ~0);
        int i = 0;

        DeckEntry upgradeTestShip = null;

        while(deck.PointsCurrent < deck.PointsMax && i < 100){
            CardUnityBase cardToAdd = allCardsUnity[UnityEngine.Random.Range(0, allCardsUnity.Count)];
            Debug.Log($"Random card is \"{cardToAdd.cardName}\"", cardToAdd);
            if(cardToAdd is CardUnityShip && upgradeTestShip == null){
                deck.AddCardToDeck(cardToAdd/* , out upgradeTestShip */);
            } else if(cardToAdd is CardUnityUpgrade && upgradeTestShip != null){
                DeckEntry upgrade = deck.AddCardToDeck(cardToAdd/* , out upgrade */);
                ((DeckEntryShip) upgradeTestShip).SlotUpgrade(upgrade as DeckEntryUpgrade);
            } else {
                DeckEntry newEntry = deck.AddCardToDeck(cardToAdd/* , out newEntry */);
            }

            i = i + 1;
        }

        DeserializeCardsFromJSON.SerializeDeck(deck);
        
        AssetDatabase.Refresh();
    }

    [ContextMenu("Deserialize deck")]
    public void DeserializeDeck(){
        currentSerializableDeck = JsonConvert.DeserializeObject<SerializableDeck>(AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/_testDeck.json").text);
        
        currentDeck = new Deck(currentSerializableDeck);
    }

#endif

    public static SerializedDeckInfo SerializeDeck(Deck deck, string fileName = ""){
        SerializableDeck sDeck = new SerializableDeck();
        sDeck.SetUpSerializableDeck(deck);
        sDeck.deckName = fileName.Length < 1 ? deck.DeckName : fileName;

        string serializedString = JsonConvert.SerializeObject(sDeck, Formatting.Indented);
        
        TextAsset textAsset = new TextAsset(serializedString);
        // AssetDatabase.CreateAsset(textAsset, $"Assets/{deck.DeckName}.json");
        if(!System.IO.Directory.Exists($"{Application.persistentDataPath}/LocalDecks")){
            System.IO.Directory.CreateDirectory($"{Application.persistentDataPath}/LocalDecks/");
        }

        string path = $"{Application.persistentDataPath}/LocalDecks/{(fileName.Length < 1 ? deck.DeckName : fileName)}.json";
        
        // File.WriteAllText(path, serializedString);

        // Debug.LogWarning(path);

        return new SerializedDeckInfo(){
            jsonString = serializedString,
            fullPath = path
        };
    }

    public static Deck DeserializeDeck(string deckName){
        string path = $"{Application.persistentDataPath}/LocalDecks/{deckName}.json";
        if(!File.Exists(path)){
            return null;
        } else {
            SerializableDeck sDeck = JsonConvert.DeserializeObject<SerializableDeck>(File.ReadAllText(path));
            return DeserializeCardsFromJSON.DeserializeDeck(sDeck);
        }
    }

    public static Deck DeserializeDeck(SerializableDeck sDeck){
        return new Deck(sDeck);
    }
}

[Serializable]
public struct SerializedDeckInfo {
    public string fullPath;
    public string jsonString;
}

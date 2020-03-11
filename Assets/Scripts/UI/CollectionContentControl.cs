using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CollectionContentControl : MonoBehaviour
{

    [System.Serializable]
    public struct DefaultColumnCount {
        public CardSize categorySize;
        [Range(1, 5)]
        public int defaultColumnCount;
    }

    private List<CardUnityBase> unityCards;
    private List<CardCollection> allCardCollections;
    
    [Header("Prefab references")]
    [SerializeField] private CardCategory cardCategoryPrefab;
    [SerializeField] private CategoryHeader categoryHeaderPrefab;


    [Header("Set via inspector")]
    [SerializeField] public List<DefaultColumnCount> defaultColumnCounts = new List<DefaultColumnCount>();

    [Header("Scene references")]
    [SerializeField] private CardSelectionSlot cardSelectionSlot;
    [SerializeField] private ScrollRect scrollRect;
    private RectTransform scrollTransform;
    [SerializeField] private RectTransform headerGroupTransform, shipContentTransform, squadronContentTransform, upgradeContentTransform;
    [Space, SerializeField] private List<CardCategory> cardCategories;

    [Header("Set up via script")]
    [SerializeField, ContextMenuItem("Load collection data", "LoadCollectionData")] 
    private CardCollection currentCollection;

    [ContextMenuItem("Cycle current faction", "CycleCurrentFaction")]
    public Faction currentFaction = (Faction) ~0;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(scrollRect == null){
            scrollRect = GetComponentInChildren<ScrollRect>();
        }
        if(scrollTransform == null){
            scrollTransform = scrollRect.GetComponent<RectTransform>();
        }

        LoadResources();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // CardCategory.OnCategoryColumnChanged += CreateCategoryMarkers;
        DeckContentControl.OnNewDeckStarted += ResetCardAmountsOfCurrentSelection;
        DeckContentControl.OnDeckLoaded += UpdateCollectionToDeck;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(currentCollection != null){
            Debug.Log("About to set collection " + currentCollection.name);
            SetCurrentCollection(currentCollection);
        }
    }

    [ContextMenu("Load Resources")]
    void LoadResources(){
        unityCards = new List<CardUnityBase>(Resources.LoadAll<CardUnityBase>("CardUnity/"));
        allCardCollections = new List<CardCollection>(Resources.LoadAll<CardCollection>("Collections/")); // needs to be replaced with a JSON approach later
        cardCategories.Clear();
    }

    [ContextMenu("Spawn all categories")]
    void SpawnAllCategories(){
        if(unityCards.Count < 1){
            LoadResources();
        }
        SpawnShipCategory();
        SpawnSquadronCategory();
        SpawnUpgradeCategory();
    }

    [ContextMenu("Spawn ship category")]
    void SpawnShipCategory(){
        SpawnCategory(CardSize.Large, defaultColumnCounts.Find(dcc => dcc.categorySize == CardSize.Large).defaultColumnCount);
    }

    [ContextMenu("Spawn squadron category")]
    void SpawnSquadronCategory(){
        SpawnCategory(CardSize.Normal, defaultColumnCounts.Find(dcc => dcc.categorySize == CardSize.Normal).defaultColumnCount);
    }

    [ContextMenu("Spawn upgrade category")]
    void SpawnUpgradeCategory(){
        SpawnCategory(CardSize.Small, defaultColumnCounts.Find(dcc => dcc.categorySize == CardSize.Small).defaultColumnCount);
    }

    public void SpawnCategory(CardSize cardSize, int defaultColumnCount = 0){
        CardCategory cardCategory = cardCategories.Find(cc => cc.categoryCardSize == cardSize);
        RectTransform contentTransform = null;
        switch(cardSize){
            case CardSize.Large:
                contentTransform = shipContentTransform;
                break;
            case CardSize.Normal:
                contentTransform = squadronContentTransform;
                break;
            case CardSize.Small:
                contentTransform = upgradeContentTransform;
                break;
        }
        if(cardCategory == null){
            cardCategory = Instantiate<CardCategory>(cardCategoryPrefab, Vector3.zero, Quaternion.identity, contentTransform.transform);
            cardCategories.Add(cardCategory);
        }

        // CategoryHeader header = Instantiate<CategoryHeader>(categoryHeaderPrefab, Vector3.zero, Quaternion.identity, headerGroupTransform);

        cardCategory.SetupCardCategory(cardSize, unityCards.FindAll(uC => uC.cardSize == cardSize), null, defaultColumnCount);

        // if(cardSize != CardSize.Large){
        //     cardCategory.transform.parent.gameObject.SetActive(false);
        // }
    }

    [ContextMenu("Cycle current faction")]
    public void CycleCurrentFaction(){
        switch((int) currentFaction){
            case 0:
                currentFaction = (Faction) ~0;
                break;
            case ~0:
                currentFaction = Faction.Empire;
                break;
            case (int) Faction.Empire:
                currentFaction = Faction.Rebellion;
                break;
            case (int) Faction.Rebellion:
                currentFaction = (Faction) 0;
                break;
        }
        foreach(CardCategory cC in cardCategories){
            cC.SetFactionTo(currentFaction);
        }
    }

    public void ResetCardAmountsOfCurrentSelection(Deck deck){
        currentCollection.ResetCardAmounts();
        ResetCollectionAmounts(deck.DeckFaction);
    }

    public void SetCurrentCollection(CardCollection newCurrent){
        Debug.Log("SetCurrentCollection", newCurrent);
        if(currentCollection != null){
            DeckContentControl.OnAddedToDeck -= currentCollection.PickFromCollection;
            DeckContentControl.OnRemovedFromDeck -= currentCollection.ReturnToCollection;
        }
        
        currentCollection = newCurrent;

        DeckContentControl.OnAddedToDeck += currentCollection.PickFromCollection;
        DeckContentControl.OnRemovedFromDeck += currentCollection.ReturnToCollection;   

        currentCollection.ResetCardAmounts();

        LoadCollectionData();
    }

    void UpdateCollectionToDeck(Deck deck){
        currentCollection.ResetCardAmounts();
        ResetCollectionAmounts(deck.DeckFaction);

        CardCategory cc = cardCategories.Find(caca => caca.categoryCardSize == CardSize.Large);
        
        foreach(DeckEntryShip de in deck.shipCards){
            currentCollection.PickFromCollection(cc.UiCardsInCategory.Find(uic => de.Card.ID == uic.Card.ID));
        }

        cc = cardCategories.Find(caca => caca.categoryCardSize == CardSize.Normal);
        
        foreach(DeckEntrySquadron de in deck.squadronCards){
            currentCollection.PickFromCollection(cc.UiCardsInCategory.Find(uic => de.Card.ID == uic.Card.ID));
        }

        cc = cardCategories.Find(caca => caca.categoryCardSize == CardSize.Small);
        
        foreach(DeckEntryUpgrade de in deck.upgradeCards){
            currentCollection.PickFromCollection(cc.UiCardsInCategory.Find(uic => de.Card.ID == uic.Card.ID));
        }
    }

    void ResetCollectionAmounts(Faction faction = 0){
        foreach(CardCategory cardCat in cardCategories){
            foreach(CardUI cui in cardCat.UiCardsInCategory){
                cui.ResetCardAccordingToCurrentCollection();
            }
            if(faction != 0){
                cardCat.SetFactionTo(faction);
            }
        }
    }

    void LoadCollectionData(){
        Debug.Log("LoadCollectionData", currentCollection);
        SpawnAllCategories();
        CardCollectionEntry entry;
        foreach(CardCategory cc in cardCategories){
            foreach(CardUI cui in cc.UiCardsInCategory){
                entry = currentCollection.FindAllOfCardSize(cc.categoryCardSize).Find(cce => cce.Identifier == cui.Card.ID);
                if(entry == null){
                    Debug.LogError($"Could not find entry for CardUI \"{cui.Card.ID}\" in category \"{cc.categoryCardSize}\"", cui);
                    continue;
                }
                cui.SetCollectionEntry(entry);
            }
        }

        // CreateCategoryMarkers();

        Canvas.ForceUpdateCanvases();
    }

    public void CenterToItem(CardCategory categoryToGoTo){
        Vector2 calculatedNormalizedPosition = Vector2.zero;
        RectTransform categoryHeaderTransform = categoryToGoTo.CategoryHeader.GetComponent<RectTransform>();
        calculatedNormalizedPosition.y = 1 - Mathf.Abs(categoryHeaderTransform.anchoredPosition.y / scrollRect.content.sizeDelta.y);
        scrollRect.normalizedPosition = calculatedNormalizedPosition;
    }

    // [ContextMenu("Create category markers")]
    // public void CreateCategoryMarkers(){
    //     CategoryMarkerPlacer placer = GetComponentInChildren<CategoryMarkerPlacer>();
    //     float[] places = new float[cardCategories.Count - 1];
    //     for(int i = 0; i < places.Length; i++){
    //         places[i] = 1 - Mathf.Abs(cardCategories[i + 1].CategoryHeader.GetComponent<RectTransform>().anchoredPosition.y  / scrollRect.content.sizeDelta.y);
    //     }
    //     placer.PlaceMarkers(places);
    // }

    [ContextMenu("Go to Ship category")]
    void GoToShipCategory(){
        GoToCategory(CardSize.Large);
    }
    [ContextMenu("Go to Squadron category")]
    void GoToSquadronCategory(){
        GoToCategory(CardSize.Normal);
    }
    [ContextMenu("Go to Upgrade category")]
    void GoToUpgradeCategory(){
        GoToCategory(CardSize.Small);
    }
    public void GoToCategory(CardSize cardSize){
        CenterToItem(cardCategories.Find(cC => cC.categoryCardSize == cardSize));
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        // CardCategory.OnCategoryColumnChanged -= CreateCategoryMarkers;
        DeckContentControl.OnNewDeckStarted -= ResetCardAmountsOfCurrentSelection;
        DeckContentControl.OnDeckLoaded -= UpdateCollectionToDeck;
    }
    
}

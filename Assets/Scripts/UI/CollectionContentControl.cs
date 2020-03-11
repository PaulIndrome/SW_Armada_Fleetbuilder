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

    
    [Header("Prefab references")]
    [SerializeField] private CardCategory cardCategoryPrefab;

    [Header("Settings")]
    [SerializeField] public List<DefaultColumnCount> defaultColumnCounts = new List<DefaultColumnCount>();

    [Header("Scene references")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform shipContentTransform, squadronContentTransform, upgradeContentTransform;

    [Header("Set via script")]
    [SerializeField, ContextMenuItem("Load collection data", "LoadCollectionData")] 
    private CardCollection currentCollection;
    [Space, SerializeField] private List<CardCategory> cardCategories;
    private List<CardUnityBase> unityCards;
    private List<CardCollection> allCardCollections;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(scrollRect == null){
            scrollRect = GetComponentInChildren<ScrollRect>();
        }
        LoadResources();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
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
        allCardCollections = new List<CardCollection>(Resources.LoadAll<CardCollection>("Collections/")); // TODO: needs to be replaced with a JSON approach later
        cardCategories.Clear();
    }

    void SpawnAllCategories(){
        if(unityCards.Count < 1){
            LoadResources();
        }
        SpawnShipCategory();
        SpawnSquadronCategory();
        SpawnUpgradeCategory();
    }

    void SpawnShipCategory(){
        SpawnCategory(CardSize.Large, defaultColumnCounts.Find(dcc => dcc.categorySize == CardSize.Large).defaultColumnCount);
    }

    void SpawnSquadronCategory(){
        SpawnCategory(CardSize.Normal, defaultColumnCounts.Find(dcc => dcc.categorySize == CardSize.Normal).defaultColumnCount);
    }

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

        cardCategory.SetupCardCategory(cardSize, unityCards.FindAll(uC => uC.cardSize == cardSize), null, defaultColumnCount);
    }

    public void ResetCardAmountsOfCurrentSelection(Deck deck){
        currentCollection.ResetCardAmounts();
        ResetCollectionAmounts(deck.DeckFaction);
    }

    public void SetCurrentCollection(CardCollection newCurrent){
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

        Canvas.ForceUpdateCanvases();
    }

    public void CenterToItem(CardUI cardToGoTo){
        Vector2 calculatedNormalizedPosition = Vector2.zero;
        RectTransform categoryHeaderTransform = cardToGoTo.GetComponent<RectTransform>();
        calculatedNormalizedPosition.y = 1 - Mathf.Abs(categoryHeaderTransform.anchoredPosition.y / scrollRect.content.sizeDelta.y);
        scrollRect.normalizedPosition = calculatedNormalizedPosition;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        DeckContentControl.OnNewDeckStarted -= ResetCardAmountsOfCurrentSelection;
        DeckContentControl.OnDeckLoaded -= UpdateCollectionToDeck;
    }
    
}

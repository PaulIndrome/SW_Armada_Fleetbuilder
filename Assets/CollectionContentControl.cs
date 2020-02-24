using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionContentControl : MonoBehaviour
{
    private List<CardUnityBase> unityCards;
    private List<CardCollection> allCardCollections;
    
    [Header("Prefab references")]
    [SerializeField] private CardCategory cardCategoryPrefab;

    [Header("Scene references")]
    [SerializeField] private ScrollRect scrollRect;
    private RectTransform scrollTransform;
    
    [SerializeField] private RectTransform contentTransform;
    [Space, SerializeField] private List<CardCategory> cardCategories;

    [Header("Set up via script")]
    [SerializeField] private CardCollection currentCollection;
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
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if(cardCategories.Count < 1){
            SpawnAllCategories();
        }
    }

    [ContextMenu("Load Resources")]
    void LoadResources(){
        unityCards = new List<CardUnityBase>(Resources.LoadAll<CardUnityBase>("CardUnity/"));
        allCardCollections = new List<CardCollection>(Resources.LoadAll<CardCollection>("Collections/")); // needs to be replaced with a JSON approach later
        cardCategories.Clear();
    }

    [ContextMenu("Spawn ship category")]
    void SpawnShipCategory(){
        SpawnCategory(CardSize.Large);
    }

    [ContextMenu("Spawn squadron category")]
    void SpawnSquadronCategory(){
        SpawnCategory(CardSize.Normal);
    }

    [ContextMenu("Spawn upgrade category")]
    void SpawnUpgradeCategory(){
        SpawnCategory(CardSize.Small);
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
            // Debug.Log($"About to set faction for {cC.categoryCardSize} to {currentFaction}");
            cC.SetFactionTo(currentFaction);
        }
    }

    

    public void SpawnCategory(CardSize cardSize){
        CardCategory cardCategoryToSpawn = Instantiate<CardCategory>(cardCategoryPrefab, Vector3.zero, Quaternion.identity, contentTransform);
        cardCategoryToSpawn.SetupCardCategory(cardSize, unityCards.FindAll(uC => uC.cardSize == cardSize));
        if(!cardCategories.Contains(cardCategoryToSpawn)){
            cardCategories.Add(cardCategoryToSpawn);
        }
    }

    public void LoadCollectionData(){
        foreach(CardCategory cc in cardCategories){

        }
    }

    public void CenterToItem(CardCategory categoryToGoTo){
        // float categoryHeaderSize = categoryToGoTo.CategoryHeader.HeaderSize;
        // Debug.Log("\n");
        // Debug.Log("categoryHeaderSize: " + categoryHeaderSize, categoryToGoTo.CategoryHeader);
        Vector2 calculatedNormalizedPosition = Vector2.zero;
        // RectTransform categoryTransform = categoryToGoTo.RectTransform;
        RectTransform categoryHeaderTransform = categoryToGoTo.CategoryHeader.GetComponent<RectTransform>();
        // RectTransform contentTransform = scrollRect.content;
        // Debug.Log("scrollRect.content.sizeDelta.y: " + scrollRect.content.sizeDelta.y, scrollRect.content);
        // Debug.Log("categoryTransform.anchoredPosition.y: " + categoryTransform.anchoredPosition.y, categoryTransform);
        // Debug.Log("categoryHeaderTransform.anchoredPosition.y: " + categoryHeaderTransform.anchoredPosition.y);
        calculatedNormalizedPosition.y = 1 - Mathf.Abs(categoryHeaderTransform.anchoredPosition.y / scrollRect.content.sizeDelta.y);
        // Debug.Log("calculatedNormalizedPosition.y: " + calculatedNormalizedPosition.y);
        scrollRect.normalizedPosition = calculatedNormalizedPosition;
    }

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
        // switch(cardSize){
        //     case CardSize.Large:
        //         break;
        //     case CardSize.Normal:
        //         break;
        //     case CardSize.Small:
        //         break;
        // }
    }
    
}

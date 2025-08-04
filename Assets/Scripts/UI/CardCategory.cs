using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class CardCategory : MonoBehaviour
{

    public delegate void CategoryColumnCountChangedDelegate();
    public static event CategoryColumnCountChangedDelegate OnCategoryColumnChanged;

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;


    [Header("Prefab references")]
    // public CategoryHeader categoryHeaderPrefab;
    public CardUI cardUIPrefab;

    // [Header("Scene references")]
    
    [Header("Set via script")]
    [ReadOnly, SerializeField] private bool[] previousActive;
    [ReadOnly, SerializeField] private int currentColumnCount = 3;
    [ReadOnly, SerializeField] private int previousColumnCount = 3;
    [ReadOnly, SerializeField] private List<CardUI> uiCardsInCategory;
    [ReadOnly, SerializeField] private CategoryHeader categoryHeader;
    [ReadOnly(true), Range(120f, 360f), SerializeField] private float cardWidth = 120f;
    [ReadOnly] public CardType categoryCardType = CardType.None;
    [ReadOnly, SerializeField] private List<CardUnityBase> cardsInCategory;

    public CategoryHeader CategoryHeader => categoryHeader;
    public List<CardUI> UiCardsInCategory => uiCardsInCategory;


    public RectTransform RectTransform {
        get {
            if(rectTransform == null){
                Awake();
            }
            return rectTransform;
        }
    }

    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if(categoryCardType == CardType.Upgrade){
            UpgradeSlotButtonsControl.OnUpgradeSetMode += ToggleSetUpgradeMode;
        }
    }

    public void SetupCardCategory(CardType cardType, List<CardUnityBase> cards, int defaulColumnCount = 0){
        string categoryName;

        switch (cardType){
            case CardType.Ship:
                // categoryHeader.SetHeaderText("Ships");
                categoryName = "Category-Ships";
                break;
            case CardType.Squadron:
                // categoryHeader.SetHeaderText("Squadrons");
                categoryName = "Category-Squadrons";
                break;
            case CardType.Upgrade:
                // categoryHeader.SetHeaderText("Upgrades");
                categoryName = "Category-Upgrades";
                UpgradeSlotButtonsControl.OnUpgradeSetMode += ToggleSetUpgradeMode;
                break;
            case CardType.Objective:
                // categoryHeader.SetHeaderText("Objectives");
                categoryName = "Category-Objectives";
                break;
            default:
                // categoryHeader.SetHeaderText("Miscellaneous");
                categoryName = "Category-Miscellaneous";
                break;
        }

        gameObject.name = $"{transform.GetSiblingIndex().ToString("000")}-{categoryName}";  


        cardsInCategory = cards;
        categoryCardType = cardType;

        SetupCardsInCategory();

        if(defaulColumnCount > 0){
            SetColumns(defaulColumnCount);
        }

        gameObject.layer = LayerMask.NameToLayer("UI");
    }

    public void ToggleCategory(bool onOff){
        categoryHeader.gameObject.SetActive(onOff);
        gameObject.SetActive(onOff);
    }

    [ContextMenu("Sort category by cost asc")]
    public void SortCategoryByCostAscending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.Card.cost.CompareTo(c2.Card.cost);});
        RearrangeCards();
    }

    [ContextMenu("Sort category by cost desc")]
    public void SortCategoryByCostDescending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.Card.cost.CompareTo(c2.Card.cost);});
        RearrangeCards(true);
    }

    [ContextMenu("Sort category by in-deck asc")]
    public void SortCategoryByInDeckAscending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.CurrentAmountInDeck.CompareTo(c2.CurrentAmountInDeck);});
        RearrangeCards();
    }

    [ContextMenu("Sort category by in-deck desc")]
    public void SortCategoryByInDeckDescending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.CurrentAmountInDeck.CompareTo(c2.CurrentAmountInDeck);});
        RearrangeCards(true);
    }

    public void ToggleSetUpgradeMode(UpgradeType type, int slotIndex, bool onOff){
        Debug.Log($"Category {gameObject.name} toggling set upgrade mode {(onOff ? "ON" : "OFF")}");
        if(onOff){
            // if(previousActive == null || previousActive.Length < 1){
            //     previousActive = new bool[uiCardsInCategory.Count];
            //     for(int i = 0; i < previousActive.Length; i++){
            //         previousActive[i] = uiCardsInCategory[i].gameObject.activeSelf;
            //     }
            // }
            SetUpgradeType(type);
            if(currentColumnCount > 3){
                SetColumns(3);
            }
        } else {
            // if(previousActive == null || previousActive.Length < 1) {
            //     SetFactionTo(CurrentDeck.Deck.DeckFaction);
            // } else {
            //     for(int i = 0; i < previousActive.Length; i++){
            //         uiCardsInCategory[i].gameObject.SetActive(previousActive[i]);
            //     }
            // }
            // previousActive = null;
            SetFactionTo(CurrentDeck.Deck.DeckFaction);
            SetColumns(previousColumnCount);
        }
    }

    void RearrangeCards(bool reverse = false){
        // Debug.Log("Rearranging");
        if(reverse){
            // Debug.Log("reverse");
            for(int i = 0; i < uiCardsInCategory.Count ; i++){
                // Debug.Log(uiCardsInCategory[i].transform.GetSiblingIndex());
                uiCardsInCategory[i].transform.SetAsFirstSibling();
                // Debug.Log(uiCardsInCategory[i].transform.GetSiblingIndex());
            }
        } else {
            // Debug.Log("non-reverse");
            for(int i = 0; i < uiCardsInCategory.Count ; i++){
                // Debug.Log(uiCardsInCategory[i].transform.GetSiblingIndex());
                uiCardsInCategory[i].transform.SetAsLastSibling();
                // Debug.Log(uiCardsInCategory[i].transform.GetSiblingIndex());
            }
        }
    }

    public void SetupCardsInCategory(){
        // Debug.Log($"About to set up cards in category {categoryCardSize}: {cardsInCategory.Count} ui cards will be set up");
        if(cardsInCategory.Count < 1) return;

        cardsInCategory.Sort(delegate(CardUnityBase c1, CardUnityBase c2) {return c1.cardTypeRaw.CompareTo(c2.cardTypeRaw);});

        Canvas.ForceUpdateCanvases();

        CardUI newCard;
        for(int i = 0; i < cardsInCategory.Count; i++){
            newCard = uiCardsInCategory.Find(nc => nc.Card.ID == cardsInCategory[i].ID);
            if(newCard == null){
                newCard = Instantiate<CardUI>(cardUIPrefab, Vector3.zero, Quaternion.identity, transform);
                uiCardsInCategory.Add(newCard);
            }
            newCard.SetupCardUI(cardsInCategory[i]);
        }
    }

    public void SetFactionTo(Faction activeFaction){
        foreach(CardUI cui in uiCardsInCategory){
            cui.ToggleByFaction(activeFaction);
        }
    }

    public void SetView(ViewType view){
        foreach(CardUI cui in uiCardsInCategory){
            cui.ToggleView(view);
        }
    }

    public void SetUpgradeType(UpgradeType type, bool keepFaction = true){
        foreach(CardUI cui in uiCardsInCategory){
            cui.ToggleForUpgradeSetMode(type);
        }
    }

    public void SetColumns(int numberOfColumns){
        previousColumnCount = currentColumnCount;

        float newWidth = (RectTransform.rect.width - (gridLayoutGroup.spacing.x * (numberOfColumns - 1)) - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / numberOfColumns;
        cardWidth = newWidth;
        gridLayoutGroup.cellSize = new Vector2(newWidth, newWidth * 1.5f);

        Canvas.ForceUpdateCanvases();

        for(int i = 0; i < uiCardsInCategory.Count; i++){
            uiCardsInCategory[i].PositionAmountGroup();
        }

        if(OnCategoryColumnChanged != null)
            OnCategoryColumnChanged();
        
        currentColumnCount = numberOfColumns;
    }

    public void SetColumns(float numberOfColumns){
        SetColumns((int) Mathf.Ceil(numberOfColumns));
    }

    [ContextMenu("Set 2 Columns")]
    public void Set2Columns(){
        SetColumns(2);
    }

    [ContextMenu("Set 3 Columns")]
    public void Set3Columns(){
        SetColumns(3);
    }
    [ContextMenu("Set 4 Columns")]
    public void Set4Columns(){
        SetColumns(4);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if(categoryCardType == CardType.Upgrade){
            UpgradeSlotButtonsControl.OnUpgradeSetMode -= ToggleSetUpgradeMode;
        }
    }
}

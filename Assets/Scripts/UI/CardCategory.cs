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
    [ReadOnly, SerializeField] private List<CardUI> uiCardsInCategory;
    [ReadOnly, SerializeField] private CategoryHeader categoryHeader;
    [ReadOnly(true), Range(120f, 360f), SerializeField] private float cardWidth = 120f;
    [ReadOnly] public CardSize categoryCardSize;
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

    public void SetupCardCategory(CardSize cardSize, List<CardUnityBase> cards, CategoryHeader header, int defaulColumnCount = 0){
        // transform.SetSiblingIndex((int) cardSize);


        if(header != null){
            categoryHeader = header;
            categoryHeader.ResetTransform();

            // categoryHeader = Instantiate<CategoryHeader>(categoryHeaderPrefab, Vector3.zero, Quaternion.identity, transform.parent);
            // categoryHeader.transform.SetSiblingIndex(transform.GetSiblingIndex());
            // categoryHeader.columnButtons[0].onClick.AddListener(() => SetColumns(2));
            // categoryHeader.columnButtons[1].onClick.AddListener(() => SetColumns(3));
            // categoryHeader.columnButtons[2].onClick.AddListener(() => SetColumns(4));
            // categoryHeader.columnButtons[3].onClick.AddListener(() => SetColumns(5));
            
            categoryHeader.ColumnSlider.SetValueWithoutNotify(defaulColumnCount);
            categoryHeader.ColumnSlider.onValueChanged.AddListener(SetColumns);

            string categoryName;

            switch (cardSize){
                case CardSize.Large:
                    categoryHeader.SetHeaderText("Ships");
                    categoryName = "Category-Ships";
                    break;
                case CardSize.Normal:
                    categoryHeader.SetHeaderText("Squadrons");
                    categoryName = "Category-Squadrons";
                    break;
                case CardSize.Small:
                    categoryHeader.SetHeaderText("Upgrades");
                    categoryName = "Category-Upgrades";
                    break;
                default:
                    categoryHeader.SetHeaderText("Miscellaneous");
                    categoryName = "Category-Miscellaneous";
                    break;
            }

            gameObject.name = $"{transform.GetSiblingIndex().ToString("000")}-{categoryName}";  
        }


        cardsInCategory = cards;
        categoryCardSize = cardSize;

        SetupCardsInCategory();

        if(defaulColumnCount > 0){
            SetColumns(defaulColumnCount);
        }

        gameObject.layer = LayerMask.NameToLayer("UI");

        // return categoryHeader;
    }

    public void ToggleCategory(bool onOff){
        categoryHeader.gameObject.SetActive(onOff);
        gameObject.SetActive(onOff);
    }

    [ContextMenu("Sort category by cost ascending")]
    public void SortCategoryByCostAscending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.Card.cost.CompareTo(c2.Card.cost);});
        RearrangeCards();
    }

    [ContextMenu("Sort category by cost descending")]
    public void SortCategoryByCostDescending(){
        uiCardsInCategory.Sort(delegate(CardUI c1, CardUI c2) {return c1.Card.cost.CompareTo(c2.Card.cost);});
        RearrangeCards(true);
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
        Debug.Log($"About to set up cards in category {categoryCardSize}: {cardsInCategory.Count} ui cards will be set up");
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

    public void SetColumns(int numberOfColumns){
        float newWidth = (RectTransform.sizeDelta.x - (gridLayoutGroup.spacing.x * (numberOfColumns - 1)) - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / numberOfColumns;
        cardWidth = newWidth;
        gridLayoutGroup.cellSize = new Vector2(newWidth, newWidth * 1.5f);

        Canvas.ForceUpdateCanvases();

        for(int i = 0; i < uiCardsInCategory.Count; i++){
            uiCardsInCategory[i].PositionAmountGroup();
        }

        if(OnCategoryColumnChanged != null)
            OnCategoryColumnChanged();
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
}

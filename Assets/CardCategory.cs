using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class CardCategory : MonoBehaviour
{

    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;


    [Header("Prefab references")]
    public CategoryHeader categoryHeaderPrefab;
    public CardUI cardUIPrefab;

    [Header("Scene references")]
    [SerializeField] private CategoryHeader categoryHeader;
    public CategoryHeader CategoryHeader => categoryHeader;
    [SerializeField] private List<CardUI> uiCardsInCategory;
    
    [Header("Set via script")]
    [Range(120f, 360f), SerializeField] private float cardWidth = 120f;
    public CardSize categoryCardSize;
    [SerializeField] private List<CardUnityBase> cardsInCategory;


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

    public void SetupCardCategory(CardSize cardSize, List<CardUnityBase> cards){
        categoryHeader = Instantiate<CategoryHeader>(categoryHeaderPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        categoryHeader.transform.SetSiblingIndex(transform.GetSiblingIndex());

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

        cardsInCategory = cards;
        categoryCardSize = cardSize;

        SetupCardsInCategory();

        gameObject.name = $"{transform.GetSiblingIndex().ToString("000")}-{categoryName}";
    }

    public void SetupCardsInCategory(){
        Debug.Log($"About to set up cards in category {categoryCardSize}: {cardsInCategory.Count} ui cards will be set up");
        if(cardsInCategory.Count < 1) return;
        cardsInCategory.Sort();
        CardUI newCard;
        for(int i = 0; i < cardsInCategory.Count; i++){
            newCard = Instantiate<CardUI>(cardUIPrefab, Vector3.zero, Quaternion.identity, transform);
            newCard.SetupCardUI(cardsInCategory[i]);
            uiCardsInCategory.Add(newCard);
        }
    }

    public void SetFactionTo(Faction activeFaction){
        foreach(CardUI cui in uiCardsInCategory){
            cui.ToggleByFaction(activeFaction);
        }
    }

    public void SetColumns(int numberOfColumns){
        float newWidth = (rectTransform.sizeDelta.x - (gridLayoutGroup.spacing.x * (numberOfColumns - 1)) - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right) / numberOfColumns;
        cardWidth = newWidth;
        gridLayoutGroup.cellSize = new Vector2(newWidth, newWidth * 1.5f);
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cardWidth != gridLayoutGroup.cellSize.x)
            gridLayoutGroup.cellSize = new Vector2(cardWidth, cardWidth * 1.5f);
    }
}

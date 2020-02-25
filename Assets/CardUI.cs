using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button), typeof(Image))]
public class CardUI : MonoBehaviour
{

    public delegate void SelectCardDelegate(CardUI selected);
    public static event SelectCardDelegate onCardSelected;

    // [Header("Settings")]
    // [SerializeField] private Vector2 anchoredPositionOffset = Vector2.zero;

    [Header("Scene references")]
    [SerializeField] private Button cardButton;
    [SerializeField] private Image cardImage;
    [SerializeField] private RectTransform amountGroupTransform;
    [SerializeField] private TextMeshProUGUI amountInCollectionText;
    [SerializeField] private TextMeshProUGUI amountInDeckText;
    
    // TODO: amountInCollection & amountInDeck 

    private RectTransform rectTransform;

    [Header("Set via script")]
    [ReadOnly, SerializeField] private int currentAmountInCollection;
    [ReadOnly, SerializeField] private int currentAmountInDeck;
    public float spriteWidth = 0f;
    public float spriteHeight = 0f;
    public float rightGap = 0f;
    public float topGap = 0f;
    [SerializeField] private CardUnityBase card;


    public int CurrentAmountInCollection {
        get { return currentAmountInCollection; }
        set {
            currentAmountInCollection = value;
            SetAmountInCollectionText(currentAmountInCollection);
        }
    }

    public int CurrentAmountInDeck {
        get { return currentAmountInDeck; }
        set {
            currentAmountInDeck = value;
            SetAmountInDeckText(currentAmountInDeck);
        }
    }

    public CardUnityBase Card => card;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        cardButton = GetComponent<Button>();
        cardImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if(card != null){
            card.UnlinkUICard(this);
            card.LinkUICard(this);
        }
    }

    public void SetupCardUI(CardUnityBase baseCard){
        // Debug.Log($"Setting up ui card for {baseCard.cardName}", baseCard);
        if(card != null){
            card.UnlinkUICard(this);
        }

        card = baseCard;
        
        if(cardImage == null || cardButton == null || rectTransform == null){
            Awake();
        }

        cardImage.sprite = card.sprite;
        gameObject.name = baseCard.cardName;

        PositionAmountGroup();

        baseCard.LinkUICard(this);
    }

    public void PositionAmountGroup(float customXOffset = 0f, float customYOffset = 0f){
        spriteWidth = (rectTransform.sizeDelta.y / cardImage.sprite.texture.height) * cardImage.sprite.texture.width;
        spriteHeight = (rectTransform.sizeDelta.x / cardImage.sprite.texture.width) * cardImage.sprite.texture.height;

        if(spriteHeight > rectTransform.sizeDelta.y){
            topGap = 0f;
        } else {
            topGap = (rectTransform.sizeDelta.y % spriteHeight) * 0.5f;
        }

        if(spriteWidth > rectTransform.sizeDelta.x){
            rightGap = 0f;
        } else {
            rightGap = (rectTransform.sizeDelta.x % spriteWidth) * 0.5f;
        }

        Vector2 customOffset = new Vector2(customXOffset, customYOffset);
        
        Vector2 cardWidthAnchoredPosition = Vector2.zero;
        cardWidthAnchoredPosition.x = -rightGap;
        cardWidthAnchoredPosition.y = -topGap;

        amountGroupTransform.anchoredPosition = cardWidthAnchoredPosition + customOffset;
    }

    public void MoveCard(bool toDeck, int amount = 1){
        currentAmountInDeck = toDeck ? currentAmountInDeck + amount : currentAmountInDeck - amount;
        SetAmountInDeckText(currentAmountInDeck);
        currentAmountInCollection = toDeck ? currentAmountInCollection - amount : currentAmountInCollection + amount;
        SetAmountInCollectionText(currentAmountInCollection);
    }

    public void SetAmountInCollectionText(int amount){
        amountInCollectionText.text = currentAmountInCollection.ToString();
    }

    public void SetAmountInDeckText(int amount){
        amountInDeckText.text = currentAmountInDeck.ToString();
    }

    [ContextMenu("Test card setup")]
    public void TestCardSetup(){
        if(card == null) return;
        Awake();
        SetupCardUI(card);
    }

    public void ToggleByFaction(Faction activeFaction){
        if(activeFaction == (Faction) 0) {
            gameObject.SetActive(false); 
            return;
        }
        gameObject.SetActive(activeFaction == (Faction) ~0 || card.faction == (Faction) ~0 || card.faction == (Faction) 3 || card.faction == activeFaction);
    }

    public void SelectCard(){
        if(onCardSelected != null){
            onCardSelected(this);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if(card != null){
            card.UnlinkUICard(this);
        }
    }
}

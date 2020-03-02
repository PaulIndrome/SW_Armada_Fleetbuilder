using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// [RequireComponent(typeof(Button), typeof(Image))]
public class CardUI : MonoBehaviour
{

    public delegate void SelectCardDelegate(CardUI selected);
    public static event SelectCardDelegate OnCardSelected;

    [Header("Settings")]
    // [SerializeField] private Vector2 anchoredPositionOffset = Vector2.zero;
    [SerializeField] private Color deckColorNormal;
    [SerializeField] private Color deckColorUsed;
    [SerializeField] private Color collectionColorNormal;
    [SerializeField] private Color collectionColorDepleted;

    [Header("Scene references")]
    [SerializeField] private Button cardButton;
    [SerializeField] private Image cardImage;
    [SerializeField] private RectTransform amountGroupTransform;
    [SerializeField] private TextMeshProUGUI amountInCollectionText;
    [SerializeField] private Image amountInCollectionImage;
    [SerializeField] private TextMeshProUGUI amountInDeckText;
    [SerializeField] private Image amountInDeckImage;
    [SerializeField] private Animator buttonAnimator;
    private int animSelectedHash;
    private int animDeselectedHash;
    
    // TODO: amountInCollection & amountInDeck 

    private RectTransform rectTransform;

    [Header("Set via script")]
    [ReadOnly, SerializeField] private int currentAmountInCollection;
    [ReadOnly, SerializeField] private int currentAmountInCollectionMax;
    [ReadOnly, SerializeField] private int currentAmountInDeck;
    public float spriteWidth = 0f;
    public float spriteHeight = 0f;
    public float rightGap = 0f;
    public float topGap = 0f;
    [ReadOnly(true), SerializeField] private CardUnityBase card;

    public int CurrentAmountInCollection {
        get { return currentAmountInCollection; }
        set {
            //Debug.Log("Calling CurrentAmountInCollection with value: " + value);
            currentAmountInCollection = Mathf.Clamp(value, 0, currentAmountInCollectionMax);
            SetAmountInCollectionText(currentAmountInCollection);
        }
    }

    public int CurrentAmountInDeck {
        get { return currentAmountInDeck; }
        set {
            //Debug.Log("Calling CurrentAmountInDeck with value: " + value);
            currentAmountInDeck = Mathf.Clamp(value, 0, card.isUnique || card.isCommander ? 1 : 999);
            SetAmountInDeckText(currentAmountInDeck);
        }
    }

    public CardUnityBase Card => card;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(!cardButton)
            cardButton = GetComponent<Button>();
            
        if(!cardImage)
            cardImage = GetComponent<Image>();

        if(!rectTransform)
            rectTransform = GetComponent<RectTransform>();

        animSelectedHash = Animator.StringToHash("Selected");
        animDeselectedHash = Animator.StringToHash("Deselected");
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

    public void MoveCard(string id, bool toDeck, int amount = 1){
        // only change amount in deck if THIS is the card that was moved to deck
        if(id == card.ID){
            // Debug.Log($"id: {id} == card.ID {card.ID}");
            CurrentAmountInDeck = toDeck ? CurrentAmountInDeck + amount : CurrentAmountInDeck - amount;
        }
        
        // amount in collection is "purely" visual
        CurrentAmountInCollection = toDeck ? CurrentAmountInCollection - amount : CurrentAmountInCollection + amount;
    }

    /// <summary>Toggles the card clickable (true), unclickable (false) or inverse-state (null)</summary>
    public void ToggleCardAvailability(string id, bool? onOff){
        cardButton.interactable = onOff.HasValue ? onOff.Value : !cardButton.interactable;
    }

    public void SetAmountInCollectionText(int amount){
        amountInCollectionImage.color = amount < 1 ? collectionColorDepleted : collectionColorNormal;
        amountInCollectionText.text = currentAmountInCollection.ToString();
    }

    public void SetAmountInDeckText(int amount){
        amountInDeckImage.color = amount > 0 ? deckColorUsed : deckColorNormal;
        amountInDeckText.text = currentAmountInDeck.ToString();
    }

    public void SetCollectionEntry(CardCollectionEntry entry){
        //Debug.Log($"Setting collection entry {entry.Identifier} to {gameObject.name}", gameObject);
        SetCollectionAmount(entry.AmountMax);
        CurrentAmountInDeck = 0;
    }

    private void SetCollectionAmount(int amountMax){
        currentAmountInCollectionMax = amountMax;
        CurrentAmountInCollection = currentAmountInCollectionMax;
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
        if(OnCardSelected != null){
            OnCardSelected(this);
            buttonAnimator.ResetTrigger(animDeselectedHash);
            buttonAnimator.SetTrigger(animSelectedHash);
        }
    }

    public void DeselectCard(){
        Debug.Log("Deselecting card " + name, this);
        buttonAnimator.ResetTrigger(animSelectedHash);
        buttonAnimator.SetTrigger(animDeselectedHash);
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

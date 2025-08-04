using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// #if UNITY_EDITOR
using UnityEngine.Profiling;
// #endif

// [RequireComponent(typeof(Button), typeof(Image))]
public class CardUI : MonoBehaviour
{

    public delegate void SelectCardDelegate(CardUI selected);
    public static event SelectCardDelegate OnCardSelected;

    public delegate void CardMovedDelegate();
    public static event CardMovedDelegate OnCardMoved;

    [Header("Settings")]
    // [SerializeField] private Vector2 anchoredPositionOffset = Vector2.zero;
    [SerializeField] private Color deckColorNormal;
    [SerializeField] private Color deckColorUsed;
    [SerializeField] private Color collectionColorNormal;
    [SerializeField] private Color collectionColorDepleted;

    [Header("Scene references")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button cardButton;
    [SerializeField] private Image cardImage;
    [Space]
    [SerializeField] private RectTransform amountGroupTransform;
    [SerializeField] private TextMeshProUGUI amountInCollectionText;
    [SerializeField] private Image amountInCollectionImage;
    [SerializeField] private TextMeshProUGUI amountInDeckText;
    [SerializeField] private Image amountInDeckImage;
    [Space]
    [SerializeField] private Animator buttonAnimator;
    // [SerializeField] private GameObject inDeckBordersParent;
    [SerializeField] private PositionToSpriteBounds bordersPositionToSpriteBounds;

    [Header("Deck messages card")]
    [SerializeField] private GameObject deckMessageCardMaxPoints;
    [SerializeField] private GameObject deckMessageCardSquadronPoints;
    [SerializeField] private GameObject deckMessageCardCollectionZero;
    [SerializeField] private GameObject deckMessageCardUniqueInCollection;
    private int animSelectedHash;
    private int animDeselectedHash;
    private int animDisabledHash;
    private int animInDeckHash;
    
    // TODO: amountInCollection & amountInDeck 

    private RectTransform rectTransform;

    [Header("Set via script")]
    [ReadOnly(true), SerializeField, Tooltip("Set to true when this card is toggled unavailable by a card with the same name")] 
    private bool blockedByName = false;
    [ReadOnly, SerializeField] private int currentAmountInCollection;
    [ReadOnly, SerializeField] private int currentAmountInCollectionMax;
    [ReadOnly, SerializeField] private int currentAmountInDeck;
    private float spriteWidth = 0f;
    private float spriteHeight = 0f;
    private float rightGap = 0f;
    private float topGap = 0f;
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
            // Debug.Log("Calling CurrentAmountInDeck with value: " + value);
            currentAmountInDeck = Mathf.Clamp(value, 0, card.isUnique || card.isCommander ? 1 : 999);
            amountInDeckImage.color = currentAmountInDeck > 0 ? deckColorUsed : deckColorNormal;
            buttonAnimator.SetBool("InDeck", CurrentAmountInDeck > 0);
            // inDeckBordersParent.SetActive(currentAmountInDeck > 0);
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
        animDisabledHash = Animator.StringToHash("Disabled");
        animInDeckHash = Animator.StringToHash("InDeck");

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

        // Deck.OnCurrentDeckPointsChanged += UpdateDeckMessagesOnCardByEvent;
        OnCardMoved += UpdateDeckMessagesOnCardByStaticDeck;
        Deck.OnFireUpdateDeckMessages += UpdateDeckMessagesOnCardByStaticDeck;
        
        if(CurrentDeck.Deck != null && Card != null)
            UpdateDeckMessagesOnCardByStaticDeck();
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
        gameObject.layer = LayerMask.NameToLayer("UI");
        
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

        bordersPositionToSpriteBounds.RecalculateToSpriteBounds();
    }

    public void MoveCard(string id, bool toDeck, int amount = 1){
        // only change amount in deck if THIS is the card that was moved to deck
        if(id == card.ID){
            // Debug.Log($"id: {id} == card.ID {card.ID}");
            CurrentAmountInDeck = toDeck ? CurrentAmountInDeck + amount : CurrentAmountInDeck - amount;
        }
        
        // amount in collection is "purely" visual
        CurrentAmountInCollection = toDeck ? CurrentAmountInCollection - amount : CurrentAmountInCollection + amount;

        if(OnCardMoved != null)
            OnCardMoved();
    }

    /// <summary>Toggles the card clickable (true), unclickable (false) or inverse-state (null)</summary>
    public void ToggleCardAvailability(string blockingName, bool onOff){
        // Debug.Log(Card.ID + $" toggled {onOff} by " + blockingName);
        if(blockedByName){
            if(blockingName != Card.cardName){
                return;
            } 
        } 

        blockedByName = blockingName == Card.cardName && !onOff;
        cardButton.interactable = onOff;

        // if(onOff.HasValue && onOff.Value && blockedByName.Length > 0 && blockingName == blockedByName){
        //     cardButton.interactable = true;
        // } 

        // Debug.Log(Card.ID + " interactible = " + cardButton.interactable);
        buttonAnimator.SetBool("Disabled", !cardButton.interactable);
    }

    public void SetAmountInCollectionText(int amount){
        amountInCollectionImage.color = amount < 1 ? collectionColorDepleted : collectionColorNormal;
        amountInCollectionText.text = currentAmountInCollection.ToString();
    }

    public void SetAmountInDeckText(int amount){
        amountInDeckText.text = currentAmountInDeck.ToString();
    }

    public void SetCollectionEntry(CardCollectionEntry entry){
        //Debug.Log($"Setting collection entry {entry.Identifier} to {gameObject.name}", gameObject);
        SetCollectionAmountMax(entry.AmountMax);
        CurrentAmountInDeck = 0;
    }

    private void SetCollectionAmountMax(int amountMax){
        currentAmountInCollectionMax = amountMax;
        CurrentAmountInCollection = currentAmountInCollectionMax;
    }

    public void ResetCardAccordingToCurrentCollection(){
        CurrentAmountInCollection = currentAmountInCollectionMax;
        CurrentAmountInDeck = 0;
        ToggleCardAvailability(Card.ID, true);
    }

    public void ToggleByFaction(Faction activeFaction){
        if(activeFaction == (Faction) 0) {
            gameObject.SetActive(false); 
            return;
        }
        bool shouldBeActive = activeFaction == (Faction) ~0 || card.faction == (Faction) ~0 || card.faction == (Faction) 3 || card.faction == activeFaction; 
        // if(!shouldBeActive && CurrentAmountInDeck > 0){
            
        // }
        gameObject.SetActive(shouldBeActive);
        // Debug.Log(Card.ID + " ui card set to " + shouldBeActive, buttonAnimator);
    }

    public void ToggleView(ViewType view){
        switch(view){
            case ViewType.DeckView:
                gameObject.SetActive(CurrentAmountInDeck > 0);
                break;
            case ViewType.BuildView:
                gameObject.SetActive(currentAmountInCollectionMax > 0);
                break;
            case ViewType.CollectionView:
                gameObject.SetActive(true);
                break;
        }
    }

    public void ToggleForUpgradeSetMode(UpgradeType type, bool keepFaction = true){
        if(card is CardUnityUpgrade){
            bool active = ((CardUnityUpgrade)card).HasUpgradeType(type);
            if(keepFaction){
                active = active && (card.faction == CurrentDeck.Deck.DeckFaction || card.faction.FactionIsEverything() || CurrentDeck.Deck.DeckFaction == (Faction) ~0);
            }
            gameObject.SetActive(active);
            // if((!keepFaction || (keepFaction && (Card.faction == CurrentDeck.Deck.DeckFaction || Card.faction == (Faction) ~0 || CurrentDeck.Deck.DeckFaction == (Faction) ~0)))){
            //     Debug.Log($"{Card.ID} is still valid");
            // }
        }
    }

    // public void UpdateDeckMessagesOnCardByEvent(int pointsCurrent, int pointsMax, int squadronPoints){
    //     // Debug.Log("Updating deck messages on " + Card.ID);
    //     ToggleMaxPointsExceeded(pointsCurrent + Card.cost > pointsMax);
    //     ToggleThirdSquadronPointsExceeded(Card is CardUnitySquadron && (squadronPoints + Card.cost) * 3 > pointsMax);
    //     ToggleCollectionAmountZero(CurrentAmountInCollection < 1, Card.isUnique);
    // }

    public void UpdateDeckMessagesOnCardByStaticDeck(){
        // #if UNITY_EDITOR
            Profiler.BeginSample("UpdateDeckMessagesOnCardByStaticDeck()", this);
        // #endif

        ToggleCollectionAmountZero(CurrentAmountInCollection < 1, Card.isUnique);
        ToggleMaxPointsExceeded(CurrentDeck.Deck.PointsCurrent + Card.cost > CurrentDeck.Deck.PointsMax);
        ToggleThirdSquadronPointsExceeded(Card is CardUnitySquadron && (CurrentDeck.Deck.SquadronPoints + Card.cost) > CurrentDeck.Deck.MaxSquadronPoints);
        if(Card is CardUnityUpgrade && !Card.isCommander){
            // CurrentDeck.deck.availableSlots.Find
        }

        // #if UNITY_EDITOR
            Profiler.EndSample();
        // #endif
    }

    public void ToggleMaxPointsExceeded(bool onOff){
        // Debug.Log($"Toggling max points exceeded icon {(onOff ? "on" : "off")}");
        deckMessageCardMaxPoints.SetActive(onOff);
    }
    public void ToggleThirdSquadronPointsExceeded(bool onOff){
        // Debug.Log($"Toggling squadron points exceeded icon {(onOff ? "on" : "off")}");
        deckMessageCardSquadronPoints.SetActive(onOff);
    }
    public void ToggleCollectionAmountZero(bool onOff, bool isUnique = false){
        // Debug.Log($"Toggling collection amount icon {(onOff ? "on" : "off")}");
        deckMessageCardUniqueInCollection.SetActive(false);
        deckMessageCardCollectionZero.SetActive(false);
        if(isUnique){
            deckMessageCardUniqueInCollection.SetActive(onOff);
        } else {
            deckMessageCardCollectionZero.SetActive(onOff);
        }
    }

    public void SelectCard(){
        if(OnCardSelected != null){
            if(UpgradeSlotButtonsControl.UPGRADE_SET_MODE){
                // Debug.Log("Setting deselected trigger on " + Card.ID);
                buttonAnimator.SetBool(animSelectedHash, false);
            } else {
                // Debug.Log("Setting selected trigger on " + Card.ID);
                buttonAnimator.SetBool(animSelectedHash, true);
            }
            OnCardSelected(this);
        }
    }

    public void DeselectCard(){
        // Debug.Log("Deselecting card " + name, this);
        buttonAnimator.SetBool(animSelectedHash, false);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if(card != null){
            card.UnlinkUICard(this);
        }
        // Deck.OnCurrentDeckPointsChanged -= UpdateDeckMessagesOnCardByEvent;
        OnCardMoved -= UpdateDeckMessagesOnCardByStaticDeck;
        Deck.OnFireUpdateDeckMessages -= UpdateDeckMessagesOnCardByStaticDeck;
    }
}

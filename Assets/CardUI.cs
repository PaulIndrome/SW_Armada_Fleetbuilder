using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class CardUI : MonoBehaviour
{

    public delegate void SelectCardDelegate(CardUnityBase selected);
    public static event SelectCardDelegate onCardSelected;

    [SerializeField] private CardUnityBase card;
    private Button button;
    private Image image;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        // button.onClick.AddListener(SelectCard);
    }

    public void SetupCardUI(CardUnityBase baseCard){
        Debug.Log($"Setting up ui card for {baseCard.cardName}", baseCard);
        card = baseCard;
        
        if(image == null || button == null){
            Awake();
        }

        image.sprite = card.sprite;
        gameObject.name = baseCard.cardName;
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
            onCardSelected(card);
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        // button.onClick.RemoveListener(SelectCard);
    }
}

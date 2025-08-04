using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CurrentFactionIcon : MonoBehaviour
{

    private Image image;

    [Header("Asset references")]
    [SerializeField] private Sprite iconRebellion;
    [SerializeField] private Sprite iconMix;
    [SerializeField] private Sprite iconEmpire;

    [Header("Settings")]
    [SerializeField] private Color colorRebellion;
    [SerializeField] private Color colorEmpire;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        DeckContentControl.OnDeckLoaded += SwitchFactionIcon;
        DeckContentControl.OnNewDeckStarted += SwitchFactionIcon;
    }

    public void SwitchFactionIcon(Deck deck){
        switch(deck.DeckFaction){
            case Faction.Empire:
                image.sprite = iconEmpire;
                image.color = colorEmpire;
                break;
            case Faction.Rebellion:
                image.sprite = iconRebellion;
                image.color = colorRebellion;
                break;
            case (Faction) ~0:
                image.sprite = iconMix;
                image.color = colorRebellion;
                break;
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        DeckContentControl.OnDeckLoaded -= SwitchFactionIcon;
        DeckContentControl.OnNewDeckStarted -= SwitchFactionIcon;
    }
}

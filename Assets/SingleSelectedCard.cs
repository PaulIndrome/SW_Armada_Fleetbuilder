using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SingleSelectedCard : MonoBehaviour
{
    private Image image;
    private CardUnityBase currentSelectedCard;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        image = GetComponent<Image>();
        image.preserveAspect = true;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        Debug.Log("SingleSelectedCard OnEnable()", this);
        CardUI.onCardSelected -= SetCurrentSelectedCard;
        CardUI.onCardSelected += SetCurrentSelectedCard;
    }

    public void SetCurrentSelectedCard(CardUnityBase cardToSet){
        currentSelectedCard = cardToSet;
        if(cardToSet != null){
            image.sprite = cardToSet.sprite;
            image.color = Color.white;
        } else {
            image.sprite = null;
            image.color = Color.clear;
        }

    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        CardUI.onCardSelected -= SetCurrentSelectedCard;
    }
}

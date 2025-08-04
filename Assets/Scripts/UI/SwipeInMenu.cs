using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SwipeInMenu : MonoBehaviour
{
    public enum SwipeFrom {
        Top,
        Bottom, 
        Left, 
        Right
    }

    [Header("Debug")]
    [SerializeField] private bool showSwipeAreas = false;
    [SerializeField] private Texture swipeInTexture, swipeOutTexture;

    [Header("Set via script")]
    [SerializeField] private bool isOnScreen = false;
    
    [Header("Scene references")]
    public RectTransform menuRectTransform;
    public Animator swipeInMenuAnimator;

    [Header("Toggles")]
    [SerializeField] Toggle factionFoldoutToggle;
    
    [Header("Settings")]
    public float animationDuration = 1;
    public Vector2 normalizedHandleSize = Vector2.zero;
    public SwipeFrom swipeFromDirection = SwipeFrom.Right;



    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        if(!menuRectTransform) enabled = false;
        switch(swipeFromDirection){
            case SwipeFrom.Top:
                menuRectTransform.pivot = new Vector2(0.5f, 1f);
                break;
            case SwipeFrom.Bottom:
                menuRectTransform.pivot = new Vector2(0.5f, 0f);
                break;
            case SwipeFrom.Right:
                menuRectTransform.pivot = new Vector2(1f, 0.5f);
                break;
            case SwipeFrom.Left:
                menuRectTransform.pivot = new Vector2(0f, 0.5f);
                break;
        }

    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        SwipeAway();
    }

    // /// <summary>
    // /// OnGUI is called for rendering and handling GUI events.
    // /// This function can be called multiple times per frame (one call per event).
    // /// </summary>
    // void OnGUI()
    // {
    //     if(showSwipeAreas){
    //         switch(swipeFromDirection){
    //             case SwipeFrom.Top:
    //                     GUI.Box(new Rect(0, 0, Screen.width, Screen.height * swipeInScreenAreaSize), swipeInTexture);
    //                     GUI.Box(new Rect(0, 0, Screen.width, Screen.height * swipeAwayScreenAreaSize), swipeOutTexture);
    //                 break;
    //             case SwipeFrom.Bottom:
    //                     GUI.Box(new Rect(0, Screen.height - (Screen.height * swipeInScreenAreaSize), Screen.width, Screen.height * swipeInScreenAreaSize), swipeInTexture);
    //                     GUI.Box(new Rect(0, Screen.height - (Screen.height * swipeAwayScreenAreaSize), Screen.width, Screen.height * swipeAwayScreenAreaSize), swipeOutTexture);
    //                 break;
    //             case SwipeFrom.Right:
    //                     GUI.Box(new Rect(Screen.width - (Screen.width * swipeInScreenAreaSize), 0, Screen.width * swipeInScreenAreaSize, Screen.height), swipeInTexture);
    //                     GUI.Box(new Rect(Screen.width - (Screen.width * swipeAwayScreenAreaSize), 0, Screen.width * swipeAwayScreenAreaSize, Screen.height), swipeOutTexture);
    //                 break;
    //             case SwipeFrom.Left:
    //                     GUI.Box(new Rect(0, 0, Screen.width * swipeInScreenAreaSize, Screen.height), swipeInTexture);
    //                     GUI.Box(new Rect(0, 0, Screen.width * swipeAwayScreenAreaSize, Screen.height), swipeOutTexture);
    //                 break;
    //         }
    //     }
    // }

    // public void SwipeReaction(SwipeData swipeData){
    //     if(swipeData.TouchIndex < 1){
    //         switch(swipeFromDirection){
    //             case SwipeFrom.Top:
    //                     if(isOnScreen && swipeData.Direction == SwipeDirection.Up && swipeData.StartPosition.y > Screen.height - (Screen.height * swipeAwayScreenAreaSize)){
    //                         SwipeAway();
    //                     } else if (!isOnScreen && swipeData.Direction == SwipeDirection.Down && swipeData.StartPosition.y > Screen.height - (Screen.height * swipeInScreenAreaSize)){
    //                         SwipeIn();
    //                     }
    //                 break;
    //             case SwipeFrom.Bottom:
    //                     if(isOnScreen && swipeData.Direction == SwipeDirection.Down && swipeData.StartPosition.y < (Screen.height * swipeAwayScreenAreaSize)){
    //                         SwipeAway();
    //                     } else if (!isOnScreen && swipeData.Direction == SwipeDirection.Up && swipeData.StartPosition.y < (Screen.height * swipeInScreenAreaSize)){
    //                         SwipeIn();
    //                     }
    //                 break;
    //             case SwipeFrom.Right:
    //                     if(isOnScreen && swipeData.Direction == SwipeDirection.Right && swipeData.StartPosition.x > Screen.width - (Screen.width * swipeAwayScreenAreaSize)){
    //                         SwipeAway();
    //                     } else if (!isOnScreen && swipeData.Direction == SwipeDirection.Left && swipeData.StartPosition.x > Screen.width - (Screen.width * swipeInScreenAreaSize)){
    //                         SwipeIn();
    //                     }
    //                 break;
    //             case SwipeFrom.Left:
    //                     if(isOnScreen && swipeData.Direction == SwipeDirection.Left && swipeData.StartPosition.x < (Screen.width * swipeAwayScreenAreaSize)){
    //                         SwipeAway();
    //                     } else if (!isOnScreen && swipeData.Direction == SwipeDirection.Right && swipeData.StartPosition.x < (Screen.width * swipeInScreenAreaSize)){
    //                         SwipeIn();
    //                     }
    //                 break;
    //         }
            
    //     }
    // }

    public void ToggleInOut(bool toScreen){
        if(toScreen){
            SwipeIn();
        } else {
            SwipeAway();
        }
    }

    public void ToggleFactionDrawer(bool drawerOut){
        swipeInMenuAnimator.SetBool("FactionsDrawerOut", drawerOut);
    }

    [ContextMenu("Test swipe in")]
    public void SwipeIn(){
        menuRectTransform.DOAnchorPos(Vector2.zero, animationDuration);
        isOnScreen = true;    
    }


    [ContextMenu("Test swipe out")]
    public void SwipeAway(){
        Vector2 swipeOutPosition = Vector2.zero;
        Vector2 handlePixelSize = menuRectTransform.sizeDelta * normalizedHandleSize;
        switch(swipeFromDirection){
            case SwipeFrom.Top:
                swipeOutPosition.y += menuRectTransform.sizeDelta.y - handlePixelSize.y;
                break;
            case SwipeFrom.Bottom:
                swipeOutPosition.y -= menuRectTransform.sizeDelta.y + handlePixelSize.y;
                break;
            case SwipeFrom.Right:
                swipeOutPosition.x += menuRectTransform.sizeDelta.x - handlePixelSize.x;
                break;
            case SwipeFrom.Left:
                swipeOutPosition.x -= menuRectTransform.sizeDelta.x + handlePixelSize.x;
                break;
        }

        factionFoldoutToggle.onValueChanged.Invoke(false);
        
        menuRectTransform.DOAnchorPos(swipeOutPosition, animationDuration);
        isOnScreen = false;
    }

    
}

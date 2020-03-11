using UnityEngine;
using UnityEngine.UI;

public class SwipedCardSelection : MonoBehaviour {

    [Header("Settings")]
    public float activationDistanceRight = 50f;
    public float activationDistanceLeft = 50f;
    public float activationDistanceUp = 50f;
    public float activationDistanceDown = 50f;

    private bool hitSwipableObject = false;
    
    public Rect draggedObjectRect;
    public Image image;
    [ReadOnly, SerializeField] private Vector2 draggedObjectStartPos;
    [SerializeField] RectTransform draggedObjectRectTransform;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        draggedObjectStartPos = draggedObjectRectTransform.anchoredPosition;
        draggedObjectRect = draggedObjectRectTransform.rect;
    }

    // /// <summary>
    // /// Update is called every frame, if the MonoBehaviour is enabled.
    // /// </summary>
    // void Update()
    // {
    //     draggedObjectRect = draggedObjectRectTransform.rect;
    // }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        SwipeDetector.OnSwipeDetected += SwipeReaction;
    }

    public void SwipeReaction(SwipeData swipeData){
        if(swipeData.TouchIndex < 1){
            // Debug.Log("Inside SwipeReaction()");
            if(!hitSwipableObject && draggedObjectRectTransform.rect.Contains(draggedObjectRectTransform.InverseTransformPoint(swipeData.StartPosition))){
                hitSwipableObject = true;
            }
            if(hitSwipableObject && (swipeData.Direction == SwipeDirection.Up || swipeData.Direction == SwipeDirection.Down)){
                // Debug.Log("Dragging");
                swipeData.Distance.x = 0;
                draggedObjectRectTransform.anchoredPosition = draggedObjectStartPos + swipeData.Distance;
                float swipeDistance = draggedObjectStartPos.y + draggedObjectRectTransform.anchoredPosition.y;
                if(swipeDistance > activationDistanceUp){
                    image.color = Color.green;
                } else if(swipeDistance < -activationDistanceUp){
                    image.color = Color.red;
                } else {
                    image.color = Color.white;
                }
            }
            if(swipeData.TouchPhase == TouchPhase.Ended){
                // float swipeDistance = draggedObjectStartPos.y - draggedObjectRectTransform.anchoredPosition.y;
                // if(swipeDistance > activationDistance || swipeDistance < -activationDistance){
                //     Debug.Log("Activation!");
                // }
                draggedObjectRectTransform.anchoredPosition = draggedObjectStartPos;
                hitSwipableObject = false;
                image.color = Color.white;
            }
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        SwipeDetector.OnSwipeDetected -= SwipeReaction;
    }

}
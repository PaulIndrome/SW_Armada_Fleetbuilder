using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectContentSwitcher : MonoBehaviour
{
    
    public enum SwitchDirection {
        Horizontal,
        Vertical
    }
    public float activationDistance = 50f;
    public float switchDuration = 0.5f;
    public float[] scrollRectPositions;
    public SwitchDirection switchDirection;
    public RectTransform switchContentSwipeArea;
    public List<RectTransform> contentTransforms;
    public Image image;
    public UnityEventString OnContentSwitch;

    private bool pointerHit = false;
    private bool switchingContent = false;
    [SerializeField] private bool swiping = false;

    private int currentContentIndex = 0;

    private Canvas[] contentCanvases;

    public int CurrentContentIndex {
        get { return currentContentIndex; }
        set {
            currentContentIndex = value;
            currentContentTransform = contentTransforms[currentContentIndex];
            currentContentStartPosition = currentContentTransform.anchoredPosition;
        }
    }
    
    private RectTransform currentContentTransform;
    private ScrollRect scrollRect;
    private Vector2 scrollRectScreenSize;
    private Vector2 currentContentStartPosition;
    private Coroutine contentSwitchRoutine;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if(contentTransforms == null || contentTransforms.Count < 1) {
            enabled = false;
            return;
        }

        scrollRectPositions = new float[contentTransforms.Count];
        contentCanvases = new Canvas[contentTransforms.Count];
        for(int i = 0; i < contentCanvases.Length; i++){
            contentCanvases[i] = contentTransforms[i].GetComponent<Canvas>();
            scrollRectPositions[i] = 1;
        }

        CurrentContentIndex = 0;
        SwitchToContent(CurrentContentIndex);
        // currentContentStartPosition = contentTransforms[CurrentContentIndex].anchoredPosition;
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        SwipeDetector.OnSwipeDetected += SwipeReaction;
    }

    void SwipeReaction(SwipeData swipeData){
        if(!switchingContent && swipeData.TouchIndex < 1){
            if(swipeData.TouchPhase == TouchPhase.Ended && swiping){
                Debug.Log("TouchPhase Ended");
                if(Mathf.Abs(swipeData.Distance.x) > activationDistance){
                    SwitchToContent(swipeData.Direction == SwipeDirection.Right ? -1 : 1);
                    return;
                } else {
                    ResetContentTransform();
                    return;
                }
            } 

            if(!swiping && switchContentSwipeArea.rect.Contains(switchContentSwipeArea.InverseTransformPoint(swipeData.StartPosition))){
                swiping = true;
            }
            // Debug.Log("Swiping content switcher");
            if(swiping && (swipeData.Direction == SwipeDirection.Right || swipeData.Direction == SwipeDirection.Left)){
                // scrollRect.content = null;
                swipeData.Distance.y = 0;
                currentContentStartPosition.y = currentContentTransform.anchoredPosition.y;
                contentTransforms[CurrentContentIndex].anchoredPosition = currentContentStartPosition + swipeData.Distance;
                
                // Debug.Log(swipeData.Distance);
                // image.color = Mathf.Abs(swipeData.Distance.x) > activationDistance ? Color.green : Color.white;
            }
            // swipeData.Distance.x *= switchDirection == SwitchDirection.Horizontal ? 1 : 0;
        }
    }

    void ResetContentTransform(){
        Debug.Log("Resetting content switch");
        scrollRectPositions[CurrentContentIndex] = scrollRect.verticalNormalizedPosition;
        currentContentTransform.anchoredPosition *= Vector2.up;
        scrollRect.content = currentContentTransform;
        swiping = false;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        SwipeDetector.OnSwipeDetected -= SwipeReaction;
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     Debug.Log("Pointer down");
    //     pointerHit = true;
    //     currentContentStartPosition = contentTransforms[currentContentIndex].anchoredPosition;
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     pointerHit = false;
    // }

    public void SwitchToContent(int contentChange){
        if(contentChange == 0) {
            for(int i = 0; i < contentCanvases.Length; i++){
                contentCanvases[i].enabled = CurrentContentIndex == i;
            }
            return;
        }
        
        scrollRectPositions[CurrentContentIndex] = scrollRect.verticalNormalizedPosition;

        scrollRect.StopMovement();
        scrollRect.content = null;

        // Debug.Log("scrollRect.verticalNormalizedPosition " + scrollRect.verticalNormalizedPosition);
        // Debug.Log("scrollRect.normalizedPosition.y " + scrollRect.normalizedPosition.y);

        int newContentIndex = CurrentContentIndex + contentChange;

        // Debug.Log(currentContentIndex + " -> " + contentChange);
        int actualContentIndex = (newContentIndex + contentTransforms.Count) % contentTransforms.Count;
        // Debug.Log("newContentIndex %= contentTransforms.Count -> " + newContentIndex);
        
        if(switchDirection == SwitchDirection.Horizontal){
            contentSwitchRoutine = StartCoroutine(SwitchToContentRoutine(actualContentIndex, contentChange < 0 ? Vector2.right : Vector2.left));
        } else {
            contentSwitchRoutine = StartCoroutine(SwitchToContentRoutine(actualContentIndex, contentChange < 0 ? Vector2.down : Vector2.up));
        }
    }

    IEnumerator SwitchToContentRoutine(int newContentIndex, Vector2 direction){
        switchingContent = true;

        RectTransform currentContent = contentTransforms[CurrentContentIndex];
        RectTransform newContent = contentTransforms[newContentIndex];

        Vector2 currentStartPos = currentContentTransform.anchoredPosition;
        Vector2 newStartPos = currentContentStartPosition - newContent.rect.size * direction;

        // newContent.gameObject.SetActive(true);
        contentCanvases[newContentIndex].enabled = true;

        for(float t = 0; t < switchDuration; t += Time.deltaTime){
            Vector2 directionDelta = direction * (t / switchDuration);
            currentContent.anchoredPosition = currentStartPos + currentContent.rect.size * directionDelta;
            newContent.anchoredPosition = newStartPos + newContent.rect.size * directionDelta;
            yield return null;
        }

        contentCanvases[currentContentIndex].enabled = false;

        scrollRect.content = newContent;
        // yield return new WaitForEndOfFrame();
        // Canvas.ForceUpdateCanvases();
        // yield return new WaitForEndOfFrame();
        scrollRect.normalizedPosition = new Vector2(0, scrollRectPositions[newContentIndex]);
        // scrollRect.verticalScrollbar.SetValueWithoutNotify(scrollRectPosition);
        // scrollRect.verticalScrollbar.value = scrollRectPosition;
        // scrollRect.verticalNormalizedPosition = scrollRectPosition;
        // yield return new WaitForEndOfFrame();
        // Canvas.ForceUpdateCanvases();

        // currentContent.gameObject.SetActive(false);

        // newContent.anchoredPosition = Vector2.zero;

        CurrentContentIndex = newContentIndex;

        // image.color = Color.white;

        if(OnContentSwitch != null){
            OnContentSwitch.Invoke(currentContentTransform.name);
        }

        switchingContent = false;
        swiping = false;
    }
}

[System.Serializable]
public class UnityEventString : UnityEvent<string>{

}
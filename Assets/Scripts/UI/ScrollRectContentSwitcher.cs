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

    [Header("Settings")]
    public float switchDuration = 0.5f;
    public SwitchDirection switchDirection;
    
    [Header("Scene references")]
    public List<RectTransform> contentTransforms;
    public UnityEventString OnContentSwitch;

    [Header("Set via script")]
    [ReadOnly, SerializeField] private float[] scrollRectPositions;

    private bool switchingContent = false;

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
        SwitchToContent(CurrentContentIndex, Vector2.right);
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        UpgradeSlotButtonsControl.OnUpgradeSetMode += SwitchToUpgradeContent;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        scrollRect.normalizedPosition = Vector2.one;
    }

    void ResetContentTransform(){
        Debug.Log("Resetting content switch");
        scrollRectPositions[CurrentContentIndex] = scrollRect.verticalNormalizedPosition;
        currentContentTransform.anchoredPosition *= Vector2.up;
        scrollRect.content = currentContentTransform;
    }

    public void SwitchContentBy(int changeBy){
        if(changeBy == 0) return;

        int newContentIndex = (int) Mathf.Repeat(CurrentContentIndex + changeBy, contentTransforms.Count);

        // Debug.Log($"ChangeBy {changeBy} to {newContentIndex}");

        SwitchToContent(newContentIndex, changeBy < 0 ? Vector2.right : Vector2.left);
    }

    void SwitchToUpgradeContent(UpgradeType type, int slotIndex, bool onOff){
        if(onOff){
            SwitchToContent(CardType.Upgrade);
        } else {
            SwitchToContent(CardType.Ship);
        }
    }

    public void SwitchToContent(CardType cardType){
        switch(cardType){
            case CardType.Ship:
                SwitchToContent(0, CurrentContentIndex == 2 ? Vector2.left : Vector2.right);
                break;
            case CardType.Squadron:
                SwitchToContent(1, CurrentContentIndex < 1 ? Vector2.left : Vector2.right);
                break;
            case CardType.Upgrade:
                SwitchToContent(2, CurrentContentIndex == 0 ? Vector2.right : Vector2.left);
                break;
            case CardType.Objective:
                SwitchToContent(3, CurrentContentIndex == 0 ? Vector2.right : Vector2.left);
                break;
        }
    }

    void SwitchToContent(int contentToChangeTo, Vector2 direction){
        if(switchingContent) return;
        
        for(int i = 0; i < contentCanvases.Length; i++){
            contentCanvases[i].enabled = i == CurrentContentIndex || i == contentToChangeTo;
        }

        if(contentToChangeTo == CurrentContentIndex) return;

        switchingContent = true;
        
        scrollRectPositions[CurrentContentIndex] = scrollRect.verticalNormalizedPosition;

        scrollRect.StopMovement();
        scrollRect.content = null;

        // Debug.Log("scrollRect.verticalNormalizedPosition " + scrollRect.verticalNormalizedPosition);
        // Debug.Log("scrollRect.normalizedPosition.y " + scrollRect.normalizedPosition.y);

        int newContentIndex = contentToChangeTo; //Mathf.Clamp(contentToChangeTo, 0, contentTransforms.Count - 1);//CurrentContentIndex + contentToChangeTo;

        // Debug.Log(currentContentIndex + " -> " + contentChange);
        // int actualContentIndex = (newContentIndex + contentTransforms.Count) % contentTransforms.Count;
        // Debug.Log("newContentIndex %= contentTransforms.Count -> " + newContentIndex);
        
        if(switchDirection == SwitchDirection.Horizontal){
            contentSwitchRoutine = StartCoroutine(SwitchToContentRoutine(newContentIndex, direction));
        } else {
            contentSwitchRoutine = StartCoroutine(SwitchToContentRoutine(newContentIndex, direction));
        }
    }

    IEnumerator SwitchToContentRoutine(int newContentIndex, Vector2 direction){
        switchingContent = true;

        RectTransform currentContent = contentTransforms[CurrentContentIndex];
        RectTransform newContent = contentTransforms[newContentIndex];

        Vector2 currentStartPos = currentContentTransform.anchoredPosition;
        Vector2 newStartPos = new Vector2(0, contentTransforms[newContentIndex].anchoredPosition.y) - newContent.rect.size * direction;

        contentCanvases[newContentIndex].enabled = true;

        for(float t = 0; t < switchDuration; t += Time.deltaTime){
            Vector2 directionDelta = direction * (t / switchDuration);
            currentContent.anchoredPosition = currentStartPos + currentContent.rect.size * directionDelta;
            newContent.anchoredPosition = newStartPos + newContent.rect.size * directionDelta;
            yield return null;
        }

        contentCanvases[currentContentIndex].enabled = false;

        scrollRect.content = newContent;
        scrollRect.normalizedPosition = new Vector2(0, scrollRectPositions[newContentIndex]);

        CurrentContentIndex = newContentIndex;

        if(OnContentSwitch != null){
            OnContentSwitch.Invoke(currentContentTransform.name);
        }

        switchingContent = false;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        UpgradeSlotButtonsControl.OnUpgradeSetMode -= SwitchToUpgradeContent;
    }

}

[System.Serializable]
public class UnityEventString : UnityEvent<string>{

}
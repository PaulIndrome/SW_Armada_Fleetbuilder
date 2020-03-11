using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class BasicSwipableComponent : SwipableComponent, IPointerClickHandler
{

    private Image image; 
    private Color defaultImageColor;
    [SerializeField] private Color draggingOverrideColor = Color.magenta;

    public UnityEvent OnClick;
    public UnityEvent swipeRightAction;
    public UnityEvent swipeLeftAction;
    public UnityEvent swipeUpAction;
    public UnityEvent swipeDownAction;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        image = GetComponent<Image>();
        defaultImageColor = image.color;
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData){
        if(dragActivationRight && swipeRightAction != null) swipeRightAction.Invoke();
        if(dragActivationLeft && swipeLeftAction != null) swipeLeftAction.Invoke();
        if(dragActivationUp && swipeUpAction != null) swipeUpAction.Invoke();
        if(dragActivationDown && swipeDownAction != null) swipeDownAction.Invoke();
        
        base.OnEndDrag(eventData);
    }

    public override void DragActivationVisuals(bool dragging)
    {
        image.color = dragging ? draggingOverrideColor : defaultImageColor;
    }

    public void InvokeSwipeRightAction(){
        if(swipeRightAction != null) swipeRightAction.Invoke();
    }
    public void InvokeSwipeLeftAction(){
        if(swipeLeftAction != null) swipeLeftAction.Invoke();
    }
    public void InvokeSwipeUpAction(){
        if(swipeUpAction != null) swipeUpAction.Invoke();
    }
    public void InvokeSwipeDownAction(){
        if(swipeDownAction != null) swipeDownAction.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClick != null) OnClick.Invoke();
    }
}
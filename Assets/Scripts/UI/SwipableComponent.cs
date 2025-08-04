using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(RectTransform))]
public abstract class SwipableComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [System.Flags]
    public enum DragDirection {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }

    [SerializeField] private bool useThisRectTransform = true;
    [SerializeField] private bool dragVisible = true;
    [SerializeField] private bool returnToStartPos = true;
    [SerializeField] private DragDirection dragDirections;
    [ReadOnly, SerializeField] protected bool dragActivationRight = false, dragActivationLeft = false, dragActivationUp = false, dragActivationDown = false;
    [SerializeField] private float activationDistanceRight = 0, activationDistanceLeft = 0, activationDistanceUp = 0, activationDistanceDown = 0;
    [ReadOnly, SerializeField] private Vector2 dragDistance;
    [ReadOnly, SerializeField] private Vector2 dragStartPos, dragEndPos;
    [ReadOnly, SerializeField] private GameObject draggedObject;
    [ReadOnly, SerializeField] private RectTransform draggedObjectRectTransform;
    [ReadOnly, SerializeField] private Vector2 draggedObjectStartPos;

    // TODO define left, right, down, up events for callback on successfull activation

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        if(useThisRectTransform){
            draggedObjectRectTransform = GetComponent<RectTransform>();
            draggedObjectStartPos = draggedObjectRectTransform.anchoredPosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.pressPosition;
        draggedObject = eventData.pointerPressRaycast.gameObject;
        if(!useThisRectTransform){
            draggedObjectRectTransform = draggedObject.GetComponentInParent<SwipableComponent>().GetComponent<RectTransform>();
            draggedObjectStartPos = draggedObjectRectTransform.anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragDistance.x = eventData.position.x - dragStartPos.x;
        dragDistance.y = eventData.position.y - dragStartPos.y;

        if(dragDirections.HasFlag(DragDirection.Right))
            dragActivationRight = eventData.position.x > dragStartPos.x && Mathf.Abs(dragDistance.x) > activationDistanceRight;
        else
            dragDistance.x = Mathf.Clamp(dragDistance.x, float.MinValue, 0);

        if(dragDirections.HasFlag(DragDirection.Left))
            dragActivationLeft = eventData.position.x < dragStartPos.x && Mathf.Abs(dragDistance.x) > activationDistanceLeft;
        else 
            dragDistance.x = Mathf.Clamp(dragDistance.x, 0, float.MaxValue);

        if(dragDirections.HasFlag(DragDirection.Up))
            dragActivationUp = eventData.position.y > dragStartPos.y && Mathf.Abs(dragDistance.y) > activationDistanceUp;
        else 
            dragDistance.x = Mathf.Clamp(dragDistance.y, float.MinValue, 0);

        if(dragDirections.HasFlag(DragDirection.Down))
            dragActivationDown = eventData.position.y < dragStartPos.y && Mathf.Abs(dragDistance.y) > activationDistanceDown;
        else 
            dragDistance.x = Mathf.Clamp(dragDistance.y, 0, float.MaxValue);

        if(dragVisible){
            draggedObjectRectTransform.anchoredPosition = draggedObjectStartPos + dragDistance;
        }

        DragActivationVisuals(true);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        // if(dragActivationRight || dragActivationLeft || dragActivationUp || dragActivationDown){
        //     Debug.Log("DRAG ACTIVATION successfull");
        // }
        dragEndPos = eventData.position;
        dragActivationRight = dragActivationLeft = dragActivationUp = dragActivationDown = false;
        
        if(returnToStartPos)
            draggedObjectRectTransform.anchoredPosition = draggedObjectStartPos;

        DragActivationVisuals(false);
    }

    public abstract void DragActivationVisuals(bool dragging);
}
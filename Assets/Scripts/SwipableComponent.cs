using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SwipableComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    [SerializeField] private bool dragVisible = true;
    [SerializeField] private bool returnToStartPos = true;

    [ReadOnly, SerializeField] protected bool dragActivationRight = false, dragActivationLeft = false, dragActivationUp = false, dragActivationDown = false;
    [SerializeField] private float activationDistanceRight = 0, activationDistanceLeft = 0, activationDistanceUp = 0, activationDistanceDown = 0;
    [ReadOnly, SerializeField] private Vector2 dragDistance;
    [ReadOnly, SerializeField] private Vector2 dragStartPos, dragEndPos;
    [ReadOnly, SerializeField] private GameObject draggedObject;
    [ReadOnly, SerializeField] private RectTransform draggedObjectRectTransform;
    [ReadOnly, SerializeField] private Vector2 draggedObjectStartPos;

    // TODO define left, right, down, up events for callback on successfull activation

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.pressPosition;
        draggedObject = eventData.pointerPressRaycast.gameObject;
        draggedObjectRectTransform = draggedObject.GetComponentInParent<SwipableComponent>().GetComponent<RectTransform>();
        draggedObjectStartPos = draggedObjectRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragDistance.x = eventData.position.x - dragStartPos.x;
        dragDistance.y = eventData.position.y - dragStartPos.y;

        dragActivationRight =   eventData.position.x > dragStartPos.x && Mathf.Abs(dragDistance.x) > activationDistanceRight;
        dragActivationLeft =    eventData.position.x < dragStartPos.x && Mathf.Abs(dragDistance.x) > activationDistanceLeft;
        dragActivationUp =      eventData.position.y > dragStartPos.y && Mathf.Abs(dragDistance.y) > activationDistanceUp;
        dragActivationDown =    eventData.position.y < dragStartPos.y && Mathf.Abs(dragDistance.y) > activationDistanceDown;

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
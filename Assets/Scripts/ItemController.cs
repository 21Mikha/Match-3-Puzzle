using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum Status { unAssigned, visible, shadowed, invisible, gotMatched }
public enum Position { unAssigned, left, middle, right }
public class ItemController : MonoBehaviour
{
    public int id;
    public Status status = Status.unAssigned;
    public Position position = Position.unAssigned;
    public Slot CurrentParentSlot = null;

    public ItemView itemView;
    public DraggableObject draggableObject;

    private Vector3 originalPosition;
    public bool isClickable;

    private void Awake()
    {
        itemView = GetComponent<ItemView>();
        draggableObject = GetComponent<DraggableObject>();

        draggableObject.OnSlotEnter += HandleSlotEnter;
        draggableObject.OnDragStart += HandleDragStart;
        draggableObject.OnDragEnd += HandleDragEnd;
    }

    public void InitializeItem(int id, Slot ParentSlot)
    {
        this.id = id;
        CurrentParentSlot = ParentSlot;
        originalPosition = this.transform.position;
        itemView.SetItemSprite(id);
    }

    public void UpdateStatus(Status _status)
    {
        status = _status;
        if (_status == Status.visible)
        {
            isClickable = true;
            itemView.ApplyColor();
            draggableObject._collider.enabled = true;
        }
        else if (_status == Status.shadowed)
        {
            isClickable = false;
            itemView.ApplyShadow();
            draggableObject._collider.enabled = false;
        }
        else if (_status == Status.invisible)
        {
            isClickable = false;
            itemView.HideView();
            draggableObject._collider.enabled = false;
        }
    }

    private void HandleSlotEnter(Slot slot)
    {
        if (slot != CurrentParentSlot)
        {
            if (slot.CheckAvailability())
            {
                if (CurrentParentSlot != null)
                    CurrentParentSlot.RemoveItem(this, position);

                slot.AddItem(this);
                CurrentParentSlot = slot;
                originalPosition = this.transform.position;
                // Restore the original sorting order when drag ends
                itemView.RestoreOriginalLayer();
                itemView.PlayDropAnimation();
            }
            else
            {
                itemView.RestoreOriginalLayer();
                draggableObject.ReturnToOriginalPosition(originalPosition);
            }
        }
        else
        {
            itemView.RestoreOriginalLayer();
            draggableObject.ReturnToOriginalPosition(originalPosition);
        }
    }

    private void HandleDragStart()
    {
        // Move to top sorting layer when drag starts
        itemView.MoveToTopLayer();
    }

    private void HandleDragEnd()
    {
        // Restore the original sorting order when drag ends
        itemView.RestoreOriginalLayer();
        draggableObject.ReturnToOriginalPosition(originalPosition);
    }
}

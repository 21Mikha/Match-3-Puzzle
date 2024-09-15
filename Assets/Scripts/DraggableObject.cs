using System;
using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    public event Action<Slot> OnSlotEnter;  // Event for entering a slot
    public event Action OnDragStart; // Event for starting drag
    public event Action OnDragEnd;  // Event for ending drag

    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;

    private float returnSpeed = 10f;
    public BoxCollider2D _collider;
    public Vector3 boxColliderOffset = new Vector3(0, 0.5f, 0);
    public LayerMask collisionLayer;

    private ItemController itemController;
    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        itemController = GetComponent<ItemController>();
    }

    void OnMouseDown()
    {
        if(itemController.isClickable)
        {
            zCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            offset = gameObject.transform.position - GetMouseWorldPos();
            isDragging = true;
        }

    }

    void OnMouseDrag()
    {
        if (itemController.isClickable)
        {
            if (isDragging)
            {
                transform.position = GetMouseWorldPos() + offset;
                OnDragStart?.Invoke();
            }
        }
    }

    void OnMouseUp()
    {
        if (itemController.isClickable)
        {
            isDragging = false;

            Collider2D hitCollider = Physics2D.OverlapBox(transform.position + boxColliderOffset, _collider.size, 0f, collisionLayer);

            if (hitCollider != null)
            {
                Slot slot = hitCollider.GetComponent<Slot>();
                OnSlotEnter?.Invoke(slot);  // Raise the slot enter event
            }
            else
            {
                OnDragEnd?.Invoke();
            }
        }
    }

    public void ReturnToOriginalPosition(Vector3 originalPosition)
    {
        StartCoroutine(SmoothReturn(originalPosition));
    }

    private System.Collections.IEnumerator SmoothReturn(Vector3 originalPosition)
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoordinate;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnDrawGizmosSelected()
    {
        if (_collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + boxColliderOffset, _collider.size);
        }
    }
}

using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float zCoordinate;
    private Vector3 originalPosition;
    private float returnSpeed = 10f;
    void OnMouseDown()
    {
        // Store the z-coordinate of the object
        zCoordinate = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Calculate the offset between the mouse position and the object position
        offset = gameObject.transform.position - GetMouseWorldPos();
        originalPosition=this.transform.position;
        // Set dragging to true
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the object's position to follow the mouse, accounting for the offset
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        // Stop dragging
        isDragging = false;
        StartCoroutine(SmoothReturn());
    }

    private Vector3 GetMouseWorldPos()
    {
        // Get the current mouse position in screen space
        Vector3 mousePoint = Input.mousePosition;

        // Add the z-coordinate to the mouse point to convert to world space
        mousePoint.z = zCoordinate;

        // Convert the mouse position to world coordinates
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private System.Collections.IEnumerator SmoothReturn()
    {
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPosition;  // Snap to the original position to avoid slight inaccuracies
    }
}

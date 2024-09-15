using UnityEngine;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Sprite[] itemSprites = new Sprite[20]; // 20 Products..
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private int originalSortingOrder;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSortingOrder = spriteRenderer.sortingOrder; // Store original sorting order

    }

    //This method triggers the drop animation
    public void PlayDropAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isDropped"); // Trigger the drop animation
        }
    }

    //Function to set the sprite based on the index number
    public void SetItemSprite(int index)
    {
        if (index >= 0 && index < itemSprites.Length)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = itemSprites[index];  // Set the sprite based on the index
            }
            else
            {
                Debug.LogWarning("SpriteRenderer component is missing on this object.");
            }
        }
        else
        {
            Debug.LogWarning("Index is out of bounds of the itemSprites array.");
        }
    }

    // Methods for handling sorting order
    public void MoveToTopLayer()
    {
        spriteRenderer.sortingOrder = 0;  // Move to the top layer
    }

    public void RestoreOriginalLayer()
    {
        spriteRenderer.sortingOrder = originalSortingOrder;  // Restore original sorting order
    }


    public void ApplyColor()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1f);
        spriteRenderer.sortingOrder = originalSortingOrder;  // Apply original sorting order
    }

    public void ApplyShadow()
    {
        spriteRenderer.color = new Color(0, 0, 0, 0.5f);
        spriteRenderer.sortingOrder = -3;
    }

    public void HideView()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0f);
        spriteRenderer.sortingOrder = -4;
    }
}

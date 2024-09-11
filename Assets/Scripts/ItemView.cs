using UnityEngine;
public class ItemView : MonoBehaviour
{
    [SerializeField] private Sprite[] itemSprites = new Sprite[20]; // 20 Products..
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // This method triggers the drop animation
    public void PlayDropAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("isDropped"); // Trigger the drop animation
        }

    }

    // Function to set the sprite based on the index number
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Spots { unAssigned,left, middle,right}
public enum SpecialEffect {normal,locked}

public class Slot : MonoBehaviour
{
    private const int capacity = 3;
    public int availableSpots;
    public event Action<Slot> OnMatch;



    //Anchor points -Used to find a suitable place for items to be placed in
    public Transform leftPoint;
    public Transform middlePoint;
    public Transform rightPoint;

    //Stacks to store items -First In Last Out
    private Stack<ItemController> leftStack=new Stack<ItemController>();
    private Stack<ItemController> middleStack = new Stack<ItemController>();
    private Stack<ItemController> rightStack = new Stack<ItemController>();


    void Awake()
    {
        leftPoint = transform.GetChild(0).gameObject.transform;
        middlePoint = transform.GetChild(1).gameObject.transform;
        rightPoint = transform.GetChild(2).gameObject.transform;
    }



    public void PopulateSlot(ItemController item, Spots spot)
    {

        if (spot == Spots.left)
        {
            item.position = Position.left;
            item.transform.position = leftPoint.position;
            leftStack.Push(item);
        }
        else if (spot == Spots.middle)
        {
            item.position = Position.middle;
            item.transform.position = middlePoint.position;
            middleStack.Push(item);
        }
        else if (spot == Spots.right)
        {
            item.position = Position.right;
            item.transform.position = rightPoint.position;
            rightStack.Push(item);
        }
        ReArrangeStack(leftStack);
        ReArrangeStack(middleStack);
        ReArrangeStack(rightStack);
        UpdateAvailability();
    }

    public void ReArrangeStack(Stack<ItemController> stack)
    {
        if (stack != null || stack.Count > 0)
        {
            //Convert stack to array to get access from top to bottom
            ItemController[] stackArray = stack.ToArray();

            for (int i = 0; i < stackArray.Length; i++)
            {
                if (i == 0)
                {
                    stackArray[i].UpdateStatus(Status.visible);
                }
                else if (i == 1)
                {
                    stackArray[i].UpdateStatus(Status.shadowed);
                }
                else
                {
                    stackArray[i].UpdateStatus(Status.invisible);
                }
            }
        }
    }



    public bool CheckAvailability()

    {
        if (availableSpots > 0)
        {
            return true;
        }
        else { return false; }
    }

    private void UpdateAvailability()
    {
        // Reset availableSpots to the maximum capacity (3)
        availableSpots = capacity;

        //Check each stack and reduce availableSpots accordingly
        if (leftStack.Count > 0 && leftStack.Peek().status == Status.visible)
        {
            availableSpots--;
        }

        if (middleStack.Count > 0 && middleStack.Peek().status == Status.visible)
        {
            availableSpots--;
        }

        if (rightStack.Count > 0 && rightStack.Peek().status == Status.visible)
        {
            availableSpots--;
        }

        //Ensure that availableSpots does not go below 0
        availableSpots = Mathf.Max(availableSpots, 0);
    }

    //AddItem is called by the draggable object when collision happens with the slot to check if adding is possible (if so) add it
    public void AddItem(ItemController item)
    {
        Spots availableSpot = GetAvailableSpot();

        if(availableSpot== Spots.left)
        {
            item.position = Position.left;
            item.transform.position = leftPoint.position;
            leftStack.Push(item);
            CheckForMatch();
            UpdateAvailability();
        }
        else if(availableSpot== Spots.middle)
        {
            item.position = Position.middle;
            item.transform.position = middlePoint.position;
            middleStack.Push(item);
            CheckForMatch();
            UpdateAvailability();
        }
        else if (availableSpot == Spots.right)
        {
            item.position = Position.right;
            item.transform.position = rightPoint.position;
            rightStack.Push(item);
            CheckForMatch();
            UpdateAvailability();
        }
        else
        {
            Debug.LogError("Error Adding an item, Unhadled situation");
        }
    }


    //RemoveItem is called by the draggable object when it gets added to a new slot to notify previous slot to remove it
    public void RemoveItem(ItemController item,Position pos)
    {
        if(pos==Position.left)
        {
            if (leftStack.Count > 0)
                leftStack.Pop();
            UpdateAvailability();
        }
        else if (pos==Position.middle)
        {
            if (middleStack.Count > 0)
            middleStack.Pop();
            UpdateAvailability();
        }
        else if(pos == Position.right)
        {
            if (rightStack.Count > 0)
            rightStack.Pop();
            UpdateAvailability();
        }
        else
        {
            Debug.LogError("Error removing an item, Unhadled situation");
        }
        CheckIfTopLayerIsEmpty();
    }
    public Spots GetAvailableSpot()
    {
        //Check left stack first
        if (leftStack.Count > 0)
        {
            if (leftStack.Peek().status == Status.shadowed)
            {
                return Spots.left;
            }
        }
        else
        {
            return Spots.left;
        }

        //Check middle stack
        if (middleStack.Count > 0)
        {
            if (middleStack.Peek().status == Status.shadowed)
            {
                return Spots.middle;
            }
        }
        else
        {
            return Spots.middle;
        }

        //Check right stack
        if (rightStack.Count > 0)
        {
            if (rightStack.Peek().status == Status.shadowed)
            {
                return Spots.right;
            }
        }
        else
        {
            return Spots.right;
        }

        Debug.LogError("Error finding available spot");
        return Spots.unAssigned;
    }

    private void CheckIfTopLayerIsEmpty()
    {
        if((leftStack.Count == 0 || leftStack.Peek().status == Status.shadowed) && (middleStack.Count == 0 || middleStack.Peek().status == Status.shadowed) && (rightStack.Count == 0 || rightStack.Peek().status == Status.shadowed))
        {
            ReArrangeStack(leftStack);
            ReArrangeStack(middleStack);
            ReArrangeStack(rightStack);
            UpdateAvailability();
        }

    }


    private void CheckForMatch()
    {
        if (leftStack.Count>0&& middleStack.Count > 0&& rightStack.Count > 0)
        {
            if(leftStack.Peek().status==Status.visible && middleStack.Peek().status == Status.visible && rightStack.Peek().status == Status.visible)
            {
                if (leftStack.Peek().id == middleStack.Peek().id && middleStack.Peek().id == rightStack.Peek().id)
                {
                    HandleMatch(leftStack.Peek(), middleStack.Peek(), rightStack.Peek());
                }
            }
        }
    }
    private void HandleMatch(ItemController a, ItemController b, ItemController c)
    {
        leftStack.Pop();
        middleStack.Pop();
        rightStack.Pop();
        a.itemView.PlayDropAnimation();
        b.itemView.PlayDropAnimation();
        c.itemView.PlayDropAnimation();
        //destroy the objects after a delay
        StartCoroutine(DestroyWithDelay(a, b, c, 0.3f));
    }

    private System.Collections.IEnumerator DestroyWithDelay(ItemController a, ItemController b, ItemController c, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Destroy the objects after the delay
        Destroy(a.gameObject);
        Destroy(b.gameObject);
        Destroy(c.gameObject);
        OnMatch?.Invoke(this);

        ReArrangeStack(leftStack);
        ReArrangeStack(middleStack);
        ReArrangeStack(rightStack);
        UpdateAvailability();

    }


}

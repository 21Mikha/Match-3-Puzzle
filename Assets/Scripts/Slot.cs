using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Spots { unAssigned,left, middle,right}
public enum SpecialEffect {normal,locked}

public class Slot : MonoBehaviour
{
    private const int capacity = 3;
    private int availableSpots = 3;




    //Anchor points -Used to find a suitable place for items to be placed in
    public Transform leftPoint;
    public Transform middlePoint;
    public Transform rightPoint;

    //Stacks to store items -First In Last Out
    private Stack<DraggableObject> leftStack=new Stack<DraggableObject>();
    private Stack<DraggableObject> middleStack = new Stack<DraggableObject>();
    private Stack<DraggableObject> rightStack = new Stack<DraggableObject>();

    void Start()
    {
        leftPoint = transform.GetChild(0).gameObject.transform;
        middlePoint = transform.GetChild(1).gameObject.transform;
        rightPoint = transform.GetChild(2).gameObject.transform;
    }



    public void PopulateSlot(DraggableObject item, Spots spot)
    {

        if (spot == Spots.left)
        {
            item.position = Position.left;
            item.transform.position = leftPoint.position;
            leftStack.Push(item);

            if (leftStack.Count > 0)
            {
                item.status = Status.shadowed;
            }
        }
        else if (spot == Spots.middle)
        {
            item.position = Position.middle;
            item.transform.position = middlePoint.position;
            middleStack.Push(item);

            if (middleStack.Count > 0)
            {
                item.status = Status.shadowed;
            }
        }
        else if (spot == Spots.right)
        {
            item.position = Position.right;
            item.transform.position = rightPoint.position;
            rightStack.Push(item);

            if(rightStack.Count > 0)
            {
                item.status = Status.shadowed;
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


    //AddItem is called by the draggable object when collision happens with the slot to check if adding is possible and add it
    public void AddItem(DraggableObject item)
    {
        Spots availableSpot = GetAvailableSpot();

        if(availableSpot== Spots.left)
        {
            item.position = Position.left;
            item.transform.position = leftPoint.position;
            leftStack.Push(item);
        }
        else if(availableSpot== Spots.middle)
        {
            item.position = Position.middle;
            item.transform.position = middlePoint.position;
            middleStack.Push(item);
        }
        else if (availableSpot == Spots.right)
        {
            item.position = Position.right;
            item.transform.position = rightPoint.position;
            rightStack.Push(item);
        }
        else
        {
            Debug.LogError("Error Adding an item, Unhadled situation");
        }
    }


    //RemoveItem is called by the draggable object when it gets added to a new slot to notify previous slot to delete it
    public void RemoveItem(DraggableObject item,Position pos)
    {
        if(pos==Position.left)
        {
            availableSpots++;
            if (leftStack.Count > 0)
                leftStack.Pop();
        }
        else if (pos==Position.middle)
        {
            availableSpots++;
            if (middleStack.Count > 0)
            middleStack.Pop();
        }
        else if(pos == Position.right)
        {
            availableSpots++;
            if (rightStack.Count > 0)
            rightStack.Pop();
        }
        else
        {
            Debug.LogError("Error removing an item, Unhadled situation");
        }
    }
    public Spots GetAvailableSpot()
    {
        //Check left stack first
        if (leftStack.Count > 0)
        {
            if (leftStack.Peek().status == Status.shadowed)
            {
                availableSpots--;
                return Spots.left;
            }
        }
        else
        {
            availableSpots--;
            return Spots.left;
        }

        //Check middle stack
        if (middleStack.Count > 0)
        {
            if (middleStack.Peek().status == Status.shadowed)
            {
                availableSpots--;
                return Spots.middle;
            }
        }
        else
        {
            availableSpots--;
            return Spots.middle;
        }

        //Check right stack
        if (rightStack.Count > 0)
        {
            if (rightStack.Peek().status == Status.shadowed)
            {
                availableSpots--;
                return Spots.right;
            }
        }
        else
        {
            availableSpots--;
            return Spots.right;
        }

        Debug.LogError("Error finding available spot");
        return Spots.unAssigned;
    }


}

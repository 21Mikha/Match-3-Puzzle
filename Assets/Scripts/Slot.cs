using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Spot {empty,occupied}
public class Slot : MonoBehaviour
{
    private const int capacity = 3;
    private int availableSpots = 3;

    public Transform leftPoint;
    public Transform middlePoint;
    public Transform rightPoint;

    private Spot[] spots=new Spot[3];
    void Start()
    {
        leftPoint = transform.GetChild(0).gameObject.transform;
        middlePoint = transform.GetChild(1).gameObject.transform;
        rightPoint = transform.GetChild(2).gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CheckAvailability()
    {
        if (availableSpots > 0)
        {
            return true;
        }
        else { return false; }
    }
    public void DropItem(DraggableObject item)
    {
       item.transform.position = GetAvailableSpot().position;
    }

    public Transform GetAvailableSpot()
    {
        if (spots[0]==Spot.empty)
        {
            spots[0] = Spot.occupied;
            availableSpots--;
            return leftPoint;
        }
        else if (spots[1] == Spot.empty)
        {
            spots[1] = Spot.occupied;
            availableSpots--;
            return middlePoint;
        }
        else
        {
            spots[2] = Spot.occupied;
            availableSpots--;
            return rightPoint;
        }
    }
    public void FreeSpot()
    {
        availableSpots++;
    }

}

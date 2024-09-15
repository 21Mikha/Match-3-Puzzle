using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Level levelData;

    //Method to be called by LevelManager to set level data
    public void SetLevelData(string json, GameObject slotPrefab, GameObject itemPrefab)
    {
        //Parse the JSON into the Level class
        levelData = JsonUtility.FromJson<Level>(json);
    }

    //Generate the level based on the parsed JSON data
    public GameObject GenerateLevel(GameObject slotPrefab, GameObject itemPrefab)
    {
        if (levelData == null)
        {
            Debug.LogError("Level data is null, cannot generate level!");
            return null;
        }

        //Create a parent object called "Level_X" where X is the level number
        string levelParentName = "Level_" + levelData.level;
        GameObject levelParent = new GameObject(levelParentName);
        GameObject ItemsParent = new GameObject("Items");
        ItemsParent.transform.SetParent(levelParent.transform);

        int slotCounter = 1;
        //Instantiate the slots
        foreach (var slot in levelData.slots)
        {
            
            //Convert position data to a Vector3 and instantiate the slot
            Vector3 position = new Vector3(slot.position.x, slot.position.y, slot.position.z);
            Slot newSlot = Instantiate(slotPrefab, position, Quaternion.identity).GetComponent<Slot>();
            //Name the slot dynamically ((for debugging only))
            newSlot.gameObject.name = "Slot " + slotCounter;

            //Increment the slot counter for the next slot
            slotCounter++;
            newSlot.transform.SetParent(levelParent.transform);


            foreach (var itemID in slot.places.left)
            {
                ItemController newItem = Instantiate(itemPrefab, position, Quaternion.identity).GetComponent<ItemController>();
                newSlot.PopulateSlot(newItem, Spots.left);
                newItem.InitializeItem(itemID, newSlot);
                newItem.transform.SetParent(ItemsParent.transform);
            }
            foreach (var itemID in slot.places.middle)
            {
                ItemController newItem = Instantiate(itemPrefab, position, Quaternion.identity).GetComponent<ItemController>();

                newSlot.PopulateSlot(newItem, Spots.middle);
                newItem.InitializeItem(itemID, newSlot);
                newItem.transform.SetParent(ItemsParent.transform);

            }
            foreach (var itemID in slot.places.right)
            {
                ItemController newItem = Instantiate(itemPrefab, position, Quaternion.identity).GetComponent<ItemController>();

                newSlot.PopulateSlot(newItem, Spots.right);
                newItem.InitializeItem(itemID, newSlot);
                newItem.transform.SetParent(ItemsParent.transform);

            }

        }


        return levelParent;
    }
}

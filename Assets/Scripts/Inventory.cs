using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks.Ugc;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemSlot[] items;
    public ItemSlot getItemStack(ItemType itemType) {
        return items[(int)itemType];
    }
    public MainMenu canvas;

    // Start is called before the first frame update
    public void ChangeItem(int itemIndex, int itemDeltaAmount) {
        if (itemDeltaAmount > 0) {
            items[itemIndex].amount += itemDeltaAmount;
        }
        else if (itemDeltaAmount < 0) {
            if (items[itemIndex].amount - itemDeltaAmount < 0) {
                Debug.Log("Not Enough Items");
            }
            else {
                items[itemIndex].amount += itemDeltaAmount;
            }
        }
    }
    public void ChangeItem(ItemSlot item) {
        ItemSlot inventoryItem = items[(int)item.type];
        if (item.amount > 0) {
            inventoryItem.amount += item.amount;
        }
        else if (item.amount < 0) {
            if (item.amount - item.amount < 0) {
                Debug.Log("Not Enough Items");
            }
            else {
                inventoryItem.amount += item.amount;
            }
        }
    }
    public bool CheckHasEnough(ItemSlot[] requiredItems) {
        List<string> missingList = new List<string>();
        bool valid = true;
        foreach (ItemSlot reqiuredItem in requiredItems) {
            if (reqiuredItem != null)
            {
                bool notEnoughItems = reqiuredItem.amount > items[(int)reqiuredItem.type].amount;
                if (notEnoughItems) {
                    int missingAmount = reqiuredItem.amount-items[(int)reqiuredItem.type].amount;
                    if (missingAmount >0) {
                        missingList.Add(reqiuredItem.type.ToString()+" "+missingAmount+"x");
                    }
                    valid = false;
                }
            }
        }
        if (!valid) {
            StartCoroutine(canvas.showNotification("You're missing: "+String.Join(",",missingList),3));
        }
        return valid;
    }
}

[Serializable]
public class ItemSlot {
    public int amount;
    public ItemType type;
    public ItemSlot(ItemType itemType, int amount) {
        this.type = itemType;
        this.amount = amount;
    }
}

public enum ItemType {
    Wood,
    Sapling,
    Stone,
    Lenght
}
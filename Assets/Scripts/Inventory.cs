using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemSlot[] items;

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
}

[Serializable]
public class ItemSlot {
    public int amount;
    public string name;
}

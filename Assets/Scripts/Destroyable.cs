using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public Tool preferedTool;
    [SerializeField]
    private float Health = 10f;
    public Drop[] drops;
    public void Damage(float damage, Tool preferedTool, Inventory inventory) {
        if (preferedTool == this.preferedTool) {
            Health-=damage;
            if (Health <= 0) {
                Destroy(gameObject);
                foreach(Drop drop in drops) {
                    inventory.ChangeItem(drop.GetItemSlot());
                }
            }
        }
    }
    public bool CheckCanBuild(Inventory inventory) {
        ItemSlot[] itemSlots = new ItemSlot[(int)ItemType.Lenght];
        foreach (Drop drop in drops) {
            itemSlots[(int)drop.itemType] = drop.GetItemSlot();
        }
        return inventory.CheckHasEnough(itemSlots);
    }

    public void removeItems(Inventory inventory) {
        foreach (Drop drop in drops) {
            ItemSlot itemSlot = drop.GetItemSlot();
            itemSlot.amount = - itemSlot.amount;
            inventory.ChangeItem(itemSlot);
        }

    }
}

[Serializable]
public class Drop {
    public ItemType itemType;
    public int minDrop;
    public int maxDrop;
    public ItemSlot GetItemSlot() {
        return new ItemSlot(itemType, UnityEngine.Random.Range(minDrop,maxDrop));
    }
    public Drop(ItemType itemType, int minDrop, int maxDrop) {
        this.itemType = itemType;
        this.minDrop = minDrop;
        this.maxDrop = maxDrop;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : Destroyable
{
    public BoxCollider boxCollider;
    public float gridSize;

    public BuildableType type;
    public bool CheckCanBuild(Inventory inventory)
    {
        ItemSlot[] itemSlots = new ItemSlot[(int)ItemType.Lenght];
        foreach (Drop drop in drops)
        {
            itemSlots[(int)drop.itemType] = drop.GetItemSlot();
        }
        return inventory.CheckHasEnough(itemSlots);
    }

    public virtual void RemoveItems(Inventory inventory)
    {
        foreach (Drop drop in drops)
        {
            ItemSlot itemSlot = drop.GetItemSlot();
            itemSlot.amount = -itemSlot.amount;
            inventory.ChangeItem(itemSlot);
        }
    }

    public virtual void setBuildingPosition(GameObject blueprint, RaycastHit raycastHit, Quaternion rotationOffset)
    {
        Vector3 translatedSize = Math.multiplyVector3(boxCollider.size, blueprint.transform.localScale);
        Vector3 startPos = new Vector3(Mathf.Round(raycastHit.point.x / gridSize) * gridSize, raycastHit.point.y, Mathf.Round(raycastHit.point.z / gridSize) * gridSize);
        blueprint.transform.position = FindLowestSafePosition(startPos, rotationOffset, translatedSize, 200f);
        blueprint.transform.rotation = rotationOffset;
    }

    public Vector3 FindLowestSafePosition(Vector3 startPosition, Quaternion startRotation, Vector3 boxSize, float maxCheckDistance)
    {
        float maxY = startPosition.y + maxCheckDistance;
        Vector3 currentPosition = startPosition;

        while (currentPosition.y < maxY)
        {
            if (!Physics.CheckBox(currentPosition, boxSize / 2, startRotation))
            {
                return currentPosition;
            }
            else
            {
                currentPosition.y += 0.01f;
            }

        }

        Debug.LogError("Not found Lowest safe position");
        return startPosition;
    }
}

public enum BuildableType
{
    Foundation,
    Wall,
    Crafting_Table,
}
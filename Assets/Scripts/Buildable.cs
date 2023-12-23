using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildable : Destroyable
{
    public BoxCollider boxCollider;
    public float gridSize;

    public BuildableType type;

    protected bool isBlueprint = true;

    public virtual CanBeBuilt CheckCanBuild(Inventory inventory)
    {
        ItemSlot[] itemSlots = new ItemSlot[(int)ItemType.Lenght];
        foreach (Drop drop in drops)
        {
            itemSlots[(int)drop.itemType] = drop.GetItemSlot();
        }
        if (inventory.CheckHasEnough(itemSlots))
        {
            return CanBeBuilt.YES;
        }
        else
        {
            return CanBeBuilt.NOT_ENOUGH_ITEMS;
        }
    }

    public virtual void RemoveItems(Inventory inventory)
    {
        foreach (Drop drop in drops)
        {
            ItemSlot itemSlot = drop.GetItemSlot();
            itemSlot.amount = -itemSlot.amount;
            inventory.ChangeItem(itemSlot);
        }
        isBlueprint = false;
    }

    public virtual void setBuildingPosition(GameObject blueprint, RaycastHit raycastHit)
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(blueprint.transform.rotation.eulerAngles);
        if (scrollInput != 0)
        {
            blueprint.transform.rotation *= Quaternion.Euler(0, 90 * Mathf.Sign(scrollInput), 0);
        }
        Vector3 translatedSize = Math.multiplyVector3(boxCollider.size, blueprint.transform.localScale);
        Vector3 startPos = new Vector3(Mathf.Round(raycastHit.point.x / gridSize) * gridSize, raycastHit.point.y, Mathf.Round(raycastHit.point.z / gridSize) * gridSize);
        blueprint.transform.position = FindLowestSafePosition(startPos, blueprint.transform.rotation, translatedSize, 200f);
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
    Ramp
}

public enum CanBeBuilt
{
    YES,
    NOT_ENOUGH_ITEMS,
    OCCUPIED
}
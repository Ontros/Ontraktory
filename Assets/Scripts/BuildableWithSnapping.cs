using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableWithSnapping : Buildable
{
    SnappingPosition buildOnSnappingPosition;
    public override void setBuildingPosition(GameObject blueprint, RaycastHit raycastHit, Quaternion rotationOffset)
    {
        Buildable colliderBuildale = raycastHit.collider.GetComponent<Buildable>();
        if (colliderBuildale == null)
        {
            base.setBuildingPosition(blueprint, raycastHit, rotationOffset);
            return;
        }
        SnappingPosition[] snappingPositions = colliderBuildale.GetComponentsInChildren<SnappingPosition>();
        List<SnappingPosition> snappingPositionsOfThisType = new List<SnappingPosition>();
        foreach (SnappingPosition snappingPosition in snappingPositions)
        {
            if (snappingPosition.buildableType == type)
            {
                snappingPositionsOfThisType.Add(snappingPosition);
            }
        }
        SnappingPosition closestSnappingPostion = null;
        float currentDistance = Mathf.Infinity;
        foreach (SnappingPosition snappingPositionOfThisType in snappingPositionsOfThisType)
        {
            float foundDistance = Vector3.Distance(snappingPositionOfThisType.transform.position, raycastHit.point);
            if (foundDistance < currentDistance && !snappingPositionOfThisType.isOccupied)
            {
                currentDistance = foundDistance;
                closestSnappingPostion = snappingPositionOfThisType;
            }
        }
        if (closestSnappingPostion != null)
        {
            buildOnSnappingPosition = closestSnappingPostion;
            blueprint.transform.position = closestSnappingPostion.transform.position + closestSnappingPostion.offet;
            blueprint.transform.rotation = closestSnappingPostion.transform.rotation;
        }
        else
        {
            base.setBuildingPosition(blueprint, raycastHit, rotationOffset);
            return;
        }
        // Vector3 translatedSize = Math.multiplyVector3(boxCollider.size, blueprint.transform.localScale);
        // Vector3 startPos = new Vector3(Mathf.Round(raycastHit.point.x / gridSize) * gridSize, raycastHit.point.y, Mathf.Round(raycastHit.point.z / gridSize) * gridSize);
        // return FindLowestSafePosition(startPos, rotationOffset, translatedSize, 200f);
    }
    public override void RemoveItems(Inventory inventory)
    {
        if (buildOnSnappingPosition != null)
        {
            buildOnSnappingPosition.isOccupied = true;
        }
        base.RemoveItems(inventory);
    }

    void OnDestroy()
    {
        if (buildOnSnappingPosition != null)
        {
            buildOnSnappingPosition.isOccupied = false;
        }
    }
}

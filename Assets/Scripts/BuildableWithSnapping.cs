using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableWithSnapping : Buildable
{
    SnappingPosition buildOnSnappingPosition;
    public override void setBuildingPosition(GameObject blueprint, RaycastHit raycastHit)
    {
        Buildable colliderBuildale = raycastHit.collider.GetComponent<Buildable>();
        if (colliderBuildale == null)
        {
            base.setBuildingPosition(blueprint, raycastHit);
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
            if (foundDistance < currentDistance)
            {
                currentDistance = foundDistance;
                closestSnappingPostion = snappingPositionOfThisType;
            }
        }
        if (closestSnappingPostion != null)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0)
            {
                closestSnappingPostion.currentRotationIndex = (closestSnappingPostion.currentRotationIndex + closestSnappingPostion.possibleRototations.Length + (int)Mathf.Sign(scrollInput)) % closestSnappingPostion.possibleRototations.Length;
            }
            blueprint.transform.rotation = closestSnappingPostion.transform.rotation * Quaternion.AngleAxis(closestSnappingPostion.possibleRototations[closestSnappingPostion.currentRotationIndex], Vector3.up);
            blueprint.transform.position = closestSnappingPostion.transform.position + blueprint.transform.forward * closestSnappingPostion.xOffset + closestSnappingPostion.transform.up * closestSnappingPostion.yOffset + blueprint.transform.right * closestSnappingPostion.zOffset;
            buildOnSnappingPosition = closestSnappingPostion;
            // if (closestSnappingPostion.forceRotation)
            // {
            //     blueprint.transform.rotation = closestSnappingPostion.transform.rotation;
            //     blueprint.transform.position = closestSnappingPostion.transform.position + closestSnappingPostion.offet;
            // }
            // else
            // {
            //     blueprint.transform.rotation = rotationOffset;
            //     closestSnappingPostion.transform.rotation = rotationOffset;
            //     blueprint.transform.position = closestSnappingPostion.transform.position + closestSnappingPostion.transform.forward * closestSnappingPostion.offet.x;
            // }
        }
        else
        {
            base.setBuildingPosition(blueprint, raycastHit);
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

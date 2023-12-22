using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableWithSnapping : Buildable
{
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
            closestSnappingPostion.rotateMouseWheel(Input.GetAxis("Mouse ScrollWheel"));
            blueprint.transform.rotation = closestSnappingPostion.transform.rotation * Quaternion.AngleAxis(closestSnappingPostion.possibleRototations[closestSnappingPostion.currentRotationIndex], Vector3.up);
            blueprint.transform.position = closestSnappingPostion.transform.position + blueprint.transform.forward * closestSnappingPostion.xOffset + closestSnappingPostion.transform.up * closestSnappingPostion.yOffset + blueprint.transform.right * closestSnappingPostion.zOffset;
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


    public bool isOccupied()
    {
        foreach (BuildableWithSnapping buildableWithSnapping in FindObjectsOfType<BuildableWithSnapping>())
        {
            // Debug.Log(Vector3.Distance(transform.position, buildableWithSnapping.transform.position);
            if (buildableWithSnapping.type == type && Vector3.Distance(transform.position, buildableWithSnapping.transform.position) < 0.1 && !buildableWithSnapping.isBlueprint)
            {
                return true;
            }
        }
        return false;
    }

    //OnBuild
    public override void RemoveItems(Inventory inventory)
    {
        base.RemoveItems(inventory);
    }

    public override CanBeBuilt CheckCanBuild(Inventory inventory)
    {
        if (transform.position != Vector3.zero && isOccupied())
        {
            Debug.Log("Occupied");
            return CanBeBuilt.OCCUPIED;
        }
        return base.CheckCanBuild(inventory);
    }
}

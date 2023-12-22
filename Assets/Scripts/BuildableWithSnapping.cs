using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableWithSnapping : Buildable
{
    SnappingPosition foreignSnappingPosition;
    SnappingPosition localSnappingPosition;
    public override void setBuildingPosition(GameObject blueprint, RaycastHit raycastHit)
    {
        Buildable colliderBuildale = raycastHit.collider.GetComponent<Buildable>();
        if (colliderBuildale == null)
        {
            foreignSnappingPosition = null;
            localSnappingPosition = null;
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
            if (foundDistance < currentDistance && snappingPositionOfThisType.hasAvailableRotations())
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
            foreignSnappingPosition = closestSnappingPostion;

            //Get local snapping position
            SnappingPosition closestLocalSnappingPosition = null;
            float closestLocalSnappingPositionDistance = Mathf.Infinity;
            foreach (SnappingPosition localSnappingPosition in blueprint.GetComponentsInChildren<SnappingPosition>())
            {
                if (localSnappingPosition.buildableType == colliderBuildale.type)
                {
                    float foundDistance = Vector3.Distance(localSnappingPosition.transform.position, foreignSnappingPosition.transform.position);
                    if (foundDistance < closestLocalSnappingPositionDistance)
                    {
                        closestLocalSnappingPositionDistance = foundDistance;
                        closestLocalSnappingPosition = localSnappingPosition;
                    }
                }
            }
            if (closestLocalSnappingPosition != null)
            {
                localSnappingPosition = closestLocalSnappingPosition;
            }
            else
            {
                Debug.LogError("Not found closest local snapping position");
            }

        }
        else
        {
            foreignSnappingPosition = null;
            localSnappingPosition = null;
            base.setBuildingPosition(blueprint, raycastHit);
            return;
        }
        // Vector3 translatedSize = Math.multiplyVector3(boxCollider.size, blueprint.transform.localScale);
        // Vector3 startPos = new Vector3(Mathf.Round(raycastHit.point.x / gridSize) * gridSize, raycastHit.point.y, Mathf.Round(raycastHit.point.z / gridSize) * gridSize);
        // return FindLowestSafePosition(startPos, rotationOffset, translatedSize, 200f);
    }
    //OnBuild
    public override void RemoveItems(Inventory inventory)
    {
        if (foreignSnappingPosition != null)
        {
            foreignSnappingPosition.isOccupied[foreignSnappingPosition.currentRotationIndex] = true;
            localSnappingPosition.dependandSnappingPositions.Add(new DependantSnappingPosition(foreignSnappingPosition, foreignSnappingPosition.currentRotationIndex));
            localSnappingPosition.isOccupied[0] = true;
            foreignSnappingPosition.dependandSnappingPositions.Add(new DependantSnappingPosition(localSnappingPosition, 0));
        }
        base.RemoveItems(inventory);
    }

    void OnDestroy()
    {
        if (foreignSnappingPosition != null)
        {
            foreach (DependantSnappingPosition dependantSnappingPosition in foreignSnappingPosition.dependandSnappingPositions)
            {
                dependantSnappingPosition.snappingPosition.isOccupied[dependantSnappingPosition.rotationIndex] = false;
            }
        }
    }
}

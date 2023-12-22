using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnappingPosition : MonoBehaviour
{
    public BuildableType buildableType;

    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;

    public float[] possibleRototations;

    public bool[] isOccupied;
    public int currentRotationIndex = 0;

    public List<DependantSnappingPosition> dependandSnappingPositions = new List<DependantSnappingPosition>();
    public bool buildableHasOpposite = true;

    public void Start()
    {
        isOccupied = new bool[possibleRototations.Length];
    }

    public List<float> getPossibleRotations()
    {
        List<float> output = new List<float>();
        int index = 0;
        foreach (float rotation in possibleRototations)
        {
            if (!isOccupied[index])
            {
                output.Add(rotation);
            }
            index++;
        }
        return output;
    }

    public bool hasAvailableRotations()
    {
        foreach (bool current in isOccupied)
        {
            if (!current)
            {
                return true;
            }
        }
        return false;
    }

    public void rotateMouseWheel(float mouseScroll)
    {
        //TODO: 404 optimization not found
        if (mouseScroll != 0)
        {
            currentRotationIndex = (currentRotationIndex + possibleRototations.Length + (int)Mathf.Sign(mouseScroll)) % possibleRototations.Length;
            while (isOccupied[currentRotationIndex])
            {
                currentRotationIndex = (currentRotationIndex + 1) % possibleRototations.Length;
            }
        }
    }
}

[Serializable]
public class DependantSnappingPosition
{
    public SnappingPosition snappingPosition;
    public int rotationIndex;
    public DependantSnappingPosition(SnappingPosition snappingPosition, int rotationIndex)
    {
        this.snappingPosition = snappingPosition;
        this.rotationIndex = rotationIndex;
    }
}
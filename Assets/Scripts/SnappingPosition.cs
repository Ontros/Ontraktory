using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingPosition : MonoBehaviour
{
    public BuildableType buildableType;
    public bool isOccupied = false;

    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;

    public float[] possibleRototations;

    public int currentRotationIndex = 0;
}

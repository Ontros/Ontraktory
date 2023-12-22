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

    public int currentRotationIndex = 0;
    public void rotateMouseWheel(float mouseScroll)
    {
        if (mouseScroll != 0)
        {
            currentRotationIndex = (currentRotationIndex + possibleRototations.Length + (int)Mathf.Sign(mouseScroll)) % possibleRototations.Length;
        }
    }
}
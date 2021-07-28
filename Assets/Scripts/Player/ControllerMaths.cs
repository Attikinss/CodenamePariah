using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class ControllerMaths
{
    public static float CalculateJumpForce(float wantedHeight, float weight, float g)
    {
        float jumpForceRequired = Mathf.Sqrt(-2.0f * g * wantedHeight);

        return jumpForceRequired;
        
    }
}


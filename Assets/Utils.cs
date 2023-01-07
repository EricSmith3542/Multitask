using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float MapFloat(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin)*(toMax - toMin)/(fromMax-fromMin);
    }

    public static float Logerp(float a, float b, float t)
    {
        return a * Mathf.Pow(b / a, t);
    }
}

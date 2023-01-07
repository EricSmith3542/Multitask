using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float MapFloat(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin)*(toMax - toMin)/(fromMax-fromMin);
    }

    public static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T val = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = val;
        }
    }
}

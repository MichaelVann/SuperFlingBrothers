using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class VLib
{
    public static float TruncateFloatsDecimalPlaces(float a_float, int a_decimalsToKeep)
    {
        float multiplier = Mathf.Pow(10, a_decimalsToKeep);
        float truncatedFloat = ((int)(a_float * multiplier) / multiplier);

        return truncatedFloat;
    }

    public static int SafeMod(int a_value, int a_mod)
    {
        return (a_value % a_mod + a_mod) % a_mod;
    }
}
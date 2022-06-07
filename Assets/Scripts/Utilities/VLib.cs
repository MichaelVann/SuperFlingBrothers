using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class VLib
{
    static string[] m_consonants = { "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z", "ch", "kh", "gh" };
    static string[] m_vowels = { "a", "e", "i", "o", "u", "oo", "ou", "ee" };

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

    public static int vRandom(int a_inclusiveMin, int a_exclusiveMax)
    {
        return UnityEngine.Random.Range(a_inclusiveMin, a_exclusiveMax);
    }

    public static float vRandom(float a_inclusiveMin, float a_inclusiveMax)
    {
        return UnityEngine.Random.Range(a_inclusiveMin, a_inclusiveMax);
    }

    public static Color PercentageToColor(float a_percentage)
    {
        Color returnColor = Color.white;

        float redRatio = Mathf.Clamp(2f - (2f * a_percentage), 0f, 1f);
        float greenRatio = Mathf.Clamp(5f * (a_percentage) - 1.5f, 0f, 1f);
        returnColor = new Color(redRatio, greenRatio, 0f);
        return returnColor;
    }

    public static string GenerateRandomizedName()
    {
        string name = "";
        int length = vRandom(2, 10);
        int startWithVowel = vRandom(0, 2);

        for (int i = 0; i < length; i++)
        {

            if (i % 2 == startWithVowel)
            {
                name += m_consonants[vRandom(0, m_consonants.Length)];
            }
            else
            {
                name += m_vowels[vRandom(0, m_vowels.Length)];
            }
            if (i == 0)
            {
                string tempString = name;
                tempString = name[0].ToString().ToUpper();
                for (int j = 1; j < name.Length; j++)
                {
                    tempString += name[i];
                }
                name = tempString;
            }
        }
        return name;
    }

    public static string[] GenerateRandomPersonsName()
    {
        string[] generatedNames = new string[2];

        string[] firstNames =
        {
            "Andrew",
            "Bart",
            "Ben",
            "John",
            "Stuart",
            "Giles",
            "Carnaby",
            "Callum",
            "Sean",
            "Will",
            "Micky",
            "Jack",
            "Tyler",
            "Israq",
            "Daniel",
            "Stephen",
            "Richard",
            "Oliver",
            "Archie",

            "Eloise",
            "Mikenna",
            "Molly",
            "Emma",
            "Lavinia",
            "Sandra",
            "Mary",
            "Julia",
            "Tina",
            "Penelope",
            "Lucy",
            "Becky",
            "Ruth",
            "Calli",
            "Aditi",
            "Molly",
            "Emma",
            "Naomi",
            "Zara",
        };

        string[] lastNames =
        {
            "Digby",
            "Webster-Hawes",
            "Constant",
            "Vann",
            "Ison",
            "Hansen",
            "Gilani",
            "Dhrubo",
            "Modra",
            "Head",
            "Hensgen",
            "Basnet",
            "Shoebridge",
            "Porter",
            "Foster",
            "Erieau",
            "Gordon-Anderson",
            "Taylor",
            "Waters"
        };

        generatedNames[0] = firstNames[UnityEngine.Random.Range(0, firstNames.Length)];
        generatedNames[1] = lastNames[UnityEngine.Random.Range(0, lastNames.Length)];

        return generatedNames;
    }
}
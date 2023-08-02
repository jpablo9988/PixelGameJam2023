using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings 
{
    public static bool FoundGuilty = false;
    public static bool EnoughEvidence = false;
    public static float Volume = 0.85f;
    public static Character accusedCharacter;
    public static void ResetStatsGame()
    {
        FoundGuilty = false;
        EnoughEvidence = false;
        accusedCharacter = null;
    }
}

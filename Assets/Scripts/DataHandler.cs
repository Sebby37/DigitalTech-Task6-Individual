using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Games
{
    public static string Sanitisation = "Sanitisation";
    public static string Mask = "Mask";
    public static string KeepTheDistance = "Keep The Distance";
}

public class DataHandler
{
    // Function to set the score of an entry
    public static void SetScore(string entry, int newScore)
    {
        PlayerPrefs.SetInt(entry, newScore);
    }

    // Function to get the score of an entry
    public static int GetScore(string entry)
    {
        if (!PlayerPrefs.HasKey(entry))
            SetScore(entry, 0);

        return PlayerPrefs.GetInt(entry);
    }

    // Function to reset the save data
    public static void ResetData()
    {
        PlayerPrefs.DeleteAll();
    }
}

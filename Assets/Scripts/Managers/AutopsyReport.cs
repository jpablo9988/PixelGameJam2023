using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutopsyReport : MonoBehaviour
{
    public static AutopsyReport Instance { get; private set; }
    List<string> phrasesList;
    List<int> possibleOutcomes;
    private readonly string comparisonString = "The location where the body was KILLED could not be determined. ";
    public bool ShowLocation { get; private set; }
    public bool ShowTime { get; private set; }
    public bool ShowWeapon { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one AutopsyReport found in scene!");
        }
        if (Instance == null) Instance = this;
        phrasesList = new();
        possibleOutcomes = new List<int>{ 0, 1, 2 };
        phrasesList.Add("Here are the autopsy findings...");
        phrasesList.Add(comparisonString);
        ShowLocation = false;
        ShowTime = false;
        ShowWeapon = false;
        GenerateDiscoverableFactors();
    }
    private void GenerateDiscoverableFactors()
    {
        int localOutcome = possibleOutcomes[Random.Range(0, possibleOutcomes.Count)];
        switch (localOutcome)
        {
            case 0:
                ShowWeapon = true;
                possibleOutcomes.Remove(1);
                break;
            case 1:
                ShowLocation = true;
                break;
            case 2:
                ShowTime = true;
                possibleOutcomes.Remove(1);
                break;
        }
    }
    public void AddInformationToAutopsy()
    {
        int outcome = possibleOutcomes[Random.Range(0, possibleOutcomes.Count)];
        switch (outcome)
        {
            case 0:
                phrasesList.Add("From the body markings and test results, you could determine that " +
                    "the weapon is some type of " + StoryGenerator.Instance.MurderWeapon.EvidenceToString());
                possibleOutcomes.Remove(0);
                break;
            case 1:
                phrasesList.Add("From the body's possessions and trace amounts of specific particles, you can deduce with certainty " +
                    "that the body was killed at " + StoryGenerator.Instance.MurderLocation[0].GetName());
                phrasesList.Remove(comparisonString);
                possibleOutcomes.Remove(1);
                break;
            case 2:
                phrasesList.Add("From the body's rigor mortis, you can confirm that the body was killed around " +
                     TimeRange.TimeRangeToString(StoryGenerator.Instance.MurderTimes[0].currentTime) + " last night.");
                possibleOutcomes.Remove(2);
                break;
        }
    }
    public void TellAutopsy ()
    {
        DialogueManager.Instance.EnterDialogue(phrasesList);
    }

}

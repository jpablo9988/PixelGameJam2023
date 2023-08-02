using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AcusationPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject dropdownParent;
    [SerializeField]
    private GameObject accusePanel;
    private List<TMP_Dropdown> dropdowns;
    private bool[] checks = {false, false, false, false};
    private bool dialogueControlsActive = false;
    //There will be 4 dropdowns.
    //1. Suspects.
    //2. Weapons
    //3. Time Ranges
    //4. Location
    void Start()
    {
        dropdowns = new(dropdownParent.GetComponentsInChildren<TMP_Dropdown>());
        accusePanel.SetActive(false);
        if (dropdowns.Count != 4)
        {
            Debug.LogWarning("There's not 4 dropdown panels! Error!. There are currently " + dropdowns.Count);
        }

    }
    public void PopulateDropdownOptions()
    {
        for (int i = 0; i < dropdowns.Count; i++)
        {
            dropdowns[i].ClearOptions();
        }
        List<string> optionsToFill = new();
        for(int i = 0; i < StoryGenerator.Instance.GetCharacters().Count; i++)
        {
            optionsToFill.Add(StoryGenerator.Instance.GetCharacters()[i].GetName());
        }
        dropdowns[0].AddOptions(optionsToFill);
        optionsToFill.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(Weapon.WeaponType)).Length; i++)
        {
            optionsToFill.Add(Weapon.GetNameType((Weapon.WeaponType)i));
        }
        dropdowns[1].AddOptions(optionsToFill);
        optionsToFill.Clear();
        for (int i = 0; i < System.Enum.GetNames(typeof(TimeRange.AvailableTimes)).Length; i++)
        {
            optionsToFill.Add(TimeRange.TimeRangeToString((TimeRange.AvailableTimes)i));
        }
        dropdowns[2].AddOptions(optionsToFill);
        optionsToFill.Clear();
        for (int i = 0; i < StoryGenerator.Instance.GetLocations().Count; i++)
        {
            optionsToFill.Add(StoryGenerator.Instance.GetLocations()[i].GetName());
        }
        dropdowns[3].AddOptions(optionsToFill);
        optionsToFill.Clear();
    }
    public void InteractAccusePanel()
    {
        if (! accusePanel.activeInHierarchy )
        {
            if (InputManager.Instance.AreDialogueControlsActive())
            {
                InputManager.Instance.ActivateDialogueControls(false);
                dialogueControlsActive = InputManager.Instance.AreDialogueControlsActive();
            }
            UIManager.Instance.ActivateWorldCanvas(false);
            accusePanel.SetActive(true);
        }
        else
        {
            if (dialogueControlsActive)
            {
                dialogueControlsActive = false;
                InputManager.Instance.ActivateDialogueControls(true);
            }
            UIManager.Instance.ActivateWorldCanvas(true);
            accusePanel.SetActive(false);
        }
    }
    public void Accuse()
    {
        if (dropdowns[0].options[dropdowns[0].value].text.Equals(StoryGenerator.Instance.Killer.GetName()))
        {
            checks[0] = true;
        }
        if (dropdowns[1].options[dropdowns[1].value].text.Equals(StoryGenerator.Instance.MurderWeapon.EvidenceToString()))
        {
            checks[1] = true;
        }
        if (dropdowns[2].options[dropdowns[2].value].text.Equals(TimeRange.TimeRangeToString(StoryGenerator.Instance.MurderTimes[0].currentTime)))
        {
            checks[2] = true;
        }
        if (dropdowns[3].options[dropdowns[3].value].text.Equals(StoryGenerator.Instance.MurderLocation[0].GetName()))
        {
            checks[3] = true;
        }
        int noCorrect = 0;
        for( int i = 0; i < checks.Length; i++)
        {
            if (checks[i])
            {
                noCorrect++;
            }
        }
        if (noCorrect >= 3 && checks[0])
        {
            Settings.FoundGuilty = true;
            Settings.EnoughEvidence = true; 
        }
        else if (noCorrect < 3 && checks[0])
        {
            Settings.FoundGuilty = true;
            Settings.EnoughEvidence = false;
        }
        else
        {
            Settings.FoundGuilty = false;
            Settings.EnoughEvidence = false;
        }
        for(int i = 0; i < StoryGenerator.Instance.GetCharacters().Count; i++)
        {
            if (dropdowns[0].options[dropdowns[0].value].text.Equals(StoryGenerator.Instance.GetCharacters()[i].GetName()))
            {
                Settings.accusedCharacter = StoryGenerator.Instance.GetCharacters()[i];
                break;
            }
        }
        Debug.Log(Settings.FoundGuilty + " | " + Settings.EnoughEvidence);
        GameManager.Instance.EndGame();
        
    }
}

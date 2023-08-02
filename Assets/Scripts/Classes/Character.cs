using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private string v_Name;
    [SerializeField]
    [TextArea(3, 10)]
    private string v_Description;
    public enum Attitude { FRIENDLY, NEUTRAL, HOSTILE };
    public enum Role { INNOCENT, SUSPICIOUS, KILLER, VICTIM };

    public bool HasBeenInvestigated { get; private set; }

    public bool IsBlockedOff { get; private set; } 
    private JournalEntry weaponEntry;
    [System.Serializable]
    public struct CharacterStats
    {
        public int decieving_mod;
        public int withholding_mod;
        public int friendly_mod;
        public int hostile_mod;
    }
    public struct CharacterSettings
    {
        public Attitude c_Attitude;

        public List<VitalInformation> c_Information;

        public Role c_Role;

        public Weapon assigned_Weapon;
    }
    public CharacterSettings CurrentSettings { get; set; }

    [SerializeField]
    private CharacterStats myStats;

    [SerializeField]
    private TextAsset dialogueTree;

    public CharacterStats GetStats()
    {
        return myStats;
    }
    public string GetName()
    {
        return v_Name;
    }
    public void SetJournalEntry(JournalEntry data)
    {
        weaponEntry = data;
    }
    public void ObtainEntry()
    {
        weaponEntry.ObtainJournal();
    }
    public void BlockCharacter()
    {
        IsBlockedOff = true;
    }
    public TextAsset GetDialogueTree()
    {
        return dialogueTree;
    }
    public void InvestigateCharacter()
    {
        HasBeenInvestigated = true;
    }
}

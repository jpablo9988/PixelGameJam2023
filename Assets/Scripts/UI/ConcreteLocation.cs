using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteLocation : MonoBehaviour
{
    public Location Location { get; private set; }

    [SerializeField]
    private string location_name;
    [SerializeField]
    private string description;
    [SerializeField]
    private List<ConcreteLocation> neighbors;
    [SerializeField]
    private TextAsset dialogueTree;

    private List<JournalEntry> assignedEntries;

    [SerializeField]
    private Character insideCharacter;

    public bool IsMurderLocation { get; private set; }
    public bool ContaintsCorpse { get; private set; }
    public bool HasBeenExplored { get; private set; }

    private void Awake()
    {
        assignedEntries = new();
        HasBeenExplored = false;
    }
    public string GetName()
    {
        return this.location_name;
    }
    public string GetDescription()
    {
        return this.description;
    }
    public List<ConcreteLocation> GetNeighbors()
    {
        return this.neighbors;
    }
    public void SetLocation(Location data)
    {
        Location = data;
    }
    public void SetAsMurderLocation()
    {
        this.IsMurderLocation = true;
    }
    public void ExploreRoom()
    {
        HasBeenExplored = true;
    }
    public void CheckForCorpse()
    {
        if (insideCharacter.CurrentSettings.c_Role == Character.Role.VICTIM)
        {
            ContaintsCorpse = true;
        }
    }
    public void PopulateEntries(JournalEntry entry)
    {
        assignedEntries.Add(entry);
    }
    public void ObtainEntries()
    {
        for(int i = 0; i < assignedEntries.Count; i++)
        {
            assignedEntries[i].ObtainJournal();
        }
    }
    public List<JournalEntry> GetEntries()
    {
        return this.assignedEntries;
    }
    public Character GetCurrentCharacter()
    {
        return insideCharacter;
    }
    public TextAsset GetTreeLocation()
    {
        return dialogueTree;
    }
}

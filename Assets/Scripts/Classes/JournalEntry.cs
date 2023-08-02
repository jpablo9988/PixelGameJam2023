using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalEntry 
{
    //In case it is Pair of Time and Location:
            //Time ALWAYS is ORDERED FIRST, then LOCATION.
    List<Evidence> evidenceList;
    public bool Obtained { get; private set; }
    public int ID { get; private set; }
    private readonly Character victim;
    private bool isTypeWeapon;
    public JournalEntry (List<Evidence> evidence, int ID, Character victim, bool type)
    {
        this.evidenceList = evidence;
        this.ID = ID;
        this.victim = victim;
        isTypeWeapon = type;
    }
    public void ObtainJournal()
    {
        Obtained = true;
    }
    public Character GetAssignedCharacter()
    {
        for (int i = 0; i < evidenceList.Count; i++)
        {
            if (evidenceList[i] is Weapon weapon)
            {
                return (weapon.GetCharacter());
            }
            if (evidenceList[i] is TimeRange timeRange)
            {
                return (timeRange.GetCharacter());
            }
        }
        return null;
    }
    public Character ReturnAssignedVictim()
    {
        return victim;
    }
    public bool GetIfTypeWeapon()
    {
        return isTypeWeapon;
    }
    public List<Evidence> GetAssociatedEvidence()
    {
        return evidenceList;
    }
    public string GetWrittenJournal()
    {
        string currLocation = "";
        string locationReturnString = "";
        for (int i = 0; i < evidenceList.Count; i++)
        {
            //Debug.Log(evidenceList[i].EvidenceToString());
            if (evidenceList[i] is Weapon weapon)
            {
                return (weapon.GetCharacter().GetName() + " had a "
                    + weapon.GetSpecificName() + " last night when "
                    + victim.GetName() + " was killed. ");
            }
            if (evidenceList[i] is Location location)
            { 
                currLocation = location.EvidenceToString();
            }
            if (evidenceList[i] is TimeRange timeRange)
            {
                locationReturnString = timeRange.GetCharacter().GetName() + " was spotted at " + timeRange.EvidenceToString() + " ";
            }
        }
        return locationReturnString + " in the " + currLocation + ".";
    }

}

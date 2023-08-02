using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationLie : LocationInformation
{
    public Location P_LieLocation { get; private set; }
    public JournalEntry key;
    private bool beenDismantled = false;
    public bool IsForgotten { get; private set; }
    

    public LocationLie(Location location, Location p_LieLocation, JournalEntry key, bool isForgotten) : base(location)
    {
        P_LieLocation = p_LieLocation;
        this.key = key; 
        this.IsForgotten = isForgotten;
    }
    public void BreakKey(JournalEntry applicant)
    {
        if (key.ID == applicant.ID && !beenDismantled)
        {
            P_LieLocation = P_CurrentLocation;
            beenDismantled = true;
            IsForgotten = false;
        }
    }
    public override string GetInformationString()
    {
        if (IsForgotten) return P_CurrentLocation.P_Location_Name;
        
        return P_LieLocation.P_Location_Name;
    }

}

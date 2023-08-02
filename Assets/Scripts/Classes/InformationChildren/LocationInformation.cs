using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationInformation : VitalInformation
{
    public Location P_CurrentLocation { get; private set; }
    public TimeInformation P_AssociatedTime { get; private set; }
    public LocationInformation(Location location)
    {
        P_CurrentLocation = location;
        CurrType = InformationType.LOCATION;
    }
    public override string GetInformationString()
    {
        return P_CurrentLocation.P_Location_Name;
    }
    public void SetAssociateTime(TimeInformation time)
    {
        P_AssociatedTime = time;
    }
}

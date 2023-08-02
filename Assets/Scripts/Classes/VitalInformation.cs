using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalInformation
{
    public enum InformationType { WEAPON, LOCATION, TIME };
    public InformationType CurrType { private get; set; }

    public VitalInformation()
    {

    }
    public virtual string GetInformationString()
    {
        return "Non Specified Information";
    }
}

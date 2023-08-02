using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeInformation : VitalInformation
{
    public TimeRange P_CurrentTime { get; private set; }
    public TimeInformation(TimeRange time)
    {
        P_CurrentTime = time;
        CurrType = InformationType.TIME;
    }
    public override string GetInformationString()
    {
        return P_CurrentTime.EvidenceToString();
    }
}

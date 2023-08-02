using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLie : TimeInformation
{
    public TimeRange P_LieTimeRange { get; private set; }
    public JournalEntry key;
    public bool IsForgotten { get; private set; }
    private bool beenDismantled = false;

    public TimeLie(TimeRange time, TimeRange P_LieTimeRange, JournalEntry key, bool isForgotten) : base(time)
    {
        this.P_LieTimeRange = P_LieTimeRange;
        this.key = key;
        this.IsForgotten = isForgotten;
    }
    public void BreakKey(JournalEntry applicant)
    {
        if (key.ID == applicant.ID && !beenDismantled)
        {
            P_LieTimeRange = P_CurrentTime;
            beenDismantled = true;
            IsForgotten = false;
        }
    }
    public override string GetInformationString()
    {
        if (IsForgotten) return P_CurrentTime.EvidenceToString();
        return P_LieTimeRange.EvidenceToString();
    }

}

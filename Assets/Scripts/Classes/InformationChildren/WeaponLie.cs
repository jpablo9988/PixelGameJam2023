using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLie : WeaponInformation
{
    public Weapon P_WeaponLie { get; private set; }
    public JournalEntry key;
    private bool beenDismantled = false;
    public bool IsForgotten { get; private set; }
    public WeaponLie(Weapon weapon, Weapon P_WeaponLie, JournalEntry key, bool IsForgotten) : base(weapon)
    {
        this.P_WeaponLie = P_WeaponLie;
        this.key = key;
        this.IsForgotten = IsForgotten;
    }
    public void BreakKey(JournalEntry applicant)
    {
        if (key.ID == applicant.ID && !beenDismantled)
        {
            P_WeaponLie = P_CurrWeapon;
            beenDismantled = true;
            IsForgotten = false;
        }
    }
    public override string GetInformationString()
    {
        if (IsForgotten) return P_CurrWeapon.EvidenceToString();
        return P_WeaponLie.EvidenceToString();
    }
}

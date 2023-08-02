using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInformation : VitalInformation
{
    public Weapon P_CurrWeapon { get; private set; }

    public WeaponInformation(Weapon weapon)
    {
        P_CurrWeapon = weapon;
        CurrType = InformationType.WEAPON;
    }
    public override string GetInformationString()
    {
        return P_CurrWeapon.EvidenceToString();
    }
}

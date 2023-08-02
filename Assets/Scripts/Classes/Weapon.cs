using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Evidence
{
    private string weaponName;
    public enum WeaponType { BLUNT, FIREARM, POISON }
    public WeaponType Type { get; private set; }

    public Weapon (WeaponType type, string name)
    {
        weaponName = name;
        Type = type;
    }

    public void SetWeaponType( WeaponType weapon)
    {
        Type = weapon;
    }
    public override string EvidenceToString()
    {
        switch (Type)
        {
            case WeaponType.BLUNT:
                return "Blunt Force Weapon";
            case WeaponType.FIREARM:
                return "Firearm";
            case WeaponType.POISON:
                return "Poison";
            default:
                Debug.LogWarning("Weapon Type out of range");
                return "";

        }
    }
    public static string GetNameType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.BLUNT:
                return "Blunt Force Weapon";
            case WeaponType.FIREARM:
                return "Firearm";
            case WeaponType.POISON:
                return "Poison";
            default:
                Debug.LogWarning("Weapon Type out of range");
                return "";
        }
    }
    public string GetSpecificName()
    {
        return weaponName;
    }
}

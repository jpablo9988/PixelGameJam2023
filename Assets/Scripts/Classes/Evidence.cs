using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Evidence
{
    private Character assignedCharacter;
    public virtual string EvidenceToString()
    {
        return "";
    }
    public void SetCharacter(Character character)
    {
        assignedCharacter = character;
    }
    public Character GetCharacter()
    {
        return assignedCharacter;
    }
}

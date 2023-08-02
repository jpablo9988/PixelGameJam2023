using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private void Start()
    {
        Character character = Settings.accusedCharacter;
        DialogueManager.Instance.EnterDialogue(character.GetDialogueTree(), "Condemnation", Settings.FoundGuilty, Settings.EnoughEvidence);
    }
}

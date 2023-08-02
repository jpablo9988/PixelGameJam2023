using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 *  story manager. manages dialogue said by characters or otherwise.
 */
public class StoryManager : MonoBehaviour
{
    [SerializeField]
    [Header("Dialogue Files")]
    private TextAsset d_TestDialogue;
    [Tooltip("For testing purposes, mate.")]

    public void StartTestDialogue()
    {
        DialogueManager.Instance.EnterDialogue(d_TestDialogue, "sample");
    }
    public static void SearchLogs(ConcreteLocation location)
    {
        // -- Print Dialogue --  OR -- Generate UI Paper -- //
        List<JournalEntry> information = location.GetEntries();
        List<string> dialogue = new ();
        for (int i = 0; i < location.GetEntries().Count; i++)
        {
            dialogue.Add(information[i].GetWrittenJournal());
        }
        if (dialogue.Count == 0)
        {
            dialogue.Add("No one passed through this room last night. ");
        }
        else
        {
            dialogue.Add("Check your Journal to review this information");
        }
        DialogueManager.Instance.EnterDialogue(dialogue);
        location.ObtainEntries();
    }
    public static void QuestionCharacter(ConcreteLocation location)
    {
        Character character = location.GetCurrentCharacter();
        DialogueManager.Instance.EnterDialogue(character.GetDialogueTree(), "Questioning", character);
    }
    public static void QuestionAutopsy(ConcreteLocation location)
    {
        DialogueManager.Instance.EnterAutopsyDialogue(location.GetTreeLocation(), "Autopsy", location);
    }
    public static void QuestionLocation(ConcreteLocation location)
    {
        DialogueManager.Instance.EnterDialogue(location.GetTreeLocation(), "Investigate", location);
    }
    public static void CheckCharacter(ConcreteLocation location)
    {
        Character character = location.GetCurrentCharacter();
        DialogueManager.Instance.EnterDialogue(character.GetDialogueTree(), "Check");
    }
    public static void InvestigateCharacter(ConcreteLocation location)
    {
        Character character = location.GetCurrentCharacter();
        DialogueManager.Instance.EnterDialogue(character.GetDialogueTree(), "Investigate", character);
    }

}

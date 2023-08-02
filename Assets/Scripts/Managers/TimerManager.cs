using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static TimerManager Instance { get; private set; }
    public int TimeRemaining = 5;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one Story Generator found in scene!");
        }
        if (Instance == null) Instance = this;
    }

    public void PassTime()
    {
        if (TimeRemaining > 0)
        {
            TimeRemaining--;
        }
    }
    /*
                AnnounceTime
        time dialogue. instead of calling an ink file to read dialogue from, does it from a list of strings.
                    could be designed different, but there's not tiiiimeeeee.
    */
    public void AnnounceTime()
    {
        List<string> myAnnouncement = new();
        switch (TimeRemaining)
        {
            case 0:
                myAnnouncement.Add("It is now <b>night</b>. There is no more time. I have to announce who the killer is...");
                myAnnouncement.Add("Before someone else gets hurt. ");
                myAnnouncement.Add("<i>You can still talk to suspects, but you can't no longer investigate new information.</i> ");
                break;
            case 1:
                myAnnouncement.Add("It is now <b>evening</b>.");
                myAnnouncement.Add("The waves crash by the ship. The seagull noises start to die down as the moon rises.");
                myAnnouncement.Add("I have to find the killer before night...");
                break;
            case 2:
                myAnnouncement.Add("It is now the <b>afternoon</b>.");
                myAnnouncement.Add("The scorching sun has died down as a beautiful sunset approaches.");
                myAnnouncement.Add("I got to keep investigating before it's too late.");
                break;
            case 3:
                myAnnouncement.Add("It is now the <b>midday</b>.");
                myAnnouncement.Add("My stomach growls. I find some biscuits from last night. For now the hunger abades. ");
                myAnnouncement.Add("Let's continue the investigation, yes? ");
                break;
            case 4:
                myAnnouncement.Add("It is now the <b>morning</b>.");
                myAnnouncement.Add("The sun starts gaining force... it's quite hot now. ");
                myAnnouncement.Add("The investigation is just getting started. ");
                break;
        }
        DialogueManager.Instance.EnterDialogue(myAnnouncement);
    }
    public void NoMoreTime()
    {
        List<string> myAnnouncement = new();
        myAnnouncement.Add("There's no more time for investigations.");
        myAnnouncement.Add("The hour of judgement is nigh. ");
        DialogueManager.Instance.EnterDialogue(myAnnouncement);
    }

}

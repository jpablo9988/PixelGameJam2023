using System.Collections;
using System.Collections.Generic;
using TMPro;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

/*                  singleton class - dialogue manager.
 *           deals with dialogue loading and showcase in-game.
 */
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
            
    /*                                      Tags
     *      In INK, tags will decide what name, portrait and dialogue speed to play.    */
    private const string T_CHAR = "character";
    private const string T_DIALG_SPEED = "speed";
    private const string T_PAUSE_DIALG = "pause";
    private const string T_PORTRAIT_STATE = "state";

    private List<string> localStory;
    private int localStoryCounter = 0;
    private Story currentStory;
    private string v_CurrentSentence = "";
    private int v_CurrMaxVisibleCharacters = 0;
    private List<Choice> v_CurrentChoices;
    private bool isEnd = false;


    private int situation = 0;
    // .. Location Support .. //
    private bool willExploreRoom = false;
    private bool willDoAutopsy = false;
    private bool willAnnounceTime = false;
    private ConcreteLocation currLocationExplored;
    private Character currCharacter;
    
    Dictionary<(int, string), string> v_DialogueEventDict;
    private IEnumerator c_DialogueCoroutine;

    // ... Serializable Variables ... //
    [SerializeField]
    private Animator a_portrait;
    [SerializeField]
    private bool v_CanFastfowardText = true;
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI u_textDisplay;
    [SerializeField]
    private TextMeshProUGUI u_nameDisplay;
    [SerializeField]
    private GameObject u_dialoguePanel;
    [SerializeField]
    private List<Button> u_buttonGameObjects;

    [SerializeField]
    private AudioSource talkingSound;

    // ... Properties ... //
    public bool IsTextAdvancable { get; private set; }
    public bool IsDialogueRunning { get; private set; }
    public bool P_CanFastfowardText { get { return v_CanFastfowardText; } private set { v_CanFastfowardText = value; } }
    public bool P_IsCoroutineRunning { get; private set; }


    void Awake()
    {
        IsDialogueRunning = false;
        P_IsCoroutineRunning=false;
        v_DialogueEventDict = new Dictionary<(int, string), string>();
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this);
            Debug.LogWarning("Duplicate instance of DialogueManager has been found");
        }
        
        u_dialoguePanel.SetActive(false);
        IsTextAdvancable = false;
        times = new();
        locations = new();
    }
    List<string> times;
    List<string> locations;
    bool hasForgottenPlaces = false;
    /*                      SetInkVariables override for location.
                set INK variables that will affect the branching dialogue when dealing with location files.
     */
    private void SetInkVariables(ConcreteLocation location)
    {
        currentStory.variablesState["victim"] = StoryGenerator.Instance.Victims[0].GetName();
        currentStory.variablesState["isMurder"] = location.ContaintsCorpse;
        currentStory.variablesState["weapon"] = StoryGenerator.Instance.MurderWeapon.EvidenceToString();
        currentStory.variablesState["time"] = TimeRange.TimeRangeToString(StoryGenerator.Instance.MurderTimes[0].currentTime);
        currentStory.variablesState["location"] = StoryGenerator.Instance.MurderLocation[0].GetName();
        currentStory.variablesState["thisLocation"] = location.GetName();
        currentStory.variablesState["showLocation"] = AutopsyReport.Instance.ShowLocation;
        currentStory.variablesState["showTime"] = AutopsyReport.Instance.ShowTime;
        currentStory.variablesState["showWeapon"] = AutopsyReport.Instance.ShowWeapon;
    }
    /*                      SetInkVariables override for character.
                Takes into account existing lies from characters, and if a character forgot information.
                Changes character dialogue depending on character mood towards player.
     */
    private void SetInkVariables(Character character)
    {
        currentStory.variablesState["victim"] = StoryGenerator.Instance.Victims[0].GetName();
        for (int i = 0; i < character.CurrentSettings.c_Information.Count; i++)
        {
            
            VitalInformation currInfo = character.CurrentSettings.c_Information[i];
            if (currInfo is WeaponLie weaponLie)
            {
                
                if (weaponLie.IsForgotten)
                {
                    currentStory.variablesState["forgotWeapon"] = true;
                }
                else
                {
                    currentStory.variablesState["weapon"] = weaponLie.P_WeaponLie.GetSpecificName();
                }
            }
            else if (currInfo is WeaponInformation weapon)
            {
                currentStory.variablesState["weapon"] = weapon.P_CurrWeapon.GetSpecificName();
            }
            //Get info in dictionary way. Then order the dictionary based on times.
            if (currInfo is LocationLie lie)
            {
                if (!lie.IsForgotten)
                {
                    if (lie.P_AssociatedTime is TimeLie t_lie)
                    {
                        if (!t_lie.IsForgotten)
                        {
                            locations.Add(lie.P_LieLocation.P_Location_Name);
                            times.Add(t_lie.P_LieTimeRange.EvidenceToStringRounded());
                        }
                        else hasForgottenPlaces = true;
                    }
                    else if (lie.P_AssociatedTime is TimeInformation t_info)
                    {
                        locations.Add(lie.P_LieLocation.P_Location_Name);
                        times.Add(t_info.P_CurrentTime.EvidenceToStringRounded());
                    }
                }
                else hasForgottenPlaces = true;
            }
            else if (currInfo is LocationInformation info)
            {
                if (info.P_AssociatedTime is TimeLie t_lie)
                {
                    if (!t_lie.IsForgotten)
                    {
                        locations.Add(info.P_CurrentLocation.P_Location_Name);
                        times.Add(t_lie.P_LieTimeRange.EvidenceToStringRounded());
                    }
                    else hasForgottenPlaces = true;
                }
                else if (info.P_AssociatedTime is TimeInformation t_info)
                {
                    locations.Add(info.P_CurrentLocation.P_Location_Name);
                    times.Add(t_info.P_CurrentTime.EvidenceToStringRounded());
                }
            }
        }
        currentStory.variablesState["forgotLocation"] = hasForgottenPlaces;
        hasForgottenPlaces = false;
        currentStory.variablesState["location_repeats"] = locations.Count;
        switch (character.CurrentSettings.c_Attitude)
        {
            case Character.Attitude.FRIENDLY:
                currentStory.variablesState["current_emotion"] = "friendly";
                break;
            case Character.Attitude.NEUTRAL:
                currentStory.variablesState["current_emotion"] = "neutral";
                break;
            case Character.Attitude.HOSTILE:
                currentStory.variablesState["current_emotion"] = "hostile";
                break;
        }
        currentStory.variablesState["blocked_off"] = character.IsBlockedOff;
        currentStory.variablesState["realWeapon"] = character.CurrentSettings.assigned_Weapon.GetSpecificName();
    }
    public void EnterAutopsyDialogue(TextAsset inkFile, string pathString, ConcreteLocation location)
    {
        situation = 3;
        if (willDoAutopsy)
        {
            AutopsyReport.Instance.TellAutopsy();
            return;
        }
        else if (!willDoAutopsy && TimerManager.Instance.TimeRemaining <= 0)
        {
            TimerManager.Instance.NoMoreTime();
            return;
        }
        UIManager.Instance.ActivateWorldCanvas(false);
        currLocationExplored = location;
        u_dialoguePanel.SetActive(true);
        currentStory = new Story(inkFile.text);
        currentStory.ChoosePathString(pathString);
        IsDialogueRunning = true;
        InputManager.Instance.ActivateDialogueControls(true);
        SetInkVariables(location);
        ContinueStory();
    }

    public void EnterDialogue(TextAsset inkFile, string pathString, ConcreteLocation location)
    {
        situation = 2;
        willExploreRoom = false;
        if (location.HasBeenExplored)
        {
            pathString = "ExploreRoom";
            willExploreRoom = true;
        }
        else if (!location.HasBeenExplored && TimerManager.Instance.TimeRemaining <= 0)
        {
            TimerManager.Instance.NoMoreTime();
            return;
        }
        UIManager.Instance.ActivateWorldCanvas(false);
        currLocationExplored = location;
        SetUpInkDialogue(inkFile, pathString);
        SetInkVariables(location);
        ContinueStory();
    }

    // *Character Ink -> Names for Investigate and RealWeapon must be THIS.
    public void EnterDialogue(TextAsset inkFile, string pathString, Character character = null)
    {
        currCharacter = character;
        situation = 1;
        
        // -- check for investigations -- //
        if (pathString.Equals("Investigate"))
        {
            situation = 4;
            if (character.HasBeenInvestigated)
            {
                pathString = "RealWeapon";
            }
            else if (!character.HasBeenInvestigated && TimerManager.Instance.TimeRemaining <= 0)
            {
                TimerManager.Instance.NoMoreTime();
                return;
            }
        }
        
        else if (!pathString.Equals("Condemnation"))
        {
            UIManager.Instance.ShowDialogueScreen();
            UIManager.Instance.ActivateWorldCanvas(false);
        }
        if (character == null) situation = 5;
        SetUpInkDialogue(inkFile, pathString);
        if (pathString.Equals("Condemnation"))
        {
            currentStory.variablesState["isGuilty"] = Settings.FoundGuilty;
            currentStory.variablesState["enoughEvidence"] = Settings.EnoughEvidence;
            isEnd = true;
        }
        else
        {
            SetInkVariables(character);
        }
        ContinueStory();
    }
    public void EnterDialogue(TextAsset inkFile, string pathString)
    {
        situation = 5;
        UIManager.Instance.ActivateWorldCanvas(false);
        SetUpInkDialogue(inkFile, pathString);
        ContinueStory();
    }
    private void SetUpInkDialogue(TextAsset inkFile, string pathString)
    {
        u_dialoguePanel.SetActive(true);
        currentStory = new Story(inkFile.text);
        currentStory.ChoosePathString(pathString);
        IsDialogueRunning = true;
        InputManager.Instance.ActivateDialogueControls(true);
    }
    public void EnterDialogue(List<string> list)
    {
        situation = 0;
        UIManager.Instance.ActivateWorldCanvas(false);
        u_nameDisplay.text = "Fox";
        u_dialoguePanel.SetActive(true);
        IsDialogueRunning = true;
        localStory = list;
        InputManager.Instance.ActivateDialogueControls(true);
        ContinueStory();
    }
    // Condemnation dialogue. --
    public void EnterDialogue(TextAsset inkFile, string pathString, bool isGuilty, bool enoughEvidence)
    {
        situation = 5;
        u_dialoguePanel.SetActive(true);
        currentStory = new Story(inkFile.text);
        currentStory.ChoosePathString(pathString);
        IsDialogueRunning = true;
        InputManager.Instance.ActivateDialogueControls(true);
        currentStory.variablesState["isGuilty"] = isGuilty;
        currentStory.variablesState["enoughEvidence"] = enoughEvidence;
        isEnd = true;
        ContinueStory();
    }
    public void ExitDialogue()
    {
        locations.Clear();
        times.Clear();
        if (!isEnd)
        {
            UIManager.Instance.ActivateWorldCanvas(true);
        }
        localStoryCounter = 0;
        InputManager.Instance.ActivateDialogueControls(false);
        u_dialoguePanel.SetActive(false);
        IsDialogueRunning = false;
        switch (situation)
        {
            case 1:
                    if(isEnd)
                    {
                        GameManager.Instance.MainMenuEnd();
                    }
                    else
                    {
                        UIManager.Instance.HideDialogueScreen();
                    }
                break;
            case 2:
                if (willExploreRoom && currLocationExplored != null)
                {
                    currLocationExplored.ExploreRoom();
                    willExploreRoom = false;
                    StoryManager.SearchLogs(currLocationExplored);
                }
                break;
            case 3:
                if (willDoAutopsy && currLocationExplored != null)
                {
                    AutopsyReport.Instance.TellAutopsy();
                }
                break;
            default:
                if (willAnnounceTime)
                {
                    willAnnounceTime = false;
                    TimerManager.Instance.PassTime();
                    TimerManager.Instance.AnnounceTime();
                    if (TimerManager.Instance.TimeRemaining == 3)
                    {
                        AudioManager.Instance.StartEveningMusic();
                    }
                    if (TimerManager.Instance.TimeRemaining == 1)
                    {
                        AudioManager.Instance.StartEndingMusic();
                    }
                }
                break;
        }
    }
    private void HandleTags(List<string> currentTags)
    {
        v_DialogueEventDict.Clear();

        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');

            if (splitTag.Length > 2)
            {
                Debug.LogWarning("Tag length is not supported and it will not be parsed");

            }

            string tagKey = splitTag[0];

            switch (tagKey)
            {
                
                case T_PORTRAIT_STATE:
                    if (a_portrait != null)
                    {
                        if (splitTag[1].Contains(','))
                        {
                            AddToDictionary(splitTag);

                        }
                        else
                        {
                            int stateID = Animator.StringToHash(splitTag[1]);
                            if (this.a_portrait.HasState(0, stateID))
                                this.a_portrait.Play(splitTag[1]);
                            else
                            {
                                //Debug.Log("Using this tag: " + splitTag[1] + ", We couldn't find the portrait.");
                                this.a_portrait.Play("default");
                            }
                        }
                    }
                    break;
                
                case T_CHAR:
                    u_nameDisplay.text = splitTag[1];
                    break;
                default:
                    if (tagKey.Equals(T_DIALG_SPEED) || tagKey.Equals(T_PAUSE_DIALG))
                    {
                        AddToDictionary(splitTag);
                    }
                    else
                    {
                        Debug.LogWarning("Tag is not recognized and it will be ignored");
                    }
                    break;
            }
        }
    }
    public void ContinueStory()
    {
        switch (situation)
        {
            case 0:
                if (localStoryCounter < localStory.Count)
                {
                    if (u_textDisplay != null)
                    {
                        v_CurrentSentence = localStory[localStoryCounter];
                        localStoryCounter++;
                        v_CurrMaxVisibleCharacters = v_CurrentSentence.Length;
                        c_DialogueCoroutine = WriteDialogueNoTags(v_CurrentSentence);
                        StartCoroutine(c_DialogueCoroutine);
                    }
                }
                else
                {
                    ExitDialogue();
                }
                break;
            case 1:
            case 4:
                if (currentStory.canContinue)
                {

                    if (u_textDisplay != null)
                    {
                        
                        v_CurrentSentence = currentStory.Continue();
                        if ((bool)currentStory.variablesState["isInLoop"] && ((int)currentStory.variablesState["counter"] < (int)currentStory.variablesState["location_repeats"]))
                        {
                            currentStory.variablesState["location"] = locations[(int)currentStory.variablesState["counter"]];
                            currentStory.variablesState["time"] = times[(int)currentStory.variablesState["counter"]];
                        }
                        v_CurrentChoices = currentStory.currentChoices;
                        v_CurrMaxVisibleCharacters = v_CurrentSentence.Length;
                        c_DialogueCoroutine = WriteDialogue(v_CurrentSentence);
                        StartCoroutine(c_DialogueCoroutine);
                    }
                }
                else
                {
                    ExitDialogue();
                }
                break;
            default:
                if (currentStory.canContinue)
                {

                    if (u_textDisplay != null)
                    {
                        
                        v_CurrentSentence = currentStory.Continue();
                        v_CurrentChoices = currentStory.currentChoices;
                        v_CurrMaxVisibleCharacters = v_CurrentSentence.Length;
                        c_DialogueCoroutine = WriteDialogue(v_CurrentSentence);
                        StartCoroutine(c_DialogueCoroutine);

                    }
                }
                else
                {
                    ExitDialogue();
                }
                break;
        }
    }
    private void AddToDictionary(string[] splitTag)
    {
        /* splitInfo[0] containts the data (speed of dialogue), and splitInfo[1] contains
                     in which word is it executed. */
        string[] splitInfo = splitTag[1].Split(',');
        try
        {
            int newValue;
            newValue = int.Parse(splitInfo[1]);
            (int, string) auxTuple = (newValue, splitTag[0]);
            v_DialogueEventDict.Add(auxTuple, splitInfo[0]);
        }
        catch (FormatException e)
        {
            Debug.LogWarning("Tag " + splitTag[0] + " not in the correct format: " + e.Message + " \n Skipping Tag.");
        }
    }
    IEnumerator WriteDialogueNoTags(String sentence)
    {
        IsTextAdvancable = false;
        P_IsCoroutineRunning = true;
        char[] arraySentence = sentence.ToCharArray();
        u_textDisplay.text = sentence;
        u_textDisplay.maxVisibleCharacters = 0;
        for (int i = 0; i < arraySentence.Length; i++)
        {
            v_CanFastfowardText = true;
            u_textDisplay.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.05f);
        }
    }
    private int wordCounter = 0;
    private float currDialogueSpeed = 0.2f;
    private bool isFirstLetter = true;
    
    IEnumerator WriteDialogue(String sentence)
    {
        bool isPaused = false;
        int letterCounter = 0;
        IsTextAdvancable = false;
        P_IsCoroutineRunning = true;
        isFirstLetter = true;
        wordCounter = 0;
        u_textDisplay.text = sentence;
        u_textDisplay.maxVisibleCharacters = 0;
        HandleTags(currentStory.currentTags);
        char[] arraySentence = sentence.ToCharArray();
        foreach (char letter in arraySentence)
        {
            v_CanFastfowardText = true;
            if (isPaused)
            {
                isPaused = false;
                a_portrait.SetBool("Talk", true);
            }
            float pauseTimer = 0;
            if (Char.IsWhiteSpace(letter) && (letterCounter + 1 < arraySentence.Length))
            {
                if (!Char.IsWhiteSpace(arraySentence[letterCounter + 1]))
                {
                    isFirstLetter = true;
                    wordCounter++;
                }
            }
            if (isFirstLetter)
            {
                foreach ((int, string) tuple in v_DialogueEventDict.Keys)
                {
                    if (tuple.Item1 == wordCounter)
                    {
                        switch (tuple.Item2)
                        {
                            
                            case T_PORTRAIT_STATE:
                                if (a_portrait != null)
                                {
                                    int stateID = Animator.StringToHash(v_DialogueEventDict[tuple]);
                                    if (this.a_portrait.HasState(0, stateID))
                                        this.a_portrait.Play(v_DialogueEventDict[tuple]);
                                    else
                                    {
                                        Debug.LogWarning("Using this tag: " + v_DialogueEventDict[tuple] + ", We couldn't find the portrait.");
                                        this.a_portrait.Play("default");
                                    }
                                }
                            
                                break;
                            case T_DIALG_SPEED:
                                try
                                {
                                    currDialogueSpeed = float.Parse(v_DialogueEventDict[tuple], CultureInfo.InvariantCulture);
                                }
                                catch (FormatException e)
                                {
                                    Debug.LogWarning("Dialogue Speed not in the correct format. " + e.Message + " \n Skipping Tag.");
                                }
                                break;
                            case T_PAUSE_DIALG:
                                try
                                {
                                    pauseTimer = float.Parse(v_DialogueEventDict[tuple], CultureInfo.InvariantCulture);
                                    isPaused = true;
                                }
                                catch (FormatException e)
                                {
                                    Debug.LogWarning("Pause not in the correct format. " + e.Message + " \n Skipping Tag.");
                                }
                                break;

                        }
                    }
                }
                isFirstLetter = false;
            }
            if (situation == 1 || situation == 5)
            {
                if (isPaused) a_portrait.SetBool("Talk", false);
                else a_portrait.SetBool("Talk", true);
            }
            u_textDisplay.maxVisibleCharacters++;
            yield return new WaitForSeconds(currDialogueSpeed + pauseTimer);
            letterCounter++;
            talkingSound.Play();
        }
        if (situation == 1)
        {
            a_portrait.SetBool("Talk", false);
        }
        FinishSentenceConditions();
    }
    private void SetChoices(List<Choice> currStoryChoices)
    {
        if (currStoryChoices.Count > this.u_buttonGameObjects.Count)
        {
            Debug.LogWarning("Choices surpass maximum button limit. Please fix!");
        }
        else
        {
            int i = 0;
            foreach(Choice choice in currStoryChoices)
            {
                TextMeshProUGUI text = u_buttonGameObjects[i].GetComponentInChildren<TextMeshProUGUI>();
                text.text = choice.text;
                this.u_buttonGameObjects[i].gameObject.SetActive(true);
                this.u_buttonGameObjects[i].onClick.AddListener
                    (delegate { ChooseStoryChoice(choice); });
                
                switch (situation)
                {
                    case 2:
                        if (i == 0)
                        {
                            this.u_buttonGameObjects[i].onClick.AddListener
                            (delegate
                            {
                                willAnnounceTime = true;
                                willExploreRoom = true;
                                Debug.Log(willExploreRoom);
                            });
                        }
                        else
                        {
                            this.u_buttonGameObjects[i].onClick.AddListener
                            (delegate
                            {
                                willExploreRoom = false;
                                Debug.Log(willExploreRoom);
                            });
                        }
                        break;
                    case 3:
                        if (i == 0)
                        {
                            this.u_buttonGameObjects[i].onClick.AddListener
                        (delegate
                        {
                            willAnnounceTime = true;
                            willDoAutopsy = true;
                            AutopsyReport.Instance.AddInformationToAutopsy();
                        });
                        }
                        else
                        {
                            this.u_buttonGameObjects[i].onClick.AddListener
                        (delegate
                        {
                            willDoAutopsy = false;
                        });
                        }
                        break;
                    case 4:
                        if (i == 0)
                        {
                            this.u_buttonGameObjects[i].onClick.AddListener
                        (delegate
                        {
                            if (currCharacter != null)
                            {
                                willAnnounceTime = true;
                                currCharacter.ObtainEntry();
                                currCharacter.InvestigateCharacter();
                            }
                            else Debug.LogWarning("Character investigated doesn't exist. Take care.");
                        });
                        }
                        break;
                }
                i++;

            }
        }
    } 
    private void ChooseStoryChoice(Choice choice)
    {
        ResetBranchOptions();
        this.currentStory.ChooseChoiceIndex(choice.index);
        ContinueStory();
    }
    private void ResetBranchOptions()
    {
        for (int i = 0; i < u_buttonGameObjects.Count; i++)
        {
            this.u_buttonGameObjects[i].onClick.RemoveAllListeners();
            this.u_buttonGameObjects[i].gameObject.SetActive(false);
        }
        this.v_CurrentChoices.Clear();
    }
    public bool GetCanAdvanceText()
    {
        return this.v_CanFastfowardText;
    }
    private void FinishSentenceConditions()
    {
        v_CanFastfowardText = false;
        P_IsCoroutineRunning = false;
        if (v_CurrentChoices != null)
        {
            if (v_CurrentChoices.Count > 0)
            {
                this.SetChoices(currentStory.currentChoices);
            }
            else
            {
                IsTextAdvancable = true;
            }
        }
        else
        {
            IsTextAdvancable = true;
        }
    }
    public void SkipDialogueCoroutine()
    {
        if (P_IsCoroutineRunning)
        {
            FinishSentenceConditions();
            StopCoroutine(c_DialogueCoroutine);
            u_textDisplay.maxVisibleCharacters = v_CurrMaxVisibleCharacters;
            if (situation == 1)
            {
                a_portrait.SetBool("Talk", false);
            }
        }
    }
}

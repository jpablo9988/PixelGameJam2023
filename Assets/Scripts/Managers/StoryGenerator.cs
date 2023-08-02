using System.Collections;
using System.Collections.Generic;
using UnityEngine;


        // story generator - could be partitioned later in code cleanup. it's a bit too monolith-y rn
public class StoryGenerator : MonoBehaviour
{
    [Header("Journal Reference")]
    [SerializeField]
    private Journal journalInstance;
    [Header("Relations")]
    [SerializeField]
    private AcusationPanel accusation_Panel;
    [Header("Actors and Settings")]
    [SerializeField]
    private List<Character> _characterList;
    [SerializeField]
    private List<ConcreteLocation> _locationList;
    [SerializeField]
    private List<string> _murderWeapons;
    [Tooltip("Specific Order: First Third -> Blunt Objects, Next Third -> Firearms, Last Third -> Poisons.")]
    private List<Location> _dataLocationList;

    // ------------------RANDOMNESS VARIABLES. There's 100% a better way to organize this... but im tired ---------------//
    [Header("Randomness Adjustments - Innocent")]
    [SerializeField]
    private int friendly_Chance_Innocent = 50;
    [SerializeField]
    private int neutral_Chance_Innocent = 25;
    [SerializeField]
    private int hostile_Chance_Innocent = 25;
    [SerializeField]
    private int no_Lies_Minimum_Innocent = 0;
    [SerializeField]
    private int no_Lies_Maximum_Innocent = 1;
    [SerializeField]
    private int no_Forgot_Minimum_Innocent = 0;
    [SerializeField]
    private int no_Forgot_Maximum_Innocent = 1;
    [Header("Randomness Adjustments - Suspicious")]
    [SerializeField]
    private int friendly_Chance_Suspicious = 30;
    [SerializeField]
    private int neutral_Chance_Suspicious = 40;
    [SerializeField]
    private int hostile_Chance_Suspicious = 30;
    [SerializeField]
    private int no_Lies_Minimum_Suspicious = 1;
    [SerializeField]
    private int no_Lies_Maximum_Suspicious = 2;
    [SerializeField]
    private int no_Forgot_Minimum_Suspicious = 0;
    [SerializeField]
    private int no_Forgot_Maximum_Suspicious = 1;
    [Header("Randomness Adjustments - Killer")]
    [SerializeField]
    private int friendly_Chance_Killer = 45;
    [SerializeField]
    private int neutral_Chance_Killer = 10;
    [SerializeField]
    private int hostile_Chance_Killer = 45;
    [SerializeField]
    private int no_Lies_Minimum_Killer = 1;
    [SerializeField]
    private int no_Lies_Maximum_Killer = 3;
    [SerializeField]
    private int no_Forgot_Minimum_Killer = 1;
    [SerializeField]
    private int no_Forgot_Maximum_Killer = 2;

    private List<Weapon> _initialPoolWeapon;
    public List<Weapon> AllWeapons { get; private set; }

    private List<ConcreteLocation> _freeLocations;

    List<Character> notAssignedCharacters;

    public Weapon MurderWeapon { get; private set; }
    public List<TimeRange> MurderTimes { get; private set; }

    public List<ConcreteLocation> MurderLocation { get; private set; }

    public Character Killer { get; private set; }
    public List<Character> Victims { get; private set; }
    public static StoryGenerator Instance { get; private set; }
    private void Awake()
    {
        MurderTimes = new();
        MurderLocation = new();
        Victims = new();
        _dataLocationList = new();
        _initialPoolWeapon = new();
        if (Instance != null)
        {
            Debug.LogWarning("More than one Story Generator found in scene!");
        }
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        CreateLocations(); 
        CreateWeapons(); 
        GenerateStory();
    }
    /*     CreateLocations
     *     instantiates location classes.
     */
    private void CreateLocations()
    {
        for(int i = 0; i < _locationList.Count; i++)
        {
            ConcreteLocation aux = _locationList[i];
            aux.SetLocation(new Location(aux.GetName(), aux.GetDescription()));
            _dataLocationList.Add(aux.Location);
        }
        for (int i = 0; i < _locationList.Count; i++)
        {
            ConcreteLocation aux = _locationList[i];
            for (int j = 0; j < aux.GetNeighbors().Count; j++)
            {
                aux.Location.AddNeighborLocation(aux.GetNeighbors()[j].Location);
            }
            
        }
        _freeLocations = new(_locationList);
    }
    /*     CreateWeapons
 *     instantiates weapons classes according to weapon pool names and types available.
 *     for now divided in 3, but can be expanded if needed.
 */
    private void CreateWeapons()
    {
        int thirds = _murderWeapons.Count / 3;
        int initial = 0, current = thirds, loopCounter = 0;
        while (current <= _murderWeapons.Count)
        {
            Weapon.WeaponType currType = (Weapon.WeaponType)loopCounter;
            for (int j = initial; j < current; j++)
            {
                Weapon aux = new(currType, _murderWeapons[j]);
                _initialPoolWeapon.Add(aux);
                
            }
            initial += thirds;
            current += thirds;
            loopCounter++;
        }
        AllWeapons = new(_initialPoolWeapon);
    }

    private int journalIDCounter = 0;
    private int noKilledVictims = 0;
    /*
     *                                  GenerateStory.
     *          generates a story. assigns the role of a killer, victim and innocent people to characters.
     *          innocent people are divided in two: suspicious looking and innocent looking.
     *          suspicous people tend to lie more and be more hostile to the player.
     * */
    private void GenerateStory()
    {
        notAssignedCharacters = new(_characterList);
        // Assign a Killer:
        Killer = notAssignedCharacters[Random.Range(0, notAssignedCharacters.Count)];

        Killer.CurrentSettings = new()
            {
                assigned_Weapon = _initialPoolWeapon[Random.Range(0, _initialPoolWeapon.Count)],
                c_Role = Character.Role.KILLER
            };

        MurderWeapon = Killer.CurrentSettings.assigned_Weapon;
        _initialPoolWeapon.Remove(Killer.CurrentSettings.assigned_Weapon);
        notAssignedCharacters.Remove(Killer);
        // Assign a Victim: 
        KillSomebody();

        AddWeaponJournal(Killer, MurderWeapon);
        // Assign roles to other characters: 
        int count = 0;
        while(notAssignedCharacters.Count > 0)
        {
            //Suspicious-looking People -> 0, 1
            //Innocent-looking People -> 2, 3; 2 has ONE assigned character.
            Character auxChar = notAssignedCharacters[Random.Range(0, notAssignedCharacters.Count)];
            if (count < 2)
            {
                auxChar.CurrentSettings = new()
                {
                    c_Role = Character.Role.SUSPICIOUS
                };
            }
            else 
            {
                auxChar.CurrentSettings = new()
                {
                    c_Role = Character.Role.INNOCENT
                };
            }
            count++;
            AssignDataToInnocent(auxChar);
            notAssignedCharacters.Remove(auxChar);
        }
        // ... Assign Personalities ... //
        for (int i = 0; i < _characterList.Count; i++)
        {
            AssignPersonality(_characterList[i]);
            AssignLiesAndInformation(_characterList[i]);
        }
        for (int i = 0; i < _locationList.Count; i++)
        {
            _locationList[i].CheckForCorpse();
        }
        accusation_Panel.PopulateDropdownOptions();
        AutopsyReport.Instance.AddInformationToAutopsy();
        noKilledVictims++;
    }
    /*
 *                                  AssignLiesAndInformation
 *          assigns information that characters can say to player.
 *          assigns lies to said information depending on the character role.
 *          probabilites for each role are set in-editor.
 * */
    private void AssignLiesAndInformation(Character character)
    {
        int localMaximumChance;
        int resultLie, resultForgot;
        switch (character.CurrentSettings.c_Role)
        {
            case Character.Role.INNOCENT:
                localMaximumChance = no_Lies_Maximum_Innocent + character.GetStats().decieving_mod;
                if (no_Lies_Minimum_Innocent <= localMaximumChance)
                {
                    resultLie = Random.Range(no_Lies_Minimum_Innocent, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Lie Range indicator don't work. Assigning 0 lies");
                    resultLie = 0;
                }
                localMaximumChance = no_Forgot_Maximum_Innocent + character.GetStats().withholding_mod;
                if (no_Forgot_Minimum_Innocent <= localMaximumChance)
                {
                    resultForgot = Random.Range(no_Forgot_Minimum_Innocent, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Forget Range indicator don't work. Assigning 0 lies");
                    resultForgot = 0;
                }
                AssignVitalInformation(character, resultLie, resultForgot);
                break;
            case Character.Role.SUSPICIOUS:
                localMaximumChance = no_Lies_Maximum_Suspicious + character.GetStats().decieving_mod;
                if (no_Lies_Minimum_Suspicious <= localMaximumChance)
                {
                    resultLie = Random.Range(no_Lies_Minimum_Suspicious, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Lie Range indicator don't work. Assigning 0 lies");
                    resultLie = 0;
                }
                localMaximumChance = no_Forgot_Maximum_Suspicious + character.GetStats().withholding_mod;
                if (no_Forgot_Minimum_Innocent <= localMaximumChance)
                {
                    resultForgot = Random.Range(no_Forgot_Minimum_Suspicious, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Forget Range indicator don't work. Assigning 0 lies");
                    resultForgot = 0;
                }
                AssignVitalInformation(character, resultLie, resultForgot);
                break;
            case Character.Role.KILLER:
                localMaximumChance = no_Lies_Maximum_Killer + character.GetStats().decieving_mod;
                if (no_Lies_Minimum_Innocent <= localMaximumChance)
                {
                    resultLie = Random.Range(no_Lies_Minimum_Killer, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Lie Range indicator don't work. Assigning 0 lies");
                    resultLie = 0;
                }
                localMaximumChance = no_Forgot_Maximum_Killer + character.GetStats().withholding_mod;
                if (no_Forgot_Minimum_Innocent <= localMaximumChance)
                {
                    resultForgot = Random.Range(no_Forgot_Minimum_Killer, localMaximumChance);
                }
                else
                {
                    Debug.LogWarning("Forget Range indicator don't work. Assigning 0 lies");
                    resultForgot = 0;
                }
                AssignVitalInformation(character, resultLie, resultForgot);
                break;
        }
    }
    /*
*                                  AssignVitalInformation
*          vital information is data that's 100% factual and will be the basis of journal entries for the player.
*          assigns the corresponding vital information to character-known information (wether it's a lie or not).
*          
* */
    private void AssignVitalInformation(Character character, int lies, int forgotten)
    {
        List<JournalEntry> applicableJournals = new ();
        int sizeOfInformation = 0;
        List<int> honestIndexList = new();
        List<int> forgotIndexList = new();
        List<int> lieIndexList = new();
        List<VitalInformation> toAdd = new();
        for ( int i = 0; i < journalInstance.Journals.Count; i++)
        {
            if (journalInstance.Journals[i].GetAssignedCharacter() == character)
            {
                applicableJournals.Add(journalInstance.Journals[i]);
                if (journalInstance.Journals[i].GetIfTypeWeapon()) sizeOfInformation++;
                else sizeOfInformation += 2;
            }
        }
        bool alternancy = false;
        while((lies + forgotten) >= sizeOfInformation)
        {
            if (alternancy)
            {
                lies--;
                alternancy = !alternancy;
            }
            else
            {
                forgotten--;
                alternancy = !alternancy;
            }
        }
        for (int i = 0; i < sizeOfInformation; i++)
        {
            honestIndexList.Add(i);
        }
        for (int i = 0; i < lies; i++)
        {
            int newLie = honestIndexList[Random.Range(0, honestIndexList.Count)];
            honestIndexList.Remove(newLie);
            lieIndexList.Add(newLie);
        }
        for (int i = 0; i < forgotten; i++)
        {
            int newForgot = honestIndexList[Random.Range(0, honestIndexList.Count)];
            honestIndexList.Remove(newForgot);
            forgotIndexList.Add(newForgot);
        }
        int carry_Over = 0;
        for (int i = 0; i < applicableJournals.Count; i++)
        {
            LocationInformation auxLocation = null;
            TimeInformation auxTime = null;
            for (int j = 0; j < applicableJournals[i].GetAssociatedEvidence().Count; j++)
            {
                Evidence currEvidence = applicableJournals[i].GetAssociatedEvidence()[j];
                if (honestIndexList.Contains(i + j + carry_Over))
                {
                    if (currEvidence is Weapon weapon)
                    {
                        WeaponInformation info_weapon = new (weapon);
                        toAdd.Add(info_weapon);
                    }
                    else if(currEvidence is Location location)
                    {
                        LocationInformation info_location = new (location);
                        if (auxTime != null)
                        {
                            info_location.SetAssociateTime(auxTime);
                        }
                        toAdd.Add(info_location);
                        auxLocation = info_location;
                    }
                    else if (currEvidence is TimeRange time)
                    {
                        TimeInformation info_time = new (time);
                        if (auxLocation != null)
                        {
                            auxLocation.SetAssociateTime(info_time);
                        }
                        toAdd.Add(info_time);
                        auxTime = info_time;
                    }
                }
                else if (forgotIndexList.Contains(i + j + carry_Over))
                {
                    if (currEvidence is Weapon weapon)
                    {
                        WeaponLie info_weapon = new(weapon, null, applicableJournals[i],true);
                        toAdd.Add(info_weapon);
                    }
                    else if (currEvidence is Location location)
                    {
                        LocationLie info_location = new(location, null, applicableJournals[i], true);
                        if (auxTime != null)
                        {
                            info_location.SetAssociateTime(auxTime);
                        }
                        toAdd.Add(info_location);
                        auxLocation = info_location;
                    }
                    else if (currEvidence is TimeRange time)
                    {
                        TimeLie info_time = new(time,null, applicableJournals[i], true);
                        if (auxLocation != null)
                        {
                            auxLocation.SetAssociateTime(info_time);
                        }
                        toAdd.Add(info_time);
                        auxTime = info_time;
                    }
                }
                else if (lieIndexList.Contains(i + j + carry_Over))
                {
                    if (currEvidence is Weapon weapon)
                    {
                        //~~Generate Lie~~.
                        List<int> candidates = new();
                        for (int k = 0; k < AllWeapons.Count; k++)
                        {
                            if (AllWeapons[k].Type != weapon.Type)
                            {
                                candidates.Add(k);
                            }
                        }
                        WeaponLie info_weapon = new(weapon, AllWeapons[candidates[Random.Range(0, candidates.Count)]]
                            , applicableJournals[i], false);
                        toAdd.Add(info_weapon);
                    }
                    else if (currEvidence is Location location)
                    {
                        //~~Generate Lie~~.
                        List<int> candidates = new();
                        for (int k = 0; k < _dataLocationList.Count; k++)
                        {
                            if (_dataLocationList[k].P_Location_Name != location.P_Location_Name)
                            {
                                candidates.Add(k);
                            }
                        }
                        LocationLie info_location = new(location, _dataLocationList[candidates[Random.Range(0, candidates.Count)]], applicableJournals[i], false);
                        if (auxTime != null)
                        {
                            info_location.SetAssociateTime(auxTime);
                        }
                        toAdd.Add(info_location);
                        auxLocation = info_location;
                    }
                    else if (currEvidence is TimeRange time)
                    {
                        //~~Generate Lie~~.
                        List<int> candidates = new();
                        for (int k = 0; k < System.Enum.GetNames(typeof(TimeRange.AvailableTimes)).Length; k++)
                        {
                            if ((int)time.currentTime != (int)k)
                            {
                                candidates.Add(k);
                            }
                        }
                        TimeRange fake_Time = new((TimeRange.AvailableTimes)(candidates[Random.Range(0, candidates.Count)]));
                        TimeLie info_time = new(time, fake_Time, applicableJournals[i], false);
                        if (auxLocation != null)
                        {
                            auxLocation.SetAssociateTime(info_time);
                        }
                        toAdd.Add(info_time);
                        auxTime = info_time;
                    }
                }
                carry_Over += j;
            }
        }
        character.CurrentSettings = new Character.CharacterSettings
        {
            assigned_Weapon = character.CurrentSettings.assigned_Weapon,
            c_Attitude = character.CurrentSettings.c_Attitude,
            c_Role = character.CurrentSettings.c_Role,
            c_Information = toAdd
        };
    }
    /*
*                                  AssignPersonality
*          assigns a personality to a character.
*          personality changes flavour text and probabilities of lying/forgetting information.
*          suspicious and killers have a higher probability to be hostile or neutral towards the player.
*          
* */
    private void AssignPersonality(Character character)
    {
        int localFriendlyChance, localNeutralChance, localHostileChance;
        switch (character.CurrentSettings.c_Role)
        {
            case Character.Role.INNOCENT:
                localFriendlyChance = this.friendly_Chance_Innocent;
                localNeutralChance = this.neutral_Chance_Innocent;
                localHostileChance = this.hostile_Chance_Innocent;
                ApplyPersonalityModifiers(character, character.GetStats(), localFriendlyChance, localNeutralChance, localHostileChance);
                break;
            case Character.Role.SUSPICIOUS:
                localFriendlyChance = this.friendly_Chance_Suspicious;
                localNeutralChance = this.neutral_Chance_Suspicious;
                localHostileChance = this.hostile_Chance_Suspicious;
                ApplyPersonalityModifiers(character, character.GetStats(), localFriendlyChance, localNeutralChance, localHostileChance);
                break;
            case Character.Role.KILLER:
                localFriendlyChance = this.friendly_Chance_Killer;
                localNeutralChance = this.neutral_Chance_Killer;
                localHostileChance = this.hostile_Chance_Killer;
                ApplyPersonalityModifiers(character, character.GetStats(), localFriendlyChance, localNeutralChance, localHostileChance);
                break;
        }
    }
    private void ApplyPersonalityModifiers(Character character, Character.CharacterStats stats, int friendly, int neutral, int hostile)
    {
        int result = Random.Range(0, 100);
        if (stats.hostile_mod != 0)
        {
            int modifier = 1;
            if (stats.hostile_mod < 0)
            {
                modifier *= -1;
            }
            hostile += character.GetStats().hostile_mod * modifier;
            neutral -= character.GetStats().hostile_mod * modifier;
        }
        if (stats.friendly_mod != 0)
        {
            int modifier = 1;
            if (stats.friendly_mod < 0)
            {
                modifier *= -1;
            }
            friendly += character.GetStats().friendly_mod * modifier;
            neutral -= character.GetStats().friendly_mod * modifier;
        }
        if (result >= 0 && result < friendly)
        {
            character.CurrentSettings = new Character.CharacterSettings
            {
                c_Attitude = Character.Attitude.FRIENDLY,
                c_Role = character.CurrentSettings.c_Role,
                assigned_Weapon = character.CurrentSettings.assigned_Weapon
            };
        }
        else if (result >= friendly && result < (friendly + neutral))
        {
            character.CurrentSettings = new Character.CharacterSettings
            {
                c_Attitude = Character.Attitude.NEUTRAL,
                c_Role = character.CurrentSettings.c_Role,
                assigned_Weapon = character.CurrentSettings.assigned_Weapon
            };
        }
        else if (result >= (friendly + neutral) && result < (friendly + neutral + hostile))
        {
            character.CurrentSettings = new Character.CharacterSettings
            {
                c_Attitude = Character.Attitude.HOSTILE,
                c_Role = character.CurrentSettings.c_Role,
                assigned_Weapon = character.CurrentSettings.assigned_Weapon
            };
            int isThisBlocked = Random.Range(0, 1);
            if (isThisBlocked == 1)
            {
                character.BlockCharacter();
            }
        }
    }

    /*                     KillSomebody:
     *       Assigns a Victim and a path route to the killer.
     *       Also adds the journals necessary for the kill.
     *       character pathing between rooms makes sense. Players cannot teleport.
     *       
    */ 
    private void KillSomebody()
    {
        
        Victims.Add(notAssignedCharacters[Random.Range(0, notAssignedCharacters.Count)]);
        Victims[noKilledVictims].CurrentSettings = new()
        {
            c_Role = Character.Role.VICTIM
        };
        notAssignedCharacters.Remove(Victims[noKilledVictims]);
        MurderLocation.Add(_freeLocations[Random.Range(noKilledVictims, _locationList.Count)]);
        MurderLocation[noKilledVictims].SetAsMurderLocation();
        _freeLocations.Remove(MurderLocation[noKilledVictims]);

        TimeRange aux = new((TimeRange.AvailableTimes)Random.Range(0, System.Enum.GetNames(typeof(TimeRange.AvailableTimes)).Length));
        aux.SetCharacter(Killer);
        MurderTimes.Add(aux);

        //  -- Victim(s) have been assigned! -- //
        //  -- Assign Killer Route and Add Journals -- //
        Dictionary<TimeRange, Location> killerPath = LocationPathing.GeneratePathing(this._dataLocationList
            , Random.Range(2, 5), mustIncludeLocation: MurderLocation[noKilledVictims].Location, mustIncludeTimeRange: MurderTimes[noKilledVictims]);
        AddLocationTimeJournal(Killer, killerPath);
        // Debug.Log("There is a victim! ->" + Victims[noKilledVictims].GetName());

    }
    private readonly int maxAssignableWeapons = 2;
    private int currAssignedWeapons = 0, currCandidateCont = 0;
    /*
     * Case 0 -> Weapon
     * Case 1 -> Location
     * Case 2 -> TimeRange
     */
    private void AssignDataToInnocent(Character character)
    {
        int currency = 0;
        List<int> assignedOptions = new();
        List<int> possibleOptions = new() { 0, 1, 2 };
        if (character.CurrentSettings.c_Role == Character.Role.INNOCENT)
        {
            if (currAssignedWeapons != maxAssignableWeapons)
            {
                currency++;
            }
        }
        else
        {
            currency = 2;
        }
        for(int i = 0; i < currency; i++)
        {
            //Force Assign suspicious weapons.
            if ((currCandidateCont == 2 && currAssignedWeapons < maxAssignableWeapons) || 
                currCandidateCont == 1 && currAssignedWeapons == 0)
            {
                assignedOptions.Add(0);
                possibleOptions.Remove(0);
                currAssignedWeapons++;
            }
            else
            {
                assignedOptions.Add(possibleOptions[Random.Range(0, possibleOptions.Count)]);
                if (assignedOptions[i] == 0)
                {
                    possibleOptions.Remove(0);
                    currAssignedWeapons++; 
                }
            }
        }
        List<Weapon> validWeapon = new(_initialPoolWeapon);
        bool includeLocation = false, includeTimeZone = false;
        for (int i = 0; i < assignedOptions.Count; i++)
        {
            switch (assignedOptions[i])
            {
                case 0:
                    for (int j = 0; j < _initialPoolWeapon.Count; j++)
                    {
                        if (_initialPoolWeapon[j].Type != MurderWeapon.Type)
                        {
                            validWeapon.Remove(_initialPoolWeapon[j]);
                        }
                    }
                    Weapon toAssign = validWeapon[Random.Range(0, validWeapon.Count)];
                    character.CurrentSettings = new Character.CharacterSettings
                    {
                        c_Role = character.CurrentSettings.c_Role,
                        assigned_Weapon = toAssign
                    };
                    _initialPoolWeapon.Remove(toAssign);
                    AddWeaponJournal(character, toAssign);
                    break;
                case 1:
                    includeLocation = true;
                    break;
                case 2:
                    includeTimeZone = true;
                    break;
            }
        }
        for (int i = 0; i < possibleOptions.Count; i++)
        {
            switch (possibleOptions[i]) {
                case 0:
                    for (int j = 0; j < _initialPoolWeapon.Count; j++)
                    {
                        if (_initialPoolWeapon[j].Type == MurderWeapon.Type)
                        {
                            validWeapon.Remove(_initialPoolWeapon[j]);
                        }
                    }
                    Weapon toAssign = validWeapon[Random.Range(0, validWeapon.Count)];
                    character.CurrentSettings = new Character.CharacterSettings
                    {
                        c_Role = character.CurrentSettings.c_Role,
                        assigned_Weapon = toAssign
                    };
                    _initialPoolWeapon.Remove(toAssign);
                    AddWeaponJournal(character, toAssign);
                    break;
            }
        }
        Dictionary<TimeRange, Location> newPath = GenerateCharacterPath( includeLocation, includeTimeZone);
        AddLocationTimeJournal(character, newPath);
        currCandidateCont++;
    }

    private void AddWeaponJournal(Character character, Weapon weapon)
    {
        weapon.SetCharacter(character);
        JournalEntry weaponEntry = new(new List<Evidence> { weapon }, journalIDCounter, Victims[noKilledVictims], true);
        journalInstance.AddJournalEntry(weaponEntry);
        character.SetJournalEntry(weaponEntry);
        journalIDCounter++;
    }
    private void AddLocationTimeJournal(Character character, Dictionary<TimeRange, Location> path)
    {
        foreach (var location in path)
        {
            location.Key.SetCharacter(character);
            JournalEntry visitEntry = new(new List<Evidence> { location.Key, location.Value }, journalIDCounter, Victims[noKilledVictims], false);
            journalIDCounter++;
            journalInstance.AddJournalEntry(visitEntry);
            for (int i = 0; i < _locationList.Count; i++)
            {
                if (_locationList[i].Location.P_Location_Name.Equals(location.Value.P_Location_Name))
                {
                    _locationList[i].PopulateEntries(visitEntry);
                    break;
                }
            }
        }
    }
    // debug class - tests locations to make sure it makes sense.
    public void TestLocationPathing()
    {
        // Test LocationPathing.
        Dictionary<TimeRange, Location> pathing = LocationPathing.GeneratePathing(_dataLocationList, 4, mustAvoidTimeRangeIndex: 4, mustAvoidLocationIndex: 2, mustIncludeLocation: _dataLocationList[5], mustIncludeTimeRange: new TimeRange(TimeRange.AvailableTimes.TWELVE_HALF));
        Debug.Log(pathing);
        foreach(var location in pathing)
        {
            Debug.Log(location.Key.GetHour() + ":" + location.Key.GetMinutes() + " || " + location.Value.P_Location_Name);

        }
    }
    private Dictionary<TimeRange, Location> GenerateCharacterPath( bool includeLocation, bool includeTimeZone)
    {
        if (includeLocation && !includeTimeZone)
        {
            Dictionary<TimeRange, Location> path = LocationPathing.GeneratePathing(this._dataLocationList
            , Random.Range(2, 5), mustIncludeLocation: MurderLocation[noKilledVictims].Location
            , mustAvoidTimeRangeIndex: (int)MurderTimes[noKilledVictims].currentTime);
            return path;
        }
        else if (!includeLocation && includeTimeZone)
        {
            for (int i = 0; i < _dataLocationList.Count; i++)
            {
                if (_dataLocationList[i].P_Location_Name.Equals(MurderLocation[noKilledVictims].Location.P_Location_Name))
                {
                    Dictionary<TimeRange, Location> path = LocationPathing.GeneratePathing(this._dataLocationList
                    , Random.Range(2, 5), mustAvoidLocationIndex: i
                    , mustIncludeTimeRange: MurderTimes[noKilledVictims]);
                    return path;
                }
            }
            Debug.LogWarning("Error in name matching when creating character path");
            return null;
        }
        else if (includeLocation && includeTimeZone)
        {
            Dictionary<TimeRange, Location> killerPath = LocationPathing.GeneratePathing(this._dataLocationList
            , Random.Range(2, 5), mustIncludeLocation: MurderLocation[noKilledVictims].Location
            , mustIncludeTimeRange: MurderTimes[noKilledVictims]);
            return killerPath;
        }
        else
        {
            for (int i = 0; i < _dataLocationList.Count; i++)
            {
                if (_dataLocationList[i].P_Location_Name.Equals(MurderLocation[noKilledVictims].Location.P_Location_Name))
                {
                    Dictionary<TimeRange, Location> path = LocationPathing.GeneratePathing(this._dataLocationList
                    , Random.Range(2, 5), mustAvoidLocationIndex: i
                    , mustAvoidTimeRangeIndex: (int)MurderTimes[noKilledVictims].currentTime);
                    return path;
                }
            }
            Debug.LogWarning("Error in name matching when creating character path");
            return null;
        }
    }
    public List<Weapon> GetWeapons()
    {
        return this.AllWeapons;
    }
    public List<Character> GetCharacters()
    {
        return this._characterList;
    }
    public List<ConcreteLocation> GetLocations()
    {
        return this._locationList;
    }
}

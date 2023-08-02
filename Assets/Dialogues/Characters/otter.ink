VAR victim = ""
VAR weapon = ""
VAR friendly = "friendly"
VAR neutral = "neutral"
VAR hostile = "hostile"
VAR current_emotion = "friendly"
VAR time = ""
VAR location = ""
VAR first_time = true
VAR blocked_off = false
VAR forgotWeapon = false
VAR forgotLocation = false
VAR location_repeats = 4
VAR counter = 0
VAR isInLoop = false
VAR realWeapon = ""
VAR isGuilty = false
VAR enoughEvidence = false
-> Investigate
== Questioning ==
{
    - first_time == true:
    {
        - current_emotion == friendly:
            ~first_time = false
            #character:Otter #speed:0.03,0 #state:O_Idle_A
            Hmm. Some words of wisdom, little Fox. Escape that joker you call a master, and forge your own path. 
            Yes, I know. {victim} was murdered. How could I let this happen...?
            #state:O_Idle_H
            Right. You're a dectetive. I hope you're able to solve this.
        - current_emotion == neutral:
           ~first_time = false
           #character:Otter #speed:0.03,0 #state:O_Idle_N
            Hello, Fox.
            #state:O_Idle_A
            I was careless. {victim} is dead. How can I live with myself?
            #state:O_Idle_N
            You'd best be careful.
        - current_emotion == hostile:
            ~first_time = false
            #character:Otter #speed:0.03,0 #state:O_Idle_A
            So, the investigators are hard at work. Thrilling.
            #state:O_Idle_C
            Pointing fingers already? I ought to throw you insufferable mutts overboard!
            Get your sorry ass out of my sight!
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:O_Idle_H
        {What would like to discuss? | Anything else?}
    -current_emotion == neutral:
    #state:O_Idle_N
        {Is there something you needed? | What else?}
    -current_emotion == hostile:
    #state:O_Idle_H
        {Lords, what do you want now? | Spit it out.}
}
    + [Were you carrying something last night?]
    {
        - blocked_off == true:
            ->BlockedOff
        - blocked_off == false:
            ->Weapon
    }
    + [Can you recall your movements last night?]
    {
        - blocked_off == true:
            ->BlockedOff
        - blocked_off == false:
            ->Location
    }
    + [No, That's all.]->Goodbyes
    

== Weapon ==
    {
        - current_emotion == friendly:
            {
                - forgotWeapon == false:
                #state:O_Idle_N
                {<i>*sigh*</i>, Very well, only because I've nothing to hide. | Do you have wax in your ears?}
                I've got a {weapon}, for my own reasons.
                Relax, I would never think of the sort.
                - forgotWeapon == true:
                #state:O_Idle_N
                    I don't remember.
                    This old brain ain't worth a damn thing...
                    I have lots of things.
                    What? Need to conduct a search now?
                    Hurry this up, I haven't all day.
            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:O_Idle_N
                {You hard at work? So is everyone. | You deaf or something?}
                Yes, I have a {weapon}.
                I needn't explain myself.
                - forgotWeapon == true:
                #state:O_Idle_N
                Hmm. I'm not sure.
                I would never, though.
            }
        - current_emotion == hostile:
            {
                - forgotWeapon == false:
                #state:O_Idle_N
                I have a {weapon}.
                #state:O_Idle_A
                { You'll be a sitting target without your guardian. | Leave me the hell alone! }
                I've nothing more to discuss.
                - forgotWeapon == true:
                #state:O_Idle_N
                I didn't. Why would I?
            }
    }
    ->MainBranch
== BlockedOff ==
#state:O_Idle_A
    {Beat it! | Keep sqwaking and I'll rip out that tongue! }
    ->MainBranch
== Location ==
~isInLoop = true
{
    -current_emotion == friendly:
    #state:O_Idle_H
        {Sure. Let me see... | You're really thorough}
#state:O_Idle_A
    -current_emotion == neutral:
        {Fine.| Again?!}
        #state:O_Idle_A
    -current_emotion == hostile:
        {<i> growls </i> You're questioning me? | You're testing my patience.}
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:O_Idle_N
    {&I went|I also went|I did go} to {location} at around {time}.
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        #state:O_Idle_A
        I can't remember everything - you think my mind is as fresh as yours?!
    }
    #state:O_Idle_N
    That should be all my movements last night. ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:O_Idle_H
    Head my warnings, and find peace.
    -current_emotion == neutral:
    #state:O_Idle_H
    Farewell, and good luck.
    -current_emotion == hostile:
    #state:O_Idle_A
    The sooner this ends the better.
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
An imposing figure for sure. He's got a long history from what my mentor says though, so I can understand his roughness. Does he always have to frown?
->DONE

== Investigate ==
#character:Fox #speed:0.03,0
Captain Otter steps away from his quarters. This is your chance.
You quickly pounce towards his belongings to find clues.
...Let's see here: An old compass, tobacco chew, pocket watch. He's a guy with few possessions.
This isn't going anywhere.
You could distract Captain Otter and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Captain Otter for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
The Captain has a {realWeapon} on his person.
Good to know... did he lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Otter #speed:0.03,0 #state:O_Idle_H
{
   - isGuilty == true && enoughEvidence == true:
   
        Playing your hand, eh?
	This should be good. What have you got?
	That evidence means nothing.
	... ... ...
	#state:O_Idle_N
	Everyone thinks it too, huh?
	#state:O_Idle_A
	Fine, you got me, <i>detectives</i>.
	#state:O_Idle_C
	It was I. I did it.
	You wouldn't understand. You, Mr. Fox, were too scared to even fight.
	Those screams of terrors. I hear them in my sleep...
	They tell me things at night. Awful things...
	I see blood everywhere. This world will rot in ash and brimstone.
	We are all just puppets to evil. All of us. 
	I'm ending my suffering - everyone's suffering.
	Take me away, if you must.	
	None of you are safe.
    - isGuilty == true && enoughEvidence == false:
    #state:O_Idle_H
        The little boy has a theory.
        #state:O_Idle_A
	Too bad it's wrong. All of this "evidence" is a load of shit!
	Oy, Mutt, your pup ought to put a muzzle on.
	Anything more out of his mouth and he'll be minced meat!	
    - isGuilty == false:
    #state:O_Idle_A
        Pointing fingers is all you're good at!
	You disgust me - I've done <i>nothing</i> wrong!
	I am leading this ship, and this is how you treat me?!
	Oy, Mutt, put your pup on a leash - it needs more training! I want you off my ship!
}
->END
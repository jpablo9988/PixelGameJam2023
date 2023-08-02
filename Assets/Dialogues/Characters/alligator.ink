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
            #character:Alligator #speed:0.03,0 #state:A_Idle_H
            ... Little Fox, you know what has happened. 
            #state:A_Idle_A
            {victim} is dead...	
            #state:A_Idle_N
	    I have faith...		
        - current_emotion == neutral:
        #character:Alligator #speed:0.03,0 #state:A_Idle_N
           ~first_time = false
            ...
            #state:A_Idle_A
	    {victim} is dead. It's horrible.
	    #state:A_Idle_N
	    Get to work...
        - current_emotion == hostile:
        #character:Alligator #speed:0.03,0 #state:A_Idle_A
            ~first_time = false
            <i>*growls*</i>
            #state:A_Idle_C
           <i>*licking lips*</i> He tasted <i>gooood</i>. 
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:A_Idle_H
        {Fox. How can... I help? | ...Something more?}
    -current_emotion == neutral:
    #state:A_Idle_N
        {...Yes? | ...}
    -current_emotion == hostile:
    #state:A_Idle_A
        {...<i>*growls*<i> | ...}
}
    + [Were you carrying any type of weapon when {victim} was murdered?]
    {
        - blocked_off == true:
            ->BlockedOff
        - blocked_off == false:
            ->Weapon
    }
    + [Can you recall where you where were you last night?]
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
                #state:A_Idle_H
                {I will... comply. | ...again?}
                I have... {weapon}...
                - forgotWeapon == true:
                #state:A_Idle_N
                    I don't remember...
	   	    ...
            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:A_Idle_N
                {Okay. | ...}
                I have {weapon}.
                - forgotWeapon == true:
                #state:A_Idle_N
                I... I don't know.
            }
        - current_emotion == hostile:
            {
                - forgotWeapon == false:
                #state:A_Idle_N
                I have... {weapon}.
                #state:A_Idle_C
                { You look delicious... | ... }
                #state:A_Idle_N
                I have no more words.
                - forgotWeapon == true:
                #state:A_Idle_N
                <i>*shakes head*<i>
            }
    }
    ->MainBranch
== BlockedOff ==
    {You haven't much time left... | <i>*growls*<i> }
    ->MainBranch
== Location ==
~isInLoop = true
{
    -current_emotion == friendly:
    #state:A_Idle_H
        {You remind me... of him... | Search me...}
    -current_emotion == neutral:
    #state:A_Idle_N
        {Okay...| }
    -current_emotion == hostile:
    #state:A_Idle_A
        {<i> growls </i> Watch yourself. | Hold that thought...}
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:A_Idle_N
    {&I went...|I also went...|I did... go...} to {location} at around {time}...
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        #state:A_Idle_N
        I don't remember...
    }
    
    That's all... ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:A_Idle_H
    I hope... to see you again...
    -current_emotion == neutral:
    #state:A_Idle_N
    Farewell...
    -current_emotion == hostile:
    #state:A_Idle_C
    ...
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
My mentor knows him, but he always gets embarrassed to tell me more about him though. This guy doesn't talk much, I wonder what's on his mind.
->DONE

== Investigate ==
#character:Fox #speed:0.03,0
Mr. Alligator lumbers away for a minute. Where's he going, you might not wanna know. Here's your chance!
You quickly pounce towards his belongings to find clues.
...Let's see here: A first aid kit, a pair of bifocals, and... a stuffed wolf plush? 
This isn't going anywhere.
You could distract Mr. Alligator and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Captain Otter for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
Mr. Alligator has a {realWeapon} on his person.
Good to know... did he lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Alligator #speed:0.03,0 #state:A_Idle_N
{
   - isGuilty == true && enoughEvidence == true:
       	...
    #state:A_Idle_N
	It's true.
	It was I.
	#state:A_Idle_A
	I did it... to end your sorrow...
	I hear it. The pain... the tears...
	I hear it every night...
	I wanted a purpose... to fight for good... 
	I did it all... for you... 
	You left me...
	You didn't even tell me... how you felt...
	...
	End this now. I deserve this...
	I'm sorry...
	I still... can't say it...
	I'm sorry...
	I will... see you again, soon...	
    - isGuilty == true && enoughEvidence == false:
    #state:A_Idle_C
        ...
        #state:A_Idle_A
	You tear open my flesh... and see nothing...
	Evidence you got is wrong...
	#state:A_Idle_C
	I will remember this, little Fox...		
    - isGuilty == false:
    #state:A_Idle_A
        ...
	You hurt me, Fox...
	You know nothing of me...
	I wanted to... belive in you...
}
->END
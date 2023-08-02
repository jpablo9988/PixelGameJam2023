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
            #character:Cat #speed:0.03,0 #state:C_Idle_N
            Good day Master Fox! I've no doubt you've heard of ill news. 
	    {victim} is dead. This is... most disturbing.
	    #state:C_Idle_H
	    I wish you well in your investigative work.
        - current_emotion == neutral:
           ~first_time = false
           #character:Cat #speed:0.03,0 #state:C_Idle_N
            Greetings, Master Fox. This is terrible, 
	    {victim} has been murdered. But you're on the case, thankfully.
	    Make swift justice, please!
        - current_emotion == hostile:
            ~first_time = false
            #character:Cat #speed:0.03,0 #state:C_Idle_N
            Ah, yes, good day.
            I see you're hard at work. That is good...
            #state:C_Idle_A
	    I feel a terrible headache.
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:C_Idle_H
        {Master Fox! Is there anything I can do for you? | Any other inquiries you have?}
    -current_emotion == neutral:
    #state:C_Idle_N
        {Why hello. What will you need? | Is that sufficient?}
    -current_emotion == hostile:
    #state:C_Idle_N
        {Very good, very good. | Is there anything else?}
}
    + [Were you carrying a weapon last night?]
    {
        - blocked_off == true:
            ->BlockedOff
        - blocked_off == false:
            ->Weapon
    }
    + [Can you recall where you were you last night?]
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
                #state:C_Idle_H
                {Of course, happy to help!| Come again?}
                #state:C_Idle_N
                Yes, I do possess the {weapon}
		I simply feel more safe with this, we all need protection.
                - forgotWeapon == true:
                #state:C_Idle_A
                    This is vexing, I cannot remember.
                    #state:C_Idle_N
		    Terribly sorry, sir, but this fails me.
	   	    If you need to conduct a search, very well.

            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:C_Idle_N
                {Of course. | I'll repeat myself.}
                I do have a {weapon}
		Do not think harshly of me.
                - forgotWeapon == true:
                #state:C_Idle_N
                I'm sorry, sir, but I don't seem to recall this.
            }
        - current_emotion == hostile:
            {
                - forgotWeapon == false:
                #state:C_Idle_N
                I have the {weapon}.
                #state:C_Idle_A
                { Now, if you'll please excuse me. | If you don't mind, do not ask again, please. }
                #state:C_Idle_C
                I've spoken too much already.
                - forgotWeapon == true:
                #state:C_Idle_N
                I don't recall.
            }
    }
    ->MainBranch
== BlockedOff ==
#state:C_Idle_N
    {Pardon me, Master Fox, I'm terribly busy. | Apologies, I've nothing to offer. }
    ->MainBranch
== Location ==
~isInLoop = true
{
    -current_emotion == friendly:
    #state:C_Idle_H
        {At your service, Master Fox, I'll assist you. | I'll say it again, alright?}
    -current_emotion == neutral:
    #state:C_Idle_N
        {I'll be of assistance. | I'll repeat myself. }
    -current_emotion == hostile:
    #state:C_Idle_A
        {Right, let us begin. | Let's not continue this, please.}
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:C_Idle_N
    {&I went|I also went|I did go} to {location} at around {time}.
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        #state:C_Idle_N
       I seem to have a lapse in memory, my apologies.
    }
    #state:C_Idle_N
    That was where I was at. ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:C_Idle_H
    If you need anything else I'll be happy to assist!
    -current_emotion == neutral:
    #state:C_Idle_N
    Goodbye, stay safe, Master Fox.
    -current_emotion == hostile:
    #state:C_Idle_C
    Goodbye.
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
I'm a little glad to see someone close to my age! I'm always used to older company, so we'll be able to talk amicably! 
->DONE

== Investigate ==
#character:Fox #speed:0.03,0
Mr. Cat scurried off to get some more refreshments. Time to start searching!
You quickly pounce towards his belongings to find clues.
...Let's see here: Old poetry, spare bowties, and a recorder! He's musical, that's neat! 
This isn't going anywhere.
You could distract Mr. Cat and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Mr. Cat for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
Mr. Cat has a {realWeapon} on his person.
Good to know... did he lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Cat #speed:0.03,0 #state:C_Idle_N
{
   - isGuilty == true && enoughEvidence == true:
   #state:C_Idle_N
       	Well, this is quite the turn of events.
	So you propose that it was I?
	#state:C_Idle_H
	Very well, detective Fox, present yourself.
	...I see.
	#state:C_Idle_N
	Everyone else thinks so, as well...
	#state:C_Idle_C
	Very well. You found me.
	You want the truth? I have a hitlist.
	I'm not a mere waiter aboard a ship of freaks.
	I'm hired by a vast organization.
	You were easy targets, it was just my luck!
	I'm hired by a vast organization.
	{victim} was their number 1 target.
	Are you daft? I'm saying who. You already know too much.
	I was hoping to end you all swifty. You detectives are pieces of work all right.
	It matters not. I'm not alone in this. None of you are safe.		
    - isGuilty == true && enoughEvidence == false:
    #state:C_Idle_N
        Oh, this is interesting.
    #state:C_Idle_A
	Tell me, then, <i>master</i>, you have evidence of this?
	I see. Nothing here is conclusive.
	For such an expert in crime solving you haven't a single lead.
	How sad is this. People more qualified than you should be here!		
    - isGuilty == false:
    #state:C_Idle_A
        The absolute nerve!
	I was hoping that we would become friendly on this trip!
	This is you throw around at me!?
	Pathetic. You have no business being here!
}
->END
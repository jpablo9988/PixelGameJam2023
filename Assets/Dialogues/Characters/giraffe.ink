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
            #character:Giraffe #speed:0.03,0 #state:G_Idle_H
            Hello there, Fox, it is lovely outside.
            #state:G_Idle_N
	    I had a bad feeling about this "vacation".
	    {victim} is dead. This is going to haunt me.
	    #state:G_Idle_H
	    You're a crime solver, right? I hope you figure this out.
        - current_emotion == neutral:
           ~first_time = false
           #character:Giraffe #speed:0.03,0 #state:G_Idle_N
           Hello Fox.
	    {victim} is dead. I knew I shouldn't have come...
	    Can you figure this out?
        - current_emotion == hostile:
            ~first_time = false
            #character:Giraffe #speed:0.03,0 #state:G_Idle_N
            Hello.
            #state:G_Idle_A
            You're awfully calm about this.
            #state:G_Idle_N
	    I need some fresh air.
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:G_Idle_H
        {Sure thing, what can I do for you? | Uh-huh, anything else?}
    -current_emotion == neutral:
    #state:G_Idle_N
        {How can I help? | Mm-hmm, something more?}
    -current_emotion == hostile:
    #state:G_Idle_A
        {Oh, what is it? | Can we get a move on?}
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
                #state:G_Idle_N
                {I suppose this is the part you investigate. | You want me to say that again?}
                I have a {weapon} with me.
		Don't be scared, it's always good to arm yourself.
                - forgotWeapon == true:
                #state:G_Idle_N
                    Oh, I don't remember. Sorry.
		    Slipped my mind, honestly. 
		    You can go ahead and look around, I guess.
		    Don't take too long.

            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:G_Idle_N
                {Comes with your job description, I know. | Don't be doing this too much.}
               	I got a {weapon}.
		What's that look for? You would need to defend yourself too.
                - forgotWeapon == true:
                #state:G_Idle_N
               	Can't say I remember.
            }
        - current_emotion == hostile:
            {  
                - forgotWeapon == false:
                #state:G_Idle_N
                I got {weapon}, if you must know.
                #state:G_Idle_A
                { You could use something similar too, boy. You're helpless as is. | That it? }
                #state:G_Idle_N
                Are we done here?
                - forgotWeapon == true:
                Beats me.
            }
    }
    ->MainBranch
== BlockedOff ==
#state:G_Idle_A
    {I need some time to myself. | I got nothing for yuh. }
    ->MainBranch
== Location ==
~isInLoop = true
{

    -current_emotion == friendly:
    #state:G_Idle_H
        {Sure, I'll help. | Ask me again.}
    -current_emotion == neutral:
    #state:G_Idle_N
        {Sure thing. | Come on, come on. }
    -current_emotion == hostile:
    #state:G_Idle_A
        {Uh huh. | I think we're done here. }
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:G_Idle_N
    {I went|I also went|I did go} to {location} at around {time}.
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        #state:G_Idle_N
      I don't remember, really.
    }
#state:G_Idle_N
    That's where I was today. ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:G_Idle_H
    See you soon, and take care, okay?
    -current_emotion == neutral:
    #state:G_Idle_N
    Until next time, take care.
    -current_emotion == hostile:
    #state:G_Idle_A
    Alright, alright.
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
I've heard a lot about her! I wonder if she tires of all the people wanting her attention. I don't mean to be intrigued but I hope to know her better on this trip. 

->DONE

== Investigate ==
#character:Fox #speed:0.03,0
Ms. Giraffe went out for a stretch. This will be your perfect chance!
You quickly pounce towards her belongings to find clues.
...Let's see here: A handy book on aircraft, several neck pillows, and lollipops! Aw, she has a sweet tooth!
This isn't going anywhere.
You could distract Ms. Giraffe and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Ms. Giraffe for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
Ms. Giraffe has a {realWeapon} on her person.
Good to know... did she lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Giraffe #speed:0.03,0 #state:G_Idle_A
{
   - isGuilty == true && enoughEvidence == true:
   #state:G_Idle_A
       	You little runt! Trying to accuse me of such a thing!
	You think you're so smart! 
	What did you find?!
	... tch.
	You all think so too.
	#state:G_Idle_C
	Of course I'm caught.
	Look, it's nothing personal. 
	#state:G_Idle_A
	Why did that Wench Puffin has to invite us to this...!
    I shouldn't have listened to you! I had a bad feeling from the start!
    #state:G_Idle_C
	I lashed out, I gave into my inner demons. Nothing else to it.
	Left and right they treat me like I'm the biggest celebrity on Earth.
	I'm never left alone, I get hounded no matter where I go!
	Wouldn't have this be a good opportunity for me to disappear? To find peace?
	You were all just casualties. 
	It's an unspeakable act. I know. 
	Look, take me to prison. I'll be left alone there, at least.
		
    - isGuilty == true && enoughEvidence == false:
    #state:G_Idle_A
       	... Oh, how rich is this.
	You couldn't even point your finger at me without sputtering.
	What a sorry excuse of a detective you are!
	This evidence? You're just going in circles!
	#state:G_Idle_N
	Now you've done it, and everyone else agrees.
	#state:G_Idle_C
	Watch your back, I'm not one to be messed with.

    - isGuilty == false:
    #state:G_Idle_A
        The hell are you on about!?
	Is this your integrity, calling people out for nothing?
	You're a sad excuse for a detective, you know that?
	I bet I could be more competant at your own damn job!
	Get this quack out of here! I'm done with him!
	
}
->END
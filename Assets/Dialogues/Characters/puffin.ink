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
            #character:Puffin #speed:0.03,0 #state:P_Idle_H
            How good it is to see you, Fox! Looking dashing as ever!
            #state:P_Idle_A
	    Yes, I'm aware... {victim} is dead. I did not want things to go this way!
	    #state:P_Idle_H
	    I believe in you, Fox!
        - current_emotion == neutral:
           ~first_time = false
           #character:Puffin #speed:0.03,0 #state:P_Idle_N
            I wish we could've convened on more fortunate circumstances.
	    Most tragically, {victim} was killed. By one of us, no less. The thought pains me.
	    Please, shed some justice.
        - current_emotion == hostile:
        #character:Puffin #speed:0.03,0 #state:P_Idle_N
            ~first_time = false
            Hello there, Fox.
            You're fast at work. That is good news, at least.
            #state:P_Idle_A
	    I must be off, this is making me feel terrible.
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:P_Idle_H
        {My dear, what can I help you with? | Yes, is there anything else you wish?}
    -current_emotion == neutral:
    #state:P_Idle_N
        {Mr. Fox, is there something you need? | Is there anything else you needed?}
    -current_emotion == hostile:
    #state:P_Idle_N
        {Ah, good to see you. | Is there anything else?}
}
    + [Were you carrying a weapon last night?]
    {
        - blocked_off == true:
            ->BlockedOff
        - blocked_off == false:
            ->Weapon
    }
    + [Can you recall where you were last night?]
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
                #state:P_Idle_H
                {<i>*giggle*<i> Oh, forgive me, it warms me to see you hard at work!| Should I say that again?}
                Of course, I carry {weapon} with me.
		It isn't easy being a woman, you know.
                - forgotWeapon == true:
                #state:P_Idle_N
                    How daft, I don't remember!
		    Well, right, that is...
	   	    I suppose I'll allow you to investigate. 
	   	    #state:P_Idle_H
		    Don't ogle too much, you hear!

            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:P_Idle_N
                {I'll let you play your part! | Let me say that again.}
               	I do have the {weapon} on me.
		It's only safe of me to do so.
                - forgotWeapon == true:
                I'm so forgetful, I cannot recall this.
            }
        - current_emotion == hostile:
            {
                - forgotWeapon == false:
                #state:P_Idle_N
                I do have {weapon}.
                #state:P_Idle_A
                { Right, if that's all, let us move on from this. | I belive that's all. }
                - forgotWeapon == true:
                #state:P_Idle_N
                Excuse me, I do not remember.
            }
    }
    ->MainBranch
== BlockedOff ==
#state:P_Idle_N
    {I'm sorry, dear, I think I'll need a moment.| Unfortunately I have nothing else to say at this time. }
    ->MainBranch
== Location ==
~isInLoop = true
{
    -current_emotion == friendly:
    #state:P_Idle_H
        {Do not mind, dear, let me help you! | I don't mind, really.}
    -current_emotion == neutral:
    #state:P_Idle_N
        {I'll be hapy to help. | Let's do that again. }
    -current_emotion == hostile:
    #state:P_Idle_N
        {Yes, your work. | I think that's all for us to discuss.}
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:P_Idle_N
    {&I went|I also went|I did go} to {location} at around {time}.
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        #state:P_Idle_N
      Dear me, I cannot remember. I will try to later on, perhaps.
    }
#state:P_Idle_H
    I believe that concludes my outing today. ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:P_Idle_H
    Just say the word, sweetie, and I'll be happy to help you!
    #state:P_Idle_N
    -current_emotion == neutral:
    Oh, please do be careful. It's gotten quite dangerous now.
    -current_emotion == hostile:
    #state:P_Idle_N
    Best of luck, dear.
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
It's always a little embarrassing when she calls me cute as if I'm some child. She is nice to me, though, but not to my mentor. Nice of her to invite us for a vacation!
 
->DONE

== Investigate ==
#character:Fox #speed:0.03,0
Ms. Puffin has gotten up to use the powder room. This is your chance to look around!
You quickly pounce towards her belongings to find clues.
...Let's see here: Exotic-looking scarves, architecture magazines, and blank checkbook. Well, I suppose her taste must be high class. 
This isn't going anywhere.
You could distract Ms. Puffin and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Ms. Puffin for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
Ms. Puffin has a {realWeapon} on her person.
Good to know... did she lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Puffin #speed:0.03,0 #state:P_Idle_A
{
   - isGuilty == true && enoughEvidence == true:
   
        How dare you excuse such a thing upon me?!
	I was the one to organize a <i>vacation</i>! 
	Evidence of what?!
	...!
	#state:P_Idle_C
	Have you all caught on?!
	#state:P_Idle_H
	You were correct, after all, boy. 
	#state:P_Idle_N
	Listen closely.
	#state:P_Idle_A
	You are my enemies. You used me.
	You took advantage of my kindness. My loneliness. My sorrow.
	#state:P_Idle_C
	This was a trap. A cruise disappears in the middle of the ocean.
	A lone widow is the sole survior. No one could suspect a thing.
	I rid myself of you heathens and you all would be at the bottom of the sea for eternity.
	A new life for myself!
	I was close, too. You were next, boy, as much as it would sadden me.
	A pity you had to be with that conniving bastard!
		
    - isGuilty == true && enoughEvidence == false:
    #state:P_Idle_A
       	After all I've done for you, and this is how you repay me?!
	All men are the same!
	Evidence...? What you have doesn't even make any sense!
	None of this could possibly add up!
	#state:P_Idle_N
	Everyone is in agreement.
	#state:P_Idle_A
	You will pay for this.	
    - isGuilty == false:
    #state:P_Idle_A
        How could you say such a thing!
	My idea was of a delightful vacation!
	This was the last thing I wanted!
	I've been nothing but kind and this is how you act!?
	To think I had faith in your future. 
	My attorneys will not be happy to hear this muckraking.
	
}
->END
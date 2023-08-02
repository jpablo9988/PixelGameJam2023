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
            #character:Wolf #speed:0.03,0 #state:W_Idle_H
            Thank heavens you’re here, Fox! I hope you've heard about the recent news?
            #state:W_Idle_N
            {victim} was killed last night, my friend. What a tiresome trip this is. I don't even know if we'll arrive to London.
            #state:W_Idle_H
            Already investigation, huh? You're on top of things. Good work!
        - current_emotion == neutral:
           ~first_time = false
           #character:Wolf #speed:0.03,0 #state:W_Idle_N
            Good tidings, Fox. You've heard the recent news, right?
            #state:W_Idle_A
            {victim} is ...dead. What a tragedy that is, indeed.
            #state:W_Idle_H
            Good to see you're doing well atleast, my friend. 
        - current_emotion == hostile:
            ~first_time = false
            #character:Wolf #speed:0.03,0 #state:W_Idle_A
            Oh... Mr. Fox, I must commend you for being proactive.
            #state:W_Idle_N
            Already questioning the suspects, huh?
            #state:W_Idle_A
            I'm going to be honest, I'm not feeling too well...
    }
}
-> MainBranch
== MainBranch ==
{
    -current_emotion == friendly:
    #state:W_Idle_H
        {Tell me, now. Do you have any questions for me? | Something else, friend?}
    -current_emotion == neutral:
    #state:W_Idle_N
        {What can I do you for? | Anything Else?}
    -current_emotion == hostile
    #state:W_Idle_A
        {Could you make this brief? | Hurry up, I don't got all day.}
}
    + [Were you carrying a weapon last night?]
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
                #state:W_Idle_H
                {<i>*chuckle*</i>, I suppose Giraffe is right - you’re too adorable for this. | Hey, I don't mind repeating myself...}
                #state:W_Idle_N
                Yes, I have something lethal in my person, it's a {weapon}.
                Don't trouble yourself. I've done nothing.
                - forgotWeapon == true:
                #state:W_Idle_N
                    Yes... what was it again?
                    I'm so sorry, but I don't think I had?
                    You know me, I carry stuff and I tend to forget I have it.
                    You can investigate my belongings if you need to.
                    Might take some time, though...
            }
        - current_emotion == neutral:
           {
                - forgotWeapon == false:
                #state:W_Idle_H
                {Silly me, I forget you’re a detective yourself sometimes! | You need to hear me again?}
                #state:W_Idle_N
                Yes, I have a {weapon}, friend.
                #state:W_Idle_C
                It's for a case, you understand.
                - forgotWeapon == true:
                #state:W_Idle_N
                Oh, no... no, I think I have something lethal.
                #state:W_Idle_A
                Even if I had, I didn't do it.
            }
        - current_emotion == hostile:
            {
                - forgotWeapon == false:
                #state:W_Idle_N
                I have a {weapon}.
                #state:W_Idle_A
                { I will use it on you if you continue talking to me. | Now stop asking, OK?}
                #state:W_Idle_C
                I'm just kidding, of course! Still, I'm feeling seasick, it's horrible.
                - forgotWeapon == true:
                #state:W_Idle_A
                I didn't. Why would I?
            }
    }
    ->MainBranch
== BlockedOff ==
#state:W_Idle_A
    {I'm sorry, I really don't feel like talking right now. | I have nothing to say to you, friend.}
    ->MainBranch
== Location ==
~isInLoop = true
{
    -current_emotion == friendly:
    #state:W_Idle_H
        {Anything for my apprentice. Let's see now... | I'll repeat myself until you got it memorized!}
    -current_emotion == neutral:
    #state:W_Idle_N
        {Sure, I can do that. | Again? OK...}
    -current_emotion == hostile:
    #state:W_Idle_A
        {<i> sigh </i> OK, let's get this over with.| You wan't to hear it again? | Friend, you're starting to annoy me.}
}
->LocationLoop
== LocationLoop ==
{
   
    - counter < location_repeats:
    ~counter++
    #state:W_Idle_N
    {&I went|I also went|I did go} to {location} at around {time}.
    ->LocationLoop
    - counter >= location_repeats:
    ~isInLoop = false
    ~ counter = 0
    {
        - forgotLocation == true:
        I think I forgot couple of stuff. Sorry about that.
    }
    
    That should be all my movements last night. ->MainBranch
}
== Goodbyes ==
{
    -current_emotion == friendly:
    #state:W_Idle_H
    Well. Go get 'em, tiger. Trust in what I taught you! Good luck.
    -current_emotion == neutral:
    #state:W_Idle_H
    Goodbye then, my friend. Take care, please.
    -current_emotion == hostile:
    #state:W_Idle_N
    Sure. Goodbye. I will lie down for a little bit.
}
->DONE
== Check ==
#character:Fox #speed:0.03,0
That's my mentor! He's taught me lots of things and I'm very grateful. Something about this trip is making him a little distant though.
->DONE

== Investigate ==
#character:Fox #speed:0.03,0
You see Wolf leave the room for the bathroom. Now is your chance!
You quickly pounce towards his belongings to find clues.
...Let's see here: A journal, extra pipes, a poorly concealed bottle of Whisky. I could use a drink, myself...
This isn't going anywhere.
You could distract Wolf and search for any lethal weapons, but this would consume time.
+ [Do a deeper investigation <i>(This will consume time)</i>]You follow Wolf for some time until you find an opening to investigate...
        ...<i>Some time has passed</i>..
        After some time, you manage to catch a glimpse of something...
        ->RealWeapon
+ [Leave it for now.] ->QuitInvestigation
== RealWeapon ==
Wolf has a {realWeapon} on his person.
Good to know... did he lie about this?
->DONE
== QuitInvestigation ==
Hmm... another opening could happen later on.
->DONE

== Condemnation ==
#character:Wolf #speed:0.03,0 #state:W_Idle_H
{
   - isGuilty == true && enoughEvidence == true:
   #state:W_Idle_A
        Did I hear that right? YOU are accusing ME?
        I'm your teacher for Christ's sake! How could you...?
        What evidence...? Oh.
        #state:W_Idle_H
        Heh. I suppose everyone sees it, then.
        #state:W_Idle_C
        Heh. Heh. Heh. 
	So this is it.
        Shit, I've taught you well, Apprentice Fox. Too well.
        Yeah, I did it. It was me. I'm the bastard.
        Head my words, though. They had it coming!
        #state:W_Idle_A
        Corrupt bunch of bastards, they are. You all are guilty of unimaginable sins!
        Or you don't remember what happened 10 years ago, eh?
        If I wasn't going to kill them one of you would've!
        ...
        #state:W_Idle_N
        Don't pity me, my friend. I am also selfish.
        I know you were on my trail... the thought of killing you.. well...
        It's... nothing personal, of course, you're more important than anything.
       	... You know, I'm glad you caught me. Better you than those hogs at the station.
       	#state:W_Idle_A
        Put me in cuffs. I've had enough of this.
        #state:W_Idle_N
	    Just one thing, Fox: be better. Be better than me.
    - isGuilty == true && enoughEvidence == false:
    #state:W_Idle_A
        You're accusing me!? Your partner!?
        I'm your teacher for Christ's sake! How could you...
        What evidence...?
        The things you're presenting make no sense at all.
        I did my investigation... and this is faulty evidence.
        Don't you all agree?
        #state:W_Idle_N
        Oh well, atleast that should prove that this little fox is wrong.
        #state:W_Idle_H
        And that I am, of course, Innocent.
        #state:W_Idle_C
        Good day, and <i>take care of yourself tonight, little fox.</i>
    - isGuilty == false:
    #state:W_Idle_A
        WHAT? YOU should know better than to wave around faulty accusations!
        I'm your teacher for Christ's sake! How could you...
        I didn't do this, I SWEAR! It wasn't ME!
        You betraying me, FOX, this evidence doesn't point to ANYTHING at all!
        You are pointing your finger to the wind, and that makes YOU...
        <b>S U S P I C I O U S</b>
        Take him away, people! I got some questioning to do.
}
->END




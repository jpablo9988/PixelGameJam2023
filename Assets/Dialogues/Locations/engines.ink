VAR isMurder = false
VAR explored = false

VAR location = ""
VAR time = ""
VAR weapon = ""

VAR thisLocation = ""

VAR showLocation = false
VAR showTime = false
VAR showWeapon = false

VAR victim = ""
->Autopsy
== Investigate ==
{
    -isMurder == false:
    #speed:0.05,0 #character:Fox
        As I enter the engine room, I hear them roar back.
        It's a tiresome sight, but fills me with vigor at the same time.
        Mr. Alligator seems to be in his own world managing the engines... he seems used to it.
        He barely gives me a nod as I greet him. Don't blame him, someone died last night...
        Although something in his eye caught my attention. I can't quite decipher what it is, tho.
        
    -isMurder == true:
    #speed:0.05,0 #character:Fox
        Who would guess that such a huge person could die so suddenly.
        The engines roar at the sight of Mr. Alligator's corpse.
        I wish It wasn't so, but I must find his killer.
        The room itself seems intimidating... I hesitate as I enter.
        ->DONE
}
{
    -showLocation == true && thisLocation == location:
    It seems that the room logs has been tampered with. This can't be good, I can't explore this room any further.
    If I want to find who entered here, I might need to check the neighbouring rooms...
    Someone must've went through them to enter here!
    ->DONE
    
    -showLocation == false || thisLocation != location:
    I could piece together who entered here last night by looking the the room logs, although that could <b>take me some time.</b>
    
    Do I want to explore further?
        + [Explore the Logs <i>(This will consume time)</i>]...<i>Some time has passed</i>..
        ->ExploreRoom
        + [Leave it for now.] ->QuitRoom
    
}
== ExploreRoom ==
~explored = true
Here {are|were} my findings.
->DONE
== AutopsyBody ==
Let's see now...
-> DONE
== QuitRoom ==
Hmm. I can come back later if time allows me to. 
-> DONE

== Autopsy ==
#speed:0.02,0 #character:Fox
{victim}'s body is staring at me. 
{
    -showLocation == true && thisLocation == location:
        I know {victim} was murdered in this room.
    -showLocation == true && thisLocation != location:
        {victim} was murdered in {location}. I can see a small hint of drag marks around the body.
    -showTime == true:
        {victim} was murdered at around {time}.
    -showWeapon == true:
        I know {victim} was murdered by (a) {weapon}!
}
I could do a deeper autopsy, but this could consume time. 
+ [Do a deeper autopsy <i>(This will consume time)</i>]...<i>Some time has passed</i>..
        ->AutopsyBody
+ [Leave it for now.] ->QuitRoom
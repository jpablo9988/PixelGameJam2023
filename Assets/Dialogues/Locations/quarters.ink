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
    #speed:0.02,0 #character:Fox
        The quarters of the ship. As this is currently a skeleton crew voyage, only a room seems occupied.
        The rest are closed...they seem to be locked. 
        As I enter the only unlocked room, I greet my half-inebriated mentor.
        All sorts of peculiar items lay over the messy decor. It's Wolf's room alright.
        He gives me a quippy greeting before taking a drink of his very illegal liquor. Not that I care, of course.
        
    -isMurder == true:
    #speed:0.05,0 #character:Fox
        I can't stand to see this sight. My mentor is dead.
        It was an enormous shock last night when I found his body. I almost can't stand it.
        My grief is palpable... but I promise you, Mr. Wolf, I will find your killer!
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
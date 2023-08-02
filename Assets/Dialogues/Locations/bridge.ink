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
        The captain's bridge. Through the window I can see the expanding horizon.
        The captain is rocking the wheel back and forth... he seems anxious.
        I give a solemn nod as I enter his cabin. It's quite cozy, if a bit cramped.
        
    -isMurder == true:
    #speed:0.05,0 #character:Fox
        It is a gruesome sight for sure. The poor captain lies dead on the wheel.
        The horizon seems endless, overwhelmingly so. The once cozy cabin has become a dreadful place.
        {victim}'s body stares at the ceiling. Gives me the creeps.
        Although it's not readily apparent they were killed here.
        If they were, they did a good job cleaning up the blood...
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
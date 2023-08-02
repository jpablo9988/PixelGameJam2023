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
        Ah, the deck. The fresh air is almost overwhelming.
        The wooden boards creak as I walk on the deck's halways.
        The sounds of seagulls flutter as I meet the gaze of the Giraffe.
        She gives me a howdy as I walk near. She seems distant.
        
    -isMurder == true:
    #speed:0.05,0 #character:Fox
        The decks of the ship mourn the death of the great traveler.
        I don't know how sensational the headlines will be <i>if</i> we arrive to london.
        Ms. Giraffe's corpse has been covered with a tarp. The least we can do...
        No one has had the time to drag her inside. Or had the will to do, atleast.
        The tarp does lack actual blood. What could this mean?
        ->DONE
}
{
    -showLocation == true && thisLocation == location:
    It seems that the room logs has been tampered with. This can't be good, I can't explore this room any further.
    ->DONE
    
    -showLocation == false || thisLocation != location:
    I could piece together who entered here last night by looking the the room logs, although that could <b>take me some time.</b>
        If I want to find who entered here, I might need to check the neighbouring rooms...
        Someone must've went through them to enter here!
    
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
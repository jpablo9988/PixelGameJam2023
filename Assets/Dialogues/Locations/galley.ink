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
        The kitchen galley. It seems to be well stocked, would last anyone months before they ran out of food.
        Weirdly enough, there seems to be no knives anywhere, only plastic ones. The same is true of any sharp object.
        This was also true when we first entered this voyage. Someone takes security seriously.
        The Cat is watching me intently as I explore the scene. The lights start to flicker.
        I nod but get no reaction. Can you stop staring, man?
        
    -isMurder == true:
    #speed:0.05,0 #character:Fox
        The kitchen galley has seen better days. All matter of food is thrown around, as is any tools.
        Weirdly enough, there seems to be no knives anywhere, only plastic ones. The same is true of any sharp object.
        This was also true when we first entered this voyage. Still, someone was murdered...
        {victim}'s body stares at me. I feel uncomfortable.
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

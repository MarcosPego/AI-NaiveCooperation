How to run application:
Go to AasmaProjectBuilt\AasmaProject.exe and run the file
In the main menu please be sure to go to options menu and mess with the slider a bit to assign the alpha value
Go back
Click Play and select the desired level

Controls:
"o" and "p" - can use them to change between play mode and obeserver mode, advised not to use those
"t" - change between the naive prespective and the observer prespective
"r" - restart the current level
"escape" - exit the application

Disclaimer:
1 - this is only a built version of the project
To check the unity project version with all de source code use this link:

2-There is a small bug in level 1 where the protector block will sometimes glitch itself into launching itself into space.
This bug is exclusive to the application not being able to reproduce it in the unity game editor. 
Either way the bug has no impact in the game a part from the fact of looking a bit silly.
The glitch is obsevable in the video.


Source code:
This is all the code used in the project implementation

The main scripts of the AI behaviour is the files
SourceCode/Scripts/naiveAgent.cs 
SourceCode/Scripts/protectorAgent.cs 
The other files are related to the game interaction, however the ai files do comunciate with some of these scripts.
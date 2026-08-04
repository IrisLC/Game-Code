# Finite State Machine

When I first got into learning NPC AI beyond simply "Move towards player while shooting" I learned about Finite State Machines through a video by Jason Weimann (The video can be found [here](https://www.youtube.com/watch?v=V75hgcsCGOM)). This is an implementation that I have used throughout a number of unity projects. 

When I started moving my work from Unity over to Unreal Engine one of the first projects I did to better learn the language was to create an implementation of this version of a State machine that I have come to trust in C++.

Provided is:  
    1.The original C# implementation of the state machine made for unity (Found in C# Implementation)  
    2.My personal interpretation of the code into C++. (This was done completely without any guides or tutorials aside from the Unreal Engine 5 Documentation and API) (Found in EnemyAI)

Note: Any naming oddities are due to these files originally being created for their own game projects. All code not pertaining to the state machine has been removed and the basic state methods have been replaced with logs to the engine's console to show that the code runs and changes if the test1 and test2 booleans are adjusted


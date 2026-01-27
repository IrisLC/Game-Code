# GOAP

Files for a custom implementation of a Goal Oriented Action Planning system for Unreal Engine & C++.

This implementation is based on git-amend's Unity GOAP tutorial which can be found [here](https://www.youtube.com/watch?v=T_sBYgP7_2k) with alterations to the original concept being made based on information from various GDC talks about GOAP as well as the original paper on [how GOAP was used in FEAR](https://www.gamedevs.org/uploads/three-states-plan-ai-of-fear.pdf). This is not to mention the alterations made by writing the program in C++ rather than C#.

This project was my biggest forray into C++ to date. While working on this implementation I ended up doing lots of research, learning about the minutia of how C++ handles its code and data types. Not to mention the immense familiarity I quickly gained with the debug tools found in Rider, which I had just switched to as an IDE before starting this project.

## Information about the provided files

The 'EnemyAI' and 'Utility Scripts' files should be put in the same folder and it will mostly be good to go. 

The only files that needs to be altered in any way are the 'GoapAgent' files and the 'Strategy' files.\
The 'GoapAgent' files are the AIController and will need to have various WorldStates, Actions and Goals added as well as optionally references to different sensors and locations added in. These are marked with comments in the .cpp file which contains the common syntax to add WorldStates, Actions and Goals.\
The 'Strategy' Files need a bit more work put into them. When you are adding Actions you will need to create classes that inherit from the IActionStrategies class to give the Agent instructions on what to do to carry out the actions. 


Note: When I have the time I will add a simple example of a GOAP implementation with the 'GoapAgent' files and the 'Strategy' files filled out.
# Games Created

This is the repository contains the build and source code to the project Naive Cooperation for a course of the master's degree in Instituto Superior Técnico.

This project was created for the Autonomous Agents and Multi-Agent Systems course which aimed to test and understand agent cooperation in a game's environment. 
This project was done by two students and the theme and workings of the agent was free to choose. We decided to go with a platformer with two playing characters controlled by AI: 
a naive character that always aims to reach the goal but is unaware of its surrounding dangers and a protector character that always aims to keep the naive character safe from harm but is unaware of were it wants to go.

### Agents


Both agents have a simple reactive movement system composed of a few whiskers that inform the agent of it's surroundings. The Naive agent is aware of the level's endpoint location and will try to move towards it avoiding know obstacles.

### Enemies

These enteties only harm the naive agent and have no effect on the protector agent (besides physics collisions).

Falling Blocks - This is a simple obstacle that slowly rises up and quickly falls down, when the protector agent touches it the block stops completelly until the end of the level.

Spikes - These are spikes that rise and fall from the ground.

SawBlade - A spinning blade that is slightly above the ground.

## Snapshots

The project was composed of 4 levels that increase in difulty each level. The levels always follow a linear path from the left to the right and can have varying degrees of obstacles and challenges.

The first level is simple, it has no variation in elevation and is composed by a falling block, a saw blade and moving spikes.

![level0](https://user-images.githubusercontent.com/24237112/158237397-f9cec4f7-40e8-4a53-b1b8-a7c03d416e81.PNG)

The second level has similar obstacles to the first one but has differences of eleveation and even falling pits.

![level1](https://user-images.githubusercontent.com/24237112/158237370-5dcac666-61cd-4339-988e-3d19264a4332.PNG)

The third one is more complex, it has multiple saw blades on the top of the first half to display that the agents properly understand when some obstacle presents as a danger. 
The next half has three saw blades properly spaced out and finishes with two falling blocks in a row.

![level2](https://user-images.githubusercontent.com/24237112/158237327-7eb5854a-f590-4ba4-89a9-214a9396ff1f.PNG)

The last level was the hardest containing several obstacles and variations of height.

![level3](https://user-images.githubusercontent.com/24237112/158237352-8fb1ba6c-277e-406c-ba58-203f1687515d.PNG)

## Short Demo

Here is a short video displaying the workings of the game
https://www.youtube.com/watch?v=8culmciFKcc&feature=youtu.be

## What I learned

This project as interesting in the conceptualization stand point, it was a tricky problem to solve ensuring that the protector agent could properly inform the naive agent on when to move or stop and ensuring that both agents in general have a good behaviour response system.

Fine tuning the platform obstacles was harder than normal as we had to take in consideration the natural limitations of using agent only charaters.

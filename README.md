# horror-game
Multiplayer game 3D of the horror genre, developed with Unity3d and the Photon engine.

## Lobby
I developed lobby creation through Photon Engine, using its API and RPC calls to synchronize users with the arrival of new players in the lobby and their character choices.

![Alt Text](https://github.com/nunoduartte/horror-game/blob/master/Readme%20files/Intro.gif)

## Objectives and user objects

I used some algorithms to handle user object events (Flashlight, thermometer etc) and achieve goals. RPC calls were used for synchronization between all users on the server.

![Alt Text](https://github.com/nunoduartte/horror-game/blob/master/Readme%20files/Objectives.gif)

## AI Enemy

I used NavMesh to Enemy pathfinding and some algorithms to handle enemy behavior.

![Alt Text](https://github.com/nunoduartte/horror-game/blob/master/Readme%20files/AIEnemy.gif)

## Movement, animation and player states


I used some algorithms to handle character movement and animation, as well as manage their in-game state. All these movement events, animation and character states were synchronized between all users through RPC calls.

![Alt Text](https://github.com/nunoduartte/horror-game/blob/master/Readme%20files/GameOver.gif)

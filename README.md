# *Capture and Contain Division* (GMTK Game Jam 2025)

A first-person action platformer made in four days for the GMTK Game Jam 2025 (theme: "Loop"), in Unity 6.1 with a team of five. I managed the project and was the lead developer, co-programmer, and co-designer.

Draw loops around enemy viruses to trap them in walls, then race to the end of each level to connect the circuit and restore the Cyber Loop. Once every enemy is contained, a node powers up near the start of the level: drag its cable to the matching node at the end to open the door and move on, before the viruses break free.

The game placed in the top 9% for Narrative and the top 14% for Artwork, out of 9,514 entries.

Play it in your browser or download it [on itch.io](https://iains.itch.io/capture-and-contain-division), or read more on the [*Capture and Contain Division* page on my website](https://iain-skaggs.com/capture-and-contain-division-game-jam-project).

## In the code

My code in `Assets/Scripts` includes:
- `Interactions/Player_WallHandler` and `Utilities/GeometryHelper`: the core loop-drawing mechanic. While drawing, the player leaves a trail that uses up a resource. When the trail crosses itself or returns close to its start, the loop is clipped at that point, cleaned of near-duplicate and near-collinear points, and built into a wall. `GeometryHelper` holds the segment-intersection test that finds the crossing, and the point-in-polygon test that checks which enemies are trapped inside.
- `Interactions/WallParent` and `WallSegment`: the finished walls, which settle onto the ground, track the enemies they've captured, and take damage from enemies trying to break out.
- `Enemies/` and `AI/PatrolRoute`: the melee, tank, and ranged enemies, their patrol routes and weapons, and pooled projectiles for the ranged enemy (`Enemies/Weapons/ProjectilePool`), with damage handled through a shared `IDamageable` interface.
- `Game/AudioManager`: the game's audio manager.
- Parts of the level flow (`Game/LevelManager`) and the options menu (`UI/UIOptions`).

The jam build closed a loop only when the trail returned near its start. In the post-jam v0.2.0 update, I reworked the drawing so that a loop also closes wherever the trail crosses itself, and fixed bugs in the wall and enemy systems.

## Opening the project

- Open the project in Unity Hub with Unity 6000.1.13f1 (Unity 6.1), available from [Unity's download archive](https://unity.com/releases/editor/archive), and play from `Assets/Scenes/Final/00_Landing`.
- The project uses URP, the Input System, and AI Navigation.

## A note on the code

This is four-day jam code, with the shortcuts that come with it. For my most recent jam work, see my [GMTK Game Jam 2026 repo](https://github.com/Eirreann/GMTK_GameJam_2026).

## Team

- Critic97: level design and programming
- Skully112: 3D modelling, texturing, and art
- elizabeastxo: narrative design, UI, and supplementary art
- Jordan Skaggs (BlueJayProductions): music and sound effects

# Code Test

A super simple metroidvania style platformer.

## Controls: 
* A/D or Arrow keys to move. 
* Space to Jump
* X to Attack
* R to Revive
* ESC to Pause

## Features:
- Boot scene with state machine to control menu <-> game flow
- Music and SFX piped to dedicated Audio Mixer with custom Snapshots for menus/gameplay
- Pause menu with options to resume, restart or quit
- Custom painted tilemap level
- Fully animated player and enemy characters (with custom anim events to help drive the code)
- Custom player movement including:
- - Custom ground/wall contact checking
- - Boostable Jumps (hold key longer for bigger jumps)
- - Wall grabbing
- Pick-ups
- Enemy patrol, with attack behaviour upon seeing the Player

## Walkthrough:

At first you will find you cannot jump high enough to reach the platform above the spawn point (or over the right hand wall).
In keeping with the metroidvania concept of having to explore the environment to gain abilities in order to unlock more of the level, you must head left until you acquire the Double Jump ability.
After that, you'll be able to reach the central platform, where you'll gain the Wall Grab ability. Finally, after scaling the wall, you can unlock the Attack ability.
Fight the zombie guarding the house and defeat him to complete the level. 

## Demo

[![YouTube Demo](https://imgur.com/9WOjNSn.png)](https://youtu.be/_dyEzEXfSLM)

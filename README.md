# FPS-Weapon-Procedural-Animations
This includes a bunch of scripts to add to your weapon (preferably the parent not the mesh itself) to move it realistically with no animations needed

**GunBob.cs**:

This is a script for moving the gun while moving. as expected, it references a PlayerMovement script to get movement data from the player. The movement script is included in the **Player** folder.

**GunSway.cs**

This is for swaying the gun when looking around. This also references the player movement but only to disable the effect while sprinting as it conflicts with Gunbob.cs when sprinting.

**GunIdleSway**

This is a procedural idle animation.

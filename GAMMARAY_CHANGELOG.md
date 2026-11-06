# GammaRay (3847) Changelog

## v2.0.0

### Intake
- Intake no longer phases through the bumpers when driven into a wall. Once the robot picked up its first game piece (including the preload), the intake stopped colliding with the rest of the robot, so a wall could push it straight into the chassis. It now stops against the bumpers and frame, and the robot stops with it
- Ground coral intake zone trimmed to the width of the cradle (about 12.6 in deep, was 19.4 in). Coral sitting off the edge of the intake is no longer grabbed and then knocked away; line coral up with the cradle to pick it up

### Arm
- Arm no longer wobbles, including side to side, which the real arm can't do. The elevator carriage, shoulder and elbow sections now have realistic weights (4, 3 and 2 kg, up from 1 kg each), so the arm holds steady on its joints instead of swaying against the much heavier chassis
- The wrist stays light (1 kg), so the end of the arm doesn't rock the robot when it swings between setpoints

### Algae
- Algae claw plates and standoffs have collision, so algae and field elements hit the claw instead of passing through it

### LEDs
- Idle (enabled, nothing else going on): purple wave, the pattern that used to show while disabled
- Disabled: scrolling rainbow
- Auto: the countdown is now a rainbow, still turning off one LED at a time over the 15 s
- Intaking coral: white breathe. Intaking algae: aquamarine breathe. Only while the intake is actually running, so holding intake with a piece already in the robot shows idle
- Auto-aligning to the reef: orange breathe while either auto-align button is held
- Ready to score: green breathe once auto-align has the robot within 5 cm of its spot at L1-L4, or with Barge selected and the robot in range of the barge
- Breathing fades to a fainter version of the color instead of going dark

## v1.1.0 (2026-10-11)

### Intake
- Ground coral and algae intake can flip to the other side: press Intake Mode Toggle (D-pad left) while intaking. Scoring flips it back, so the next intake starts on the default side

### Look
- Chains and belts move: chains are real links that run around their sprockets, belts carry printed text that runs with them, and the sprockets, pulleys and encoder gears turn
- Elevator belts and the wrist twist belt, missing from the model, are back; the elevator belts follow the carriage as it moves
- SRPP parts have a black woven finish, like carbon fiber
- LEDs no longer turn solid green when the right branch is picked
- LED patterns run along the strips instead of the whole strip showing one color: the auto countdown turns LEDs off one by one from top to bottom, and the disabled fade flows along the strip

## v1.0.0 (2026-10-08)

### Fixes
- Arm poses tuned for this mod now take effect in the game, not only in the editor: L1, L4 and low/high reef algae were running on older defaults since v1.0.0-beta
- Reef algae elevator heights raised
- Elevator holds still while the robot is disabled (it could keep moving, e.g. dropping to stow after a score)
- At the other alliance's reef (e.g. stealing algae) the arm picks the side facing that reef, and auto-align picks the right branch; both used to be judged against your own reef and came out backwards

### Intake
- Ground coral intake reaches further and pulls harder

### Look
- Model recolored, with a clear polycarbonate belly pan
- Robot Signal Light glows orange: solid while disabled, blinking while enabled

### Climb
- Climber is stronger, so it reaches its climbed position and lifts the robot the same way every climb (it used to stall partway, at a different angle each time)
- Climber swings out at a steady, capped speed, so it no longer clips through the cage
- Reworked climber colliders

### Platforms
- install-mod.sh (macOS and Linux) replaces install-mod-mac.sh, and install-mod.bat installs on Windows

## v3.0.0-beta (2026-10-05)

### Intake
- Intake roller is a physical, motor-driven roller that grips and pulls game pieces, not just a spinning model
- Ground coral intake uses planar tolerancing and no longer pulls coral along its length
- Coral being pulled in collides with the robot instead of passing through it, so it no longer phases through the end effector
- Coral intake pulls slower and a bit softer, with the roller doing more of the work
- Elevator sits higher for low and high reef algae intake

### Arm
- Wrist turns toward the branch the driver picked during the drive up to the reef, not only once auto-align arrives

### Platforms
- install-mod-mac.sh installs the macOS zip into MoSimulator's Mods folder

## v2.0.0-beta (2026-10-03)

### Scoring
- Coral briefly clears the robot after every score, improving scoring consistency on all levels
- Coral intake stays off for 0.35 s after scoring, so a quick switch to intake won't regrab what you just scored
- Ejecting coral from stow pops it up and clear of the robot
- Scoring L1 coral with auto-align is more consistent at making the pyramid

### Intake
- Coral intake no longer grabs coral it shouldn't (slower pull, shorter reach)
- Coral intake pulls a bit harder

### Arm
- Wrist 50% faster

## v1.0.0-beta (2026-10-02)

First release.

### Robot
- Elevator, shoulder/elbow/twist arm and climber, driven by the poses and scoring sequence from Spectrum's 2025 robot code
- Scores L1–L4 off the front or back, whichever side faces the reef
- Wrist turns toward the chosen left/right branch and flips over the poles between them
- Auto-align stops at the center of the reef face
- Shoulder and elbow avoid swinging through the robot
- Arm speeds: shoulder 4, elbow 5, wrist 7
- Lowered center of mass so it doesn't tip on quick direction changes
- Climber holds position while the robot is disabled

### Coral
- Ground intake by default; Robot Special toggles human player station intake
- Station intake comes off whichever side faces the nearest station
- Coral being pulled in ignores the robot so it can't jam against the arm and get flung

### Algae
- Algae intake from the ground, stack and low/high reef
- Separate stow pose while holding algae
- Barge: wrist turns to face the barge, stronger throw
- Spring-loaded algae pincher opens while holding algae

### Look and sound
- Spinning intake roller with sound, algae stall sound, elevator audio
- LED strips ported from Spectrum's LED states
- Inverted logo, bumper numbers in the logo font, alliance-colored bumpers
- Main menu model with LEDs
- Tutorial cards for station intake, two-sided scoring and climb

### Platforms
- Windows, macOS and Linux builds

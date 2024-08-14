If you've read the in-game credits, yes, that's my real name and Im not just Alan. (thats my nickname)
This game was created 5 or 6 weeks ago, but lecturer is too ass at providing useful codes to make anything that
he "required" our game to be "interesting". So ill admit that half of the code is with the help of ChatGPT
(with some internet searching) and me doing the reading and changes as neccessary. Otherwise this game would be
lame as shit and viable for "arbitary assignment requirements".

But still, I only actively working on it for about 5 days straight excluding class time (since those times I was drawing
graphics for the game). Basically working tirelessly morning till night (which is not effective, I know) until I was satisfied
with some more interesting stuff being learned rather than relying what lecture is giving.

And temper your expectations too, since this game is barebones and lacking any replayability. (and probably frustrating for some)


-----------------------------------------------------------------------------------------------------------------------

Version 1.0 Notes:

- Enemy tank patrol fixed, now they wont stuck at each other (hopefully)
- Implement tank engine SFX for this build (Removed previously due to lecture being an ass)
- "Guard-rail" implemented due to horrible gameplay flow (Didn't have time to implement enemy location marker)


Version 1.1 Notes:

- Enemy wont shoot you through undestructable obstacles, they will wait until you're in their line of fire (makes them feel smarter)
- Added pause menu


Version 1.2 Notes:

- Balancing update (its still a difficult game to play, there's no difficulty option)
- Added Ambient sound effects when near the seashore
- When all zones are cleared, the advance arrow will not display now.
- Updated the Briefing Map board

------------------------------------------------------------------------------------------------------------------------

Issues that's not possible to fix for my current knowledge:

- Ricochet and non-penetration are inconsistent
A: This is due to (and probably) the capsule collider 2D, as they create an inconsistent tangent to box collider 2D,
   which causes non-penetration even though you shot them 90 degrees tangent. (Also, there are only ricochet mechanics,
   the non-penetration mechanic is just a bug that became feature)

- Map flow is horrible, having trouble finding & destroying enemy tanks to proceed next area
A: Forgive me for the horrible map design, I have 0 prior time to plan any design when this assignment was announced.
   I was basically being thrown into a busy semester that I have to juggle around until I realised assignment deadline
   was closing in + other subject exam. This was basically being rushed out without extensive testing, so thats why I put
   the "Guard-rail", which reduce the likelyhood of getting lost.



------------------------------------------------------------------------------------------------------------------------

Additional story during the making process (Warning: Ranting):

- Originally, this assignment wasn't meant to spent too much time on graphic design. Lecturer had provided me free Unity Asset
  that looks like mobile game rip-off graphics that I wouldn't want to use.

-* Lecturer is actually very good at programming; but horrible on game design as his prior experience (which can seen in
  LinkedIn) stated that he previously been an Computer Science guy for IT, Assistant, Classera Admin, Tutor (Which "provide"
  fun ways of coding that includes: Minecraft modding/redstone/coding & STEM programs with MC Education) and finally
  "Game Dev & Programming Lecturer" for our college for 1 year & 8 months as of this writing. Yeah having "game dev"
  title at his resume doesn't state that he know shit that how ball-crushing hard is it to code and make entire game by yourself,
  ground up from scratch in Unity. And yet he acts like he knows shit everytime he ask us to make the game "more interesting" 
  without any good examples. Fucker never even made a complete any nice looking game by itself, hell, I don't even consider 
  *modding game (by definition for this context: modifying/adding without overhauling/recreate the entire game) will make you 
  a seasoned/expereinced game dev, jesus fucking christ.

- My classmate friend gave up doing anything crazy, although they did tried to put something different compare to previous
  semester, I still consider it "better than nothing".

- Originally there weren't any ricochet mechanics, its just basic OnTriggerEnter2D to deal damage to enemy.
  But at least curiousity doesn't kill the cat this time. ;)

-* The process of making this game a "WW2-like" theme is because our lecture wanted us to make the game more "interesting".
  And by interesting, he want us to try "make a tank game with story". Like sure, any reference we can get inspiration from?
  Well, lecturer provided us a shitty mobile tank game with the graphics obviously ripped from World of Tanks brainrot cartoons
  and tell us "there's story". When in actuality, its a shitty micro-transaction mobile game with progression + side scroll.
  
  Im so pissed, this is where when I finally snapped and awake from delusion that I though all the time that the lecturer is
  reliable; when in actuality its just an smart ass in disguise. And of course I called him out on his shitty explanation, 
  but he refused to listen multiple times before I finally get through him. Un-fucking-believeable.

  But of course after that day, I tried to recall about games with tanks, like Battlefield, Call of Duty, Halo, World of Tanks,
  War Thunder, etc. Then I came to a conclusion of making a WW2 game with an "Operation"-like objective (instead of story)
  to at least make it feel like the game has something rather than shooty shooty bang bang.

-* ChatGPT + Internet + Me is more helpful than Lecturer. What he provided us is 3 base code; Movement, Enemy Patrol, Audio Manager.
  All of this are barebones and too basic. Although Im no expert and Im using inefficient ways of doing the game with lots of
  overhead, at least Im making a game that isn't relying on barebones shit that I have either 0 knowledge or understandable
  but not enough.

  Plus, just only a Audio Manager and inheritance is used instead. Jeez.

- Cheat code was added due to being curious + wanting to implement since player can drive the M808B tank too 
  (you can find it in the credits, look closer lol)

- The reason I choose War Thunder sound effects are due to its readily-available at The Game Resource website, which hosts
  several ripped audio files from video games. Plus I can't find World of Tanks sound effects anyways, and ripping it manually 
  and checking which-is-which sound is such a pain in the ass to do.

- Although Im already aware that I will cause copyright issues for the college for using copyrighted audio, I don't give a fuck
  as long as I credit properly tbh.
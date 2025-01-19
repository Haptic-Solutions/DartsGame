Unity Game For Testing the Gauntl33t Haptic Glove.

Currently it is setup to talk to a serial port so you need to change the hard coded serial port value in AnimateFingers.cs to the appropriate Serial (COM) port.  
I am personally using a data power splitter and running the glove hard wired. However, you could use a bluetooth serial port in ideal conditions.
You can also change the file to use a UDP port instead.

For tracking and headset I am currently using Quest 2 and the Quest 2 controller because it is easy to change AAs at events and doesn't get ocluded like the Quest 3 controller. I don't see any reason I couldn't use it with Quest 3 and my Quest Pro controllers instead though.  I would eventually like to see the tracking done with the Vive Ultimate Trackers and possibly a custom Pi Based Inside Out Tracker this group will be working on: https://discord.gg/3GNt2qtBmh
Diver X is also working on a cool tracking solution that could be useful for tracking the glove when it is release.
Realistically though any help with converting this to use universal tracking methods would be greatly appreciated.

Changes from the demo at events 
- Had to remove the catana because even though it is free on asset store I am not sure if it is alright to share on Github. Hopefully will make a replacement at some point.
- Also, removed the copyright infringing music that I had on the demo.
- The art on the wall is now replaced with an AI generated picture but you might get the vague reference.  If anyone wants to make a replacement art would be appreciated.

Would love to eventually make this into a full globe trotting archeologist action adventure game which starts in a club sort of like a certain famous movie with an archeologist as the main character.

It looks like I used the XR Interaction Toolkit for actual VR Integration:
https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html

I believe I used the Steam VR Hand for the hand model. So credit to them for the hands:
https://assetstore.unity.com/packages/tools/integration/steamvr-plugin-32647?srsltid=AfmBOorNdvx-Lvl_lEOLFfG9ko-2I0UwpDcsBEtw3jR_FTNUwXZL8BJS

Any help with making this demo / game better would be greatly appreciated!

# MKDD Hex Editor
This is a little Hexing Tool I made for my MKDD ACT TAS. I do most of my Hexing in HxD actually, but in MKDD you have the relatively unique ability to swap characters midrace. If you do that in a 2P Race, the roles of the Players change (Controller 2 becomes the driver and Controller 1 handles the items).
So if you wanted to hex in a Character Swap, you'd have to **swap the controllers** afterwards for every frame to stay in synch, which isn't a thing in normal Hex Editors. Instead, I wrote this thing to do that for me. And because I was already at it, I added some QOL things to it to make hexing more appealing overall.
<div align="center">
  <img width="878" height="292" alt="HexEditorScreenshot" src="https://github.com/user-attachments/assets/ca11ef25-5915-48db-bb72-13621b1fffb9" />
</div>

## What can it do ?
First of all you need to click [Load DTM] to load a TAS file into the Editor. Then you can view the inputs on any given frame by inputting a Number under "Frame Number" or by scrolling. One kind-of-big issue this Editor has is that I have no way of detecting lag-frames within the DTM File, so the targetted frame number will get a bit overshot (the later the frame is, the more lag-frames may have occured beforehand). But you can check if it matches by eye or something like that.

- **[Z-Tech]** - Hexes in Z-Tech at the current Frame (or removes it)
- **[Dupe Frame]** - Overwrite the next Frame with the current one
- **[Insert Frame]** - Insert a Copy of the current Frame right after the current one, pushing back the Rest
- **[Remove Frame]** - Remove the current Frame entirely (no CTRL+Z atm)


If you are done editting the DTM, click [Save DTM] and it should prompt you to enter a savefile name (intentional behaviour is that it uses the same name that you loaded in, except that it appends "_clone").

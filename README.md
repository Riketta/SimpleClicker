# SimpleClicker
A simple clicker that can be used to tag mobs in World of Warcraft.  

## Usage
- Toggle clicking - [Num4].  
- Super mode (no input delay) - [Num5].  
- Human mode (some input delay) - [Num6].  
- Switch to spell key spam (Q) - [Num2].  
- Switch to interaction key spam (Num0) - [Num3].  

By default (on startup) human mode is enabled.  
By default the clicker will spam spell key Q.  

Use the --ignorefocuscheck argument to allow the clicker to send commands to the target window when it is not in focus.  

## Macro
An example of macro I use to find and tag mobs:  
```
/cleartarget
/tar reef crawler
/tar fine
/stopmacro [noexists][dead]
/script SetRaidTarget("target", 6);
/cast Judgement
```

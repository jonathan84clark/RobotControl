# Robot Control
The robot control app is designed to interact with a Raspberry Pi 3-based robot. The robot streams video and provides a web interface for basic commands.  

## Theory of Operation  
The robot control uses HTTP posts to send commands to the robot and HTTP GETs to pull data from the robot. The buttons handle both an on-down and an on-up event because the robot works by needing a stop command after a go command is given. This gives the user full control over the robot.  

## Compilation  
The robot control can be compiled with the Unity game engine.  

## Status  
The robot control is still in active development; it is not intended to be released to the public.  

# Infinity Bike
Last Update: 27 - 05 - 2018

<img src="/Ressources/Images/Logo.png" width="200">
  
Welcome to the Infinity Bike repository. A simple and inexpensive indoors bike training video game with built-in sensors that allows the player to ride on a virtual track.

## Why use Infinity Bike? 

Rainy days and the Winter season can really disrupt a training program. Riders always have the option to mount their bike on a trainer but the monotony of trainers
will always make it a bit difficult to keep up the pace. This is why we decided to add a little bit on interactivity to our training program. The Infinity Bike set-up is designed to be a simple as possible
while still allowing a rider to read the speed and steering from a bike mounted on any trainers and relay the information to the computer. 

## Where are we in development 
In it's current form, the code should compile on a Windows or Mac. We have the communication between Arduino and Unity established and we even have a demo track that you can use to test
your set-up. If you want to take full advantage of this work, you will need to have access to a 3D printer. (Tip : A lot of community libraries are starting to have them available to the public.)
Alternatively, you could design and machine your own parts. If you do let us know!

## Installation
To participate to this project, start by cloning the repository to a drive on your computer. You should get all the assets for the Unity program in the Infinity Bike folder and the Arduino Code in the ArduiBike folder.
Don't have a bike/trainer or access to a 3D printer? You can easily build a small arduino circuit that fits on your desk. Alternatively, the Arduino inputs can be emulated with the keyboard by
setting a flag in the Unity inspector.

If you make a change you think is worth including in the main project, send us a pull request and we will review it! 

Sadly, we don't have a ready out of the box version yet but it's on the to-do list. For now, you will need to clone the repository and build the current scene for your system.
### Hardware
#### Non 3D Printed parts list
#### 3D Printed parts list
#### Circuit 

<img src="/Ressources/Images/Circuit.png" width="500">


## How to use?
We did public a short instructable tutorial that should provide enough information to get started with this project. 
Building a more in-depth tutorial for potential contributors is on the To-do.

## Credits

This project started after we read the tutorial by Allan Zucconi "how to integrate Arduino with Unity" ( https://www.alanzucconi.com/2015/10/07/how-to-integrate-arduino-with-unity/ )

The request from the Arduino are handled using the SerialCommand library ( https://github.com/kroimon/Arduino-SerialCommand )



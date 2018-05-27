# Infinity Bike
Last Update: 27 - 05 - 2018

<img src="/Ressources/Images/Logo.png" width="200">
  
Welcome to the Infinity Bike repository. A simple and inexpensive indoors bike training video game with built-in sensors that allows the player to ride on a virtual track.

## Why use Infinity Bike? 

Rainy days and the Winter season can really disrupt a training program. Riders always have the option to mount their bike on a trainer but the monotony of trainers
will always make it a bit difficult to keep up the pace. This is why we decided to add a little bit on interactivity to our training program. The Infinity Bike set-up is designed to be a simple as possible
while still allowing a rider to read the speed and steering from a bike mounted on any trainers and relay the information to the computer. 

See the Youtube video: 

![Youtube video](https://img.youtube.com/vi/j3q3ih8c10g/0.jpg)](https://www.youtube.com/watch?v=j3q3ih8c10g)

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

1 x <a href="https://store.arduino.cc/usa/arduino-nano">Arduino nano</a> ($22.00)<br/>
1 x <a href="https://www.amazon.com/Elegoo-6PCS-tie-points-Breadboard-Arduino/dp/B01EV6SBXQ/ref=br_lf_m_d2xh2ecztuwj4fa_img?_encoding=UTF8&s=pc">Mini Breadboard</a> ($1.33/unit)<br/>
1 x <a href="https://www.amazon.com/Uxcell-a15011600ux0235-Linear-Rotary-Potentiometer/dp/B01DKCUVMQ/ref=zg_bs_306810011_1?_encoding=UTF8&psc=1&refRID=ZPQ3XYHZQJXNW7W0AH5P">10K Potentiometer </a> ($1.80/unit)<br/>
1 x <a href="Hall%20Effect%20KY-003%20Magnetic%20Sensor%20Module%20DC%205V%20For%20Arduino%20PIC%20AVR%20Smart%20Cars">Halls sensor </a> ($2.79)<br/>

** Estimated prices

#### 3D Printed parts list
#### Circuit 

<img src="/Ressources/Images/Circuit.png" width="500">


## How to use?
We did public a short instructable tutorial that should provide enough information to get started with this project. 
Building a more in-depth tutorial for potential contributors is on the To-do.

## Credits

This project started after we read the tutorial by Allan Zucconi "how to integrate Arduino with Unity" ( https://www.alanzucconi.com/2015/10/07/how-to-integrate-arduino-with-unity/ )

The request from the Arduino are handled using the SerialCommand library ( https://github.com/kroimon/Arduino-SerialCommand )



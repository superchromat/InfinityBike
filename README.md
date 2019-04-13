# Infinity Bike
<img src="/Ressources/Images/Logo.png" width="200">
  
Welcome to the Infinity Bike repository. A simple and inexpensive indoors bike training video game with built-in sensors that allows the player to ride on a virtual track.

Visit our website! www.infinitybike.net

## Why use Infinity Bike? 

Rainy days and the Winter season can really disrupt a training program. Riders always have the option to mount their bike on a trainer but the monotony of trainers
will always make it a bit difficult to keep up the pace. This is why we decided to add a little bit on interactivity to our training program. The Infinity Bike set-up is designed to be a simple as possible
while still allowing a rider to read the speed and steering from a bike mounted on any trainers and relay the information to the computer. 

<img src = "/Ressources/Images/Gameplay_V0/Menu.png" width="900">

<img src = "/Ressources/Images/Gameplay_V0/Biking.png" width="900">

See the Youtube video (for a deprecated version of Infinity Bike): 

[![Link to youtube video](https://img.youtube.com/vi/j3q3ih8c10g/0.jpg)](https://www.youtube.com/watch?v=j3q3ih8c10g)

## Where are we in development 
In its current form, the code should compile on a Windows or Mac. We have the communication between Arduino and Unity established and we even have a demo track that you can use to test
your set-up. If you want to take full advantage of this work, you will need to have access to a 3D printer. (Tip : A lot of community libraries are starting to have them available to the public.)
Alternatively, you could design and machine your own parts. If you do, let us know!

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
1 x <a href="https://www.aliexpress.com/item/100PCS-LOT-1-4W-220-ohm-resistor-1-ROHS1-4w-220R-ohm-Metal-Film-Resistors/32689231347.html?spm=2114.search0104.3.22.6e6347b5Z7ERSI&ws_ab_test=searchweb0_0,searchweb201602_2_10152_10065_10151_10344_10068_10130_5722815_10342_10547_10343_10340_5722915_10548_10341_5722615_10696_10084_10083_10618_10139_10307_5722715_5711215_10059_308_100031_10103_10624_10623_10622_5711315_5722515_10621_10620,searchweb201603_1,ppcSwitch_5&algo_expid=38f3e0b1-dc13-4f86-8131-47538c0df9db-3&algo_pvid=38f3e0b1-dc13-4f86-8131-47538c0df9db&priceBeautifyAB=0">220 Ohm resistor</a> ($1.00/kit)<br/>
1 x <a href="https://www.amazon.com/Uxcell-a15011600ux0235-Linear-Rotary-Potentiometer/dp/B01DKCUVMQ/ref=zg_bs_306810011_1?_encoding=UTF8&psc=1&refRID=ZPQ3XYHZQJXNW7W0AH5P">10K Potentiometer </a> ($1.80/unit)<br/>
1 x <a href="https://www.aliexpress.com/item/KY-003-Standard-Hall-Magnetic-Sensor-Module-Works-with-Arduino-Boards-for-Arduino/32693432353.html?spm=2114.search0104.3.186.599d770dWMU1Zt&ws_ab_test=searchweb0_0,searchweb201602_2_10152_10065_10151_10344_10068_10130_5722815_10342_10547_10343_10340_5722915_10548_10341_5722615_10696_10084_10083_10618_10139_10307_5722715_5711215_10059_308_100031_10103_10624_10623_10622_5711315_5722515_10621_10620,searchweb201603_1,ppcSwitch_5&algo_expid=5d7e9e2e-c6de-4431-8a5b-cecc73fa021c-27&algo_pvid=5d7e9e2e-c6de-4431-8a5b-cecc73fa021c&priceBeautifyAB=0">Hall sensor </a> ($0.96)<br/>
20 cm x <a href="https://www.aliexpress.com/item/5m-lot-GT2-6mm-open-timing-belt-width-6mm-GT2-belt-Rubbr-Fiberglass-cut-to-length/32811832945.html?spm=2114.search0104.3.50.4ca843e4JrNX47&ws_ab_test=searchweb0_0,searchweb201602_2_10152_10065_10151_10344_10068_10130_5722815_10342_10547_10343_10340_5722915_10548_10341_5722615_10696_10084_10083_10618_10139_10307_5722715_5711215_10059_308_100031_10103_10624_10623_10622_5711315_5722515_10621_10620,searchweb201603_1,ppcSwitch_5&algo_expid=1aae0170-0588-45a1-a4ac-2558d64e76cf-7&algo_pvid=1aae0170-0588-45a1-a4ac-2558d64e76cf&priceBeautifyAB=0"> 6 mm 3D printer timing belt </a> ($3.33)<br>
1 kit x <a href="https://www.aliexpress.com/item/Hot-Sale-1-Set-300pcs-M3-304-Stainless-Steel-Hex-Socket-Screws-Bolt-With-Hex-Nuts/32808686142.html?spm=2114.search0104.3.1.703b7491nfv8lT&ws_ab_test=searchweb0_0,searchweb201602_2_10152_10065_10151_10344_10068_10130_5722815_10342_10547_10343_10340_5722915_10548_10341_5722615_10696_10084_10083_10618_10139_10307_5722715_5711215_10059_308_100031_10103_10624_10623_10622_5711315_5722515_10621_10620,searchweb201603_1,ppcSwitch_5&algo_expid=671cbcd2-b8c1-400f-b5f3-cc37f87cfeda-0&algo_pvid=671cbcd2-b8c1-400f-b5f3-cc37f87cfeda&priceBeautifyAB=0">Various Lenght M3 screws and bolts</a> ($6.82)<br/>
1 x <a href= "https://www.aliexpress.com/item/Universal-Magnet-For-Bicycle-Bike-Cycling-Computer-Works-Speedometer-Odometer-Drop-Shipping/32852306956.html?spm=2114.search0104.3.21.32461d8aw5yTQN&ws_ab_test=searchweb0_0,searchweb201602_2_10152_10065_10151_10344_10068_10130_5722815_10342_10547_10343_10340_5722915_10548_10341_5722615_10696_10084_10083_10618_10139_10307_5722715_5711215_10059_308_100031_10103_10624_10623_10622_5711315_5722515_10621_10620,searchweb201603_1,ppcSwitch_5&algo_expid=20a02fd9-cbb1-4fab-92b5-93c50623b4ab-3&algo_pvid=20a02fd9-cbb1-4fab-92b5-93c50623b4ab&priceBeautifyAB=0"> Bicycle speedometer magnet </a> ($0.98) <br/>

** Estimated prices

#### 3D Printed parts list
All the 3D print .stl files are available on [Thingiverse](https://www.thingiverse.com/thing:2933374). The picture below list them all. 
We mounted the material above with 3D printed parts. 
The files we used are listed bellow and they're numbered with the same convention as the image above. 
You can use them as-is but make sure that the dimensions we used match your bike.
<img src="/Ressources/Images/3DprintedParts/PartsLayout.png" width="900">

##### Parts filename
1. FrameConnection_PotentiometerHolder_U_Holder.stl
2. FrameConnection_Spacer.stl
3. BreadboardFrameHolder.stl
4. Pulley_PotentiometerSide.stl
5. Pot_PulleyConnection.stl
6. FrameConnection.stl
7. Pulley_HandleBarSide_Print2.stl
8. FrameToHallSensorConnector.stl
9. PotHolder.stl
10. HallSensorAttach.stl
##### Assembled parts pictures
<img src="/Ressources/Images/InstalledParts.png" width="900">

Please note that we are currently working on a better stearing measurement device. As seen in the picture above, it is based on a rotation platform placed under the bike wheel. 

<img src="/Ressources/3DprintedPartsSTLV0_0/RotationPlatform/ Screenshot.png" width = "300">

More info on this device is available [here](https://github.com/AlexandreDoucet/InfinityBike/tree/master/Ressources/3DprintedParts%20STL%20V0.1/RotationPlatform).


#### Circuit
The following circuit can used for prototyping. 

<img src="/Ressources/Images/Circuit.png" width="500">

We are currently developping a permanent PCB design to get a more robust platform. The Eagle files of this PCB can be found in the [AutodeskEagleProject folder](https://github.com/AlexandreDoucet/InfinityBike/tree/master/Ressources/Electronics/AutodeskEagleProject/InfinityBike/Readme.md).

<img src = "/Ressources/Electronics/AutodeskEagleProject/Infinity Bike/PCB.jpeg" width = 250 px> <img src = "/Ressources/Electronics/AutodeskEagleProject/Infinity Bike/PBC_TurnTable.jpeg" width = 250 px> <img src = "/Ressources/Electronics/AutodeskEagleProject/Infinity Bike/MagnetAndHall.jpeg" width = 250 px> 

## How to use?
We published a short 
<a href="https://www.instructables.com/id/Infinity-Bike-Indoors-Bike-Training-Video-Game/">instructable</a> 
tutorial that should provide enough information to get started with this project. 

Please note that this tutorial is no longer up to date with the project and will need to be revisited. Building a more in-depth tutorial for potential contributors is on our To-do list.

** Because we used blender to generate the 3d models, you must have blender installed before opening the project. Otherwise some assets wont load properly.**

## How to contribute? 
From now on, we use the [git branching model proposed by Vincent Driessen](https://nvie.com/posts/a-successful-git-branching-model/). 

To add features to the project, please branch out of the develop branch. The testing of pull request will be done with the 018.3.12f1 version of Unity. 


### Project structure 

### Medium term goall 



## Credits
### Project contributors
Alexandre Doucet (_Doucet_)</br>
Maxime Boudreau (MxBoud)
Alexis Nosovich 

### External ressources
[The Unity game engine](https://unity3d.com)

This project started after we read the tutorial by Allan Zucconi "how to integrate Arduino with Unity" ( https://www.alanzucconi.com/2015/10/07/how-to-integrate-arduino-with-unity/ )

The request from the Arduino are handled using the SerialCommand library ( https://github.com/kroimon/Arduino-SerialCommand )



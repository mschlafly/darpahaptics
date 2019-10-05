# darpahaptics

This code is for generating the haptic interface and communication information to and from the interface for the human swarm interaction darpa challange. It consists of four files that are all run during the experiment. 
1. Haptic_display - This c# program generates haptic sprites representing objects in the unity environment (primarily buildings) on the TanvasTouch monitor display. The position and rotation of the objects change according to the person's position in unity environment provided by text file "person_position.txt". It is based on the HelloTanvas demo provided by Tanvas.
2. TouchDetection_UWPcs - This program utilizes the Universal Windows Platform touch detection SDK for app development to record the locations of user inputs. Input recording starts and ends with a double tap. The double tap to end recording also saves the data to text file "touch_locations.txt" in the Downloads folder on the computer. This program would contain any image to be displayed in the .xaml file.
3. TCP_communcation - Server side of a TCP socket using the winsock library. This code (a) creates a TCP socket for communicating from a windows OS and linux OS, (b) connects to client on linux OS and tests the socket, (c) continuously sends over coordinates for touch input (if new input) and recieves coordinates for the person's position and orientation in unity, writing them to a text file if the position has changed. The coodinates are send over in batched to avoid any potential loss of data.
4. user_input - ROS package that is run on the linux OS for communicating over the TCP socket, publishing touch input locations, and subscribing to a message containing the person's position. It contains two custom messages---------------- -, publishes/subscribed to two topics------------ -, and contains one rosnode client.cpp. After sourcing, using catkin_make and roscore, the node can be run with "rosrun user_input client" or from a launch file. The most up-to-date version of this code be found on Ahalya's github.

Changes in this code
-recieve rotation from -pi to pi
-send a ? in first character whenever invalid

## Supporting scripts for generating haptic bitmaps
Haptic objects are created based on a bitmap provided in a png file where a black pixel is max friction, a white pixel is no friction, and grey pixels are somewhere in between. Currently, there are five haptic objects for this environment: one representing the person (person.py), and four types of buildings (bld_small, bld_large, bld_2vert, bld_2horiz). The buildings use the python script make_band.py. To show the bitmap corresponding the to the sprites  (buildings and person) being at specific pixel positions, use example.py. The sprite locations can be found by running the haptic_display code and clicking on the sprites in the TanvasTouch engine. 

The images are saved in Haptic_display/HelloTanvas/Assets. If you want to add more objects or change the name, the new .png file must be included in the visual studio project.

## System requirements and set up
It must be run from a Windows OS using Visual Studio. 
networking notes


## Important parameters


## How to run the code

## Examples for training

## Editing haptic objects and example images

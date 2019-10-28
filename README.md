# darpahaptics

This code is for generating the haptic interface and communication information to and from the interface for the human swarm interaction darpa challange. It consists of four files that are all run during the experiment. 
1. Haptic_display - This c# program generates haptic sprites representing objects in the unity environment (primarily buildings) on the TanvasTouch monitor display. The position and rotation of the objects change according to the person's position in unity environment provided by text file "person_position.txt". It is based on the HelloTanvas demo provided by Tanvas.
2. TouchDetection - This program utilizes the Universal Windows Platform touch detection C# SDK for app development to record the locations of user inputs. Input recording starts and ends with a double tap. The double tap to end recording also saves the data to text file "touch_locations.txt" in the Downloads folder on the computer. This program would contain any image to be displayed in the .xaml file.
3. TCP_socket - Server side of a TCP socket using the winsock library. This code (a) creates a TCP socket for communicating from a windows OS and linux OS, (b) connects to client on linux OS and tests the socket, (c) continuously sends over coordinates for touch input (if new input) and recieves coordinates for the person's position and orientation in unity, writing them to a text file if the position has changed. The coodinates are send over in batched to avoid any potential loss of data.
4. user_input - ROS package that is run on the linux OS for communicating over the TCP socket, publishing touch input locations, and subscribing to a message containing the person's position. It contains two custom messages, input_array.msg and person_position.msg, publishes/subscribed to two topics, "input" and "person_location", and contains one rosnode client.cpp. After sourcing, using catkin_make and roscore, the node can be run with "rosrun user_input client" or from a launch file. The most up-to-date version of this code be found on Ahalya's github.

## Supporting scripts for generating haptic bitmaps
Haptic objects are created based on a bitmap provided in a png file where a black pixel is max friction, a white pixel is no friction, and grey pixels are somewhere in between. Currently, there are five haptic objects for this environment: one representing the person (person.py), and four types of buildings (bld_small, bld_large, bld_2vert, bld_2horiz). The buildings use the python script make_band.py. To show the bitmap corresponding the to the sprites  (buildings and person) being at specific pixel positions, use example.py. The sprite locations can be found by running the haptic_display code and clicking on the sprites in the TanvasTouch engine. 

The images are saved in Haptic_display/HelloTanvas/Assets. If you want to add more objects or change the name, the new .png file must be included in the visual studio project.

## System requirements and how to set up and run code
Programs 1-3 must be run from a Windows OS using Visual Studio (VS). It may be necessary to download additional VS packages to use Universal Windows Platform. 

For networking, both the TCP server and client must be connected to the same router and be connected to the same IP address and port defined as a variable in the top of both files. Northwestern wifi does not work; it is best to use a private network in which you have access to the network settings. If you experience problems, you may need to connect to a static IP address and/or disable the security features on the computer. user_input was created on ROS melodic. 

To set up the haptic tablet, follow the tanvas instructions. Note that the haptic tablet screen must be set to be the main monitor, however, you should only need to set it once. 

0. Open the tanvas engine application and calibrate monitor by pressing "Calibrate". 
1. Edit file path variables at the top of programs 1-3. 
2. Connect to static IP, disable security (on Dell laptop using Mcafee application), check IP address and port at the top of the TCP programs. 
3. Open the solution file (ending in .sln) for programs 1-3 on the laptop and run in no particular order by clicking the play button in VS.
4. Run the client side of the TCP socket (program 4) within ROS.

## Important parameters
h_unity - This variable controls the amount to which the montior display zooms in on the environment. The number refers to the number of units in the Unity environment will fit vertically on the monitor. We are using 10. This number is used to determine zoom_ratio which gives the number of pixels on the monitor that is equal to a unit length in unity. This variable needs to be changed in Haptic display, TouchDetection_UWPcs, and ALL of the python scripts for generating bitmaps which will all have to be rerun to incorporate this change.

IPv4/PORT - These determine the IP address and port that the TCP socket connects to. These must be the same in the server and client.

docpath_touch/docpath_person - Both are defined in the beginning of the TCP server (program 4), docpath_person in Haptic_display, and docpath_touch in TouchDetection_UWPcs.

x_person_unity, y_person_unity, and th_person - set these variables at the beginning of MainWindow() in Haptic_display/MainWindow.xaml.cs to be the desired starting position and angle. 
 
## Examples for training
I provided two examples visualizing the haptic features for training participants. Use a breakpoint in Haptic_display/MainWindow.xaml.cs to prevent the haptics from automatically updating. Set the starting position of the person (x_person_unity, y_person_unity, and th_person) to be (23,9,0) for example 1 and (7,13,0) for example 2 and h_unity to be 10. The image is displayed by uncommenting a line in Touchdetection_UWPcs/Touchdetection_UWPcs/MainWindow.xaml and editing the source to be either "Assets/example1.png" or "Assets/example2.png".

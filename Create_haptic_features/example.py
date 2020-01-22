from PIL import Image
import math
from bld_small import draw_bld_small
from bld_large import draw_bld_large
from bld_2horiz import draw_bld_2horiz
from bld_2vert import draw_bld_2vert
from person import draw_person
import os.path as path

# example number 1 for the person at (23,11,0) 10 building zoom
# example number 2 for the person at (7,14,0) 10 building zoom
examplenum = 1;

# How to run from command on windows
# go to folder and type python example1.py

# Distance in the unity environment to show on tablet (for height)
H_unity = 10;

# Size of haptic display
H_tablet = 780; #738;
W_tablet = 1280; #1024;

# Ratio for zooming
zoom_ratio = H_tablet / H_unity;


# Size of the person in unity
H_person_unity = 2;
# Size of the person in tanvas
H_person_tablet = int(round(H_person_unity * zoom_ratio));
W_person_tablet = H_person_tablet;
# PERSON
#unity_one = int(round(1 * zoom_ratio))
person_x = 562; #-unity_one;
person_y = 312; #-unity_one;
im = Image.new('RGB',(W_tablet,H_tablet),(255,255,255));
name =  path.abspath(path.join(__file__ ,"../..")); # move up two files in directory

# Draw person
for x in range(-100,W_tablet,1):
    for y in range(-100,H_tablet,1):
        if (person_x == x) and (person_y == y):
            #print("here \n")
            draw_person(person_x,person_y,W_person_tablet,W_tablet,H_tablet,im);

# Draw small building

if (examplenum == 1):
    building_x=(718,1030,-62,718,1030,-62);
    building_y=(-78,-78,-78,702,702,702);
elif (examplenum == 2):
    building_x=(94,406,1186);
    building_y=(234,234,234);
num_buildings = len(building_x);
H_building_unity = 2;
H_building_tablet = int(round(H_building_unity * zoom_ratio));
W_building_tablet = H_building_tablet;
#found = 0;
for x in range(-100,W_tablet,1):
    for y in range(-100,H_tablet,1):
        #print(found,x,y);
        for i in range(num_buildings):
            #print(num_buildings,i,building_x[i],building_y[i]);
            if (building_x[i] == x) and (building_y[i] == y):
                #found = found + 1;
                xmax = x + W_building_tablet;
                if (xmax>W_tablet):
                    xmax = W_tablet-1;
                ymax = y + H_building_tablet;
                if (ymax>H_tablet):
                    ymax = H_tablet-1;
                #print(xmax,ymax);
                draw_bld_small(x,y,W_building_tablet,H_building_tablet,xmax,ymax,im)

# Draw large building
if (examplenum == 1):
    building_x=(250,10000);
    building_y=(234,10000);
elif (examplenum == 2):
    building_x=(718,718);
    building_y=(546,-234);
num_buildings = len(building_x);
H_building_unity = 4;
H_building_tablet = int(round(H_building_unity * zoom_ratio));
W_building_tablet = H_building_tablet;
for x in range(-100,W_tablet,1):
    for y in range(-300,H_tablet,1):
        for i in range(num_buildings):
            if (building_x[i] == x) and (building_y[i] == y):
                xmax = x + W_building_tablet;
                if (xmax>W_tablet):
                    xmax = W_tablet-1;
                ymax = y + H_building_tablet;
                if (ymax>H_tablet):
                    ymax = H_tablet-1;
                draw_bld_large(x,y,W_building_tablet,H_building_tablet,xmax,ymax,im)

# Draw horizontal building
if (examplenum == 1):
    building_x=(250,250);
    building_y=(-78,702);
elif (examplenum == 2):
    building_x=(718,10000);
    building_y=(234,10000);
num_buildings = len(building_x);
H_building_unity = 2;
H_building_tablet = int(round(H_building_unity * zoom_ratio));
W_building_tablet = H_building_tablet * 2;
for x in range(-100,W_tablet,1):
    for y in range(-100,H_tablet,1):
        for i in range(num_buildings):
            if (building_x[i] == x) and (building_y[i] == y):
                xmax = x + W_building_tablet;
                if (xmax>W_tablet):
                    xmax = W_tablet-1;
                ymax = y + H_building_tablet;
                if (ymax>H_tablet):
                    ymax = H_tablet-1;
                draw_bld_2horiz(x,y,W_building_tablet,H_building_tablet,xmax,ymax,im)

# Draw vertical building
if (examplenum == 1):
    building_x=(718,1030,-62);
    building_y=(234,234,234);
elif (examplenum == 2):
    building_x=(94,406,1186,1186,406,94);
    building_y=(546,546,546,-234,-234,-234);
num_buildings = len(building_x);
W_building_unity = 2;
W_building_tablet = int(round(W_building_unity * zoom_ratio));
H_building_tablet = W_building_tablet * 2;
for x in range(-100,W_tablet,1):
    for y in range(-300,H_tablet,1):
        for i in range(num_buildings):
            if (building_x[i] == x) and (building_y[i] == y):
                xmax = x + W_building_tablet;
                if (xmax>W_tablet):
                    xmax = W_tablet-1;
                ymax = y + H_building_tablet;
                if (ymax>H_tablet):
                    ymax = H_tablet-1;
                draw_bld_2vert(x,y,W_building_tablet,H_building_tablet,xmax,ymax,im)

if (examplenum == 1):
    im.save(name+'/TouchDetection/ergodicinput/ergodicinput/Assets/example1.png');
    im.save(name+'/TouchDetection/waypointinput/waypointinput/Assets/example1.png');
elif (examplenum == 2):
    im.save(name+'/TouchDetection/ergodicinput/ergodicinput/Assets/example2.png');
    im.save(name+'/TouchDetection/waypointinput/waypointinput/Assets/example2.png');

im.show()

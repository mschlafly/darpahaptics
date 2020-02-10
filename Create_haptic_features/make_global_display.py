from PIL import Image
import math
from draw_bands import draw_bands_vertical,draw_bands_horizontal
import os.path as path
from person import draw_person

# How to run from command on windows
# go to folder and type python example1.py

# Size of haptic display
H_tablet = 780 #738;
W_tablet = 1280 #1024;
H_tablet = H_tablet + 250 + 250

im = Image.new('RGB',(W_tablet,H_tablet),(255,255,255));
name =  path.abspath(path.join(__file__ ,"../..")); # move up two files in directory

# bottom panel
x = 249
y = H_tablet - 250
xmax = 1030
ymax = H_tablet
W_building_tablet = 1030-250
H_building_tablet = 250
# black width is double both dark grey and grey, white width is double black
# H_building_tablet  = black_width*(4*(#bands-1) + 1)
numbands = 13;
black_width_float = W_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = W_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
draw_bands_vertical(x,y,xmax,ymax,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

numbands = 6;
black_width_float = H_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = H_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
#print(black_width,darkgrey_width,grey_width,white_width)
draw_bands_horizontal(x,y,xmax,ymax,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

# top panel
x = 249
y = 0
xmax = 1030
ymax = 250
W_building_tablet = 1030-250
H_building_tablet = 250
# black width is double both dark grey and grey, white width is double black
# H_building_tablet  = black_width*(4*(#bands-1) + 1)
numbands = 13;
black_width_float = W_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = W_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
draw_bands_vertical(x,y,xmax,ymax,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

numbands = 6;
black_width_float = H_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = H_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
#print(black_width,darkgrey_width,grey_width,white_width)
draw_bands_horizontal(x,y,xmax,ymax,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

# left panel
x = 0
y = 0
xmax = 249
ymax = 1280
W_building_tablet = 249
H_building_tablet = H_tablet
# black width is double both dark grey and grey, white width is double black
# H_building_tablet  = black_width*(4*(#bands-1) + 1)
numbands = 6;
black_width_float = W_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = W_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
draw_bands_vertical(x,y,xmax,ymax,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

numbands = 20;
black_width_float = H_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = H_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
#print(black_width,darkgrey_width,grey_width,white_width)
draw_bands_horizontal(x,y,xmax,ymax,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)


# right panel
x = 1030
y = 0
xmax = W_tablet
ymax = 1280
W_building_tablet = 249
H_building_tablet = H_tablet
# black width is double both dark grey and grey, white width is double black
# H_building_tablet  = black_width*(4*(#bands-1) + 1)
numbands = 6;
black_width_float = W_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = W_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
draw_bands_vertical(x,y,xmax,ymax,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

numbands = 20;
black_width_float = H_building_tablet/(5*(numbands-1) + 1);
black_width = int(round(black_width_float));
darkgrey_width = int(round(.5 * black_width_float));
grey_width = int(round(.5 * black_width_float));
white_width = int(round(3 * black_width_float));
band_width = black_width+darkgrey_width+grey_width+white_width;
diff = H_building_tablet - (band_width*(numbands-1)+black_width);
white_width = white_width + int(round(diff/(numbands-1)));
#print(black_width,darkgrey_width,grey_width,white_width)
draw_bands_horizontal(x,y,xmax,ymax,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

# draw_bld_small(x,y,W_building_tablet,H_building_tablet,xmax,ymax,im)
# for x in range(0,249,1):
#     for y in range(0,H_tablet,1):
#         im.putpixel((x,y),(0,0,0))
# for x in range(1030,1280,1):
#     for y in range(0,H_tablet,1):
#         im.putpixel((x,y),(0,0,0))

im.save(name+'/HapticDisplay/HelloTanvas/Assets/global.png')

# Distance in the unity environment to show on tablet (for height)
H_unity = 10;
zoom_ratio = H_tablet / H_unity;
# Size of the person in unity
H_person_unity = 2;
# Size of the person in tanvas
H_person_tablet = int(round(H_person_unity * zoom_ratio));
W_person_tablet = H_person_tablet;
person_x = 769; #-unity_one;
person_y = 372+250; #-unity_one;
# Draw person
for x in range(-100,W_tablet,1):
    for y in range(-100,H_tablet,1):
        if (person_x == x) and (person_y == y):
            #print("here \n")
            draw_person(person_x,person_y,W_person_tablet,W_tablet,H_tablet,im);
im.save(name+'/TouchDetection/training/training/Assets/global.png')
im.save(name+'/TouchDetection/ergodicinput/ergodicinput/Assets/global.png')
im.save(name+'/TouchDetection/waypointinput/waypointinput/Assets/global.png')

im.show()

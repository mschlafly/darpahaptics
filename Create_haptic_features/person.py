from PIL import Image
import math
import os.path as path

# Creates haptic bitmap representing the person

def draw_person(x,y,W_person_tablet,x_max,y_max,im):
    midpoint_x = x+W_person_tablet/2;
    midpoint_y = y+W_person_tablet/2;
    radius_max = .5 * W_person_tablet;
    for x_i in range(x,x+W_person_tablet):
        for y_i in range(y,y+W_person_tablet):
            distance = math.sqrt((x_i-midpoint_x)**2+(y_i-midpoint_y)**2);
            putpoint = round(distance/1.5);
            if (distance<=radius_max):
                if (putpoint % 2) == 0:
                    if ((x_i<=x_max) and (y_i<=y_max) and (x_i>=0) and (y_i>=0)):
                        im.putpixel((x_i,y_i),(0,0,0));

# Distance in the unity environment to show on tablet (for height)
H_unity = 10;

# Size of the person in unity
H_person_unity = 2;

# Size of haptic display
H_tablet = 780; #738;
W_tablet = 1280; #1024;

# Ratio for zooming
zoom_ratio = H_tablet / H_unity;

# Size of the person in tanvas
H_person_tablet = int(round(H_person_unity * zoom_ratio));
W_person_tablet = H_person_tablet;

im = Image.new('RGB',(W_person_tablet,H_person_tablet),(255,255,255));
name =  path.abspath(path.join(__file__ ,"../..")); # move up two files in directory
x=0;
y=0;
draw_person(x,y,W_person_tablet,W_person_tablet,W_person_tablet,im)

im.show()
im.save(name+'/HapticDisplay/HelloTanvas/Assets/person.png');

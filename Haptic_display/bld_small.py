from PIL import Image
import math
from make_band import make_band


def draw_bld_small(x,y,W_building_tablet, H_building_tablet,x_max,y_max,im):

    band1_black = round(.08 * H_building_tablet);
    band1_darkgrey = round(.025 * H_building_tablet);
    band1_grey = round(.025 * H_building_tablet);
    band1_white = round(.08 * H_building_tablet);

    band2_black = round(.07 * H_building_tablet);
    band2_darkgrey = round(.02 * H_building_tablet);
    band2_grey = round(.02 * H_building_tablet);
    band2_white = round(.07 * H_building_tablet);

    band3_black = round(.045 * H_building_tablet);
    band3_darkgrey = round(.01 * H_building_tablet);
    band3_grey = round(.01 * H_building_tablet);
    band3_white = round(.045 * H_building_tablet);

    # Outer box 1
    make_band(x,y,W_building_tablet,H_building_tablet,band1_black,(0,0,0),x_max,y_max,im);
    x_inner = x+band1_black;
    y_inner = y+band1_black;
    section_width = W_building_tablet - (2*band1_black);
    make_band(x_inner,y_inner,section_width,section_width,band1_darkgrey,(105,105,105),x_max,y_max,im);
    x_inner = x_inner+band1_darkgrey;
    y_inner = y_inner+band1_darkgrey;
    section_width = section_width - (2*band1_darkgrey);
    make_band(x_inner,y_inner,section_width,section_width,band1_grey,(169,169,169),x_max,y_max,im);

    # Box 2
    x_inner = x_inner+band1_grey+band1_white;
    y_inner = y_inner+band1_grey+band1_white;
    section_width = section_width - (2*band1_grey) - (2*band1_white);
    make_band(x_inner,y_inner,section_width,section_width,band2_black,(0,0,0),x_max,y_max,im);
    x_inner = x_inner+band2_black;
    y_inner = y_inner+band2_black;
    section_width = section_width - (2*band2_black);
    make_band(x_inner,y_inner,section_width,section_width,band2_darkgrey,(105,105,105),x_max,y_max,im);
    x_inner = x_inner+band2_darkgrey;
    y_inner = y_inner+band2_darkgrey;
    section_width = section_width - (2*band2_darkgrey);
    make_band(x_inner,y_inner,section_width,section_width,band2_grey,(169,169,169),x_max,y_max,im);

    # Box 3
    x_inner = x_inner+band2_grey+band2_white;
    y_inner = y_inner+band2_grey+band2_white;
    section_width = section_width - (2*band2_grey) - (2*band2_white);
    make_band(x_inner,y_inner,section_width,section_width,band3_black,(0,0,0),x_max,y_max,im);
    x_inner = x_inner+band3_black;
    y_inner = y_inner+band3_black;
    section_width = section_width - (2*band3_black);
    make_band(x_inner,y_inner,section_width,section_width,band3_darkgrey,(105,105,105),x_max,y_max,im);
    x_inner = x_inner+band3_darkgrey;
    y_inner = y_inner+band3_darkgrey;
    section_width = section_width - (2*band3_darkgrey);
    make_band(x_inner,y_inner,section_width,section_width,band3_grey,(169,169,169),x_max,y_max,im);

# Distance in the unity environment to show on tablet (for height)
H_unity = 10;

# Size of haptic display
W_tablet = 1280; #1024;
H_tablet = 780; #738;

# Ratio for zooming
zoom_ratio = H_tablet / H_unity;

# Size of the building in unity
H_building_unity = 2;

# Size of the building in tanvas
H_building_tablet = round(H_building_unity * zoom_ratio);
W_building_tablet = H_building_tablet;

im = Image.new('RGB',(H_building_tablet,H_building_tablet),(255,255,255));
name = 'HelloTanvas/Assets/bld_small';

x=0;
y=0;
draw_bld_small(x,y,W_building_tablet,H_building_tablet,H_building_tablet,H_building_tablet,im)

im.show()
im.save(name+'.png');

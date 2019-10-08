from PIL import Image
import math
from draw_bands import draw_bands_vertical,draw_bands_horizontal
import os.path as path


def draw_bld_2horiz(x,y,W_building_tablet, H_building_tablet,xmax,ymax,im):
    # black width is double both dark grey and grey, white width is double black
    # H_building_tablet  = black_width*(4*(#bands-1) + 1)
    numbands = (5-1)*2+1;
    black_width_float = W_building_tablet/(5*(numbands-1) + 1);
    black_width = int(round(black_width_float));
    darkgrey_width = int(round(.5 * black_width_float));
    grey_width = int(round(.5 * black_width_float));
    white_width = int(round(3 * black_width_float));
    band_width = black_width+darkgrey_width+grey_width+white_width;
    diff = W_building_tablet - (band_width*(numbands-1)+black_width);
    white_width = white_width + int(round(diff/(numbands-1)));
    # print(black_width,darkgrey_width,grey_width,white_width);
    # print(band_width,band_width*(numbands-1)+black_width,W_building_tablet,diff,white_width);
    draw_bands_vertical(x,y,xmax,ymax,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)

    numbands = 7;
    black_width_float = H_building_tablet/(5*(numbands-1) + 1);
    black_width = int(round(black_width_float));
    darkgrey_width = int(round(.5 * black_width_float));
    grey_width = int(round(.5 * black_width_float));
    white_width = int(round(3 * black_width_float));
    band_width = black_width+darkgrey_width+grey_width+white_width;
    diff = H_building_tablet - (band_width*(numbands-1)+black_width);
    white_width = white_width + int(round(diff/(numbands-1)));
    # print(black_width,darkgrey_width,grey_width,white_width);
    # print(band_width,band_width*(numbands-1)+black_width,W_building_tablet,diff,white_width);
    draw_bands_horizontal(x,y,xmax,ymax,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)


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
H_building_tablet = int(round(H_building_unity * zoom_ratio));
W_building_tablet = H_building_tablet * 2;

im = Image.new('RGB',(W_building_tablet,H_building_tablet),(255,255,255));
name =  path.abspath(path.join(__file__ ,"../..")); # move up two files in directory

#print(W_building_tablet,H_building_tablet);

x=0;
y=0;
draw_bld_2horiz(x,y,W_building_tablet,H_building_tablet,W_building_tablet,H_building_tablet,im)

im.show()
im.save(name+'/Haptic_display/HelloTanvas/Assets/bld_2horiz.png');

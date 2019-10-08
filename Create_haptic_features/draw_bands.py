from PIL import Image

def draw_bands_vertical(x,y,x_max,y_max,height,black_width,darkgrey_width,grey_width,white_width,im):
    imax = 20;
    x_new = x;
    black_color = (0,0,0);
    darkgrey_color = (105,105,105);
    grey_color = (169,169,169);
    for i in range(0,imax):
        x_start = x_new;
        for y_i in range(int(y),int(y+height)):
            x_new = x_start;
            for x_i in range(x_new,x_new+black_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),black_color);
            x_new = x_new + black_width;
            for x_i in range(x_new,x_new+darkgrey_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),darkgrey_color);
            x_new = x_new + darkgrey_width;
            for x_i in range(x_new,x_new+grey_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),grey_color);
            x_new = x_new + grey_width + white_width;
def draw_bands_horizontal(x,y,x_max,y_max,width,black_width,darkgrey_width,grey_width,white_width,im):
    imax =  30;
    y_new = y;
    black_color = (0,0,0);
    darkgrey_color = (105,105,105);
    grey_color = (169,169,169);
    for i in range(0,imax):
        y_start = y_new;
        for x_i in range(int(x),int(x+width)):
            y_new = y_start;
            for y_i in range(y_new,y_new+black_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),black_color);
            y_new = y_new + black_width;
            for y_i in range(y_new,y_new+darkgrey_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),darkgrey_color);
            y_new = y_new + darkgrey_width;
            for y_i in range(y_new,y_new+grey_width):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    im.putpixel((x_i,y_i),grey_color);
            y_new = y_new + grey_width + white_width;

# # Distance in the unity environment to show on tablet (for height)
# H_unity = 10;
#
# # Size of haptic display
# W_tablet = 1280; #1024;
# H_tablet = 780; #738;
#
# # Ratio for zooming
# zoom_ratio = H_tablet / H_unity;
#
# # Size of the building in unity
# H_building_unity = 2;
#
# # Size of the building in tanvas
# H_building_tablet = int(round(H_building_unity * zoom_ratio));
# W_building_tablet = H_building_tablet;
#
# im = Image.new('RGB',(H_building_tablet,H_building_tablet),(255,255,255));
# name = 'HelloTanvas/Assets/bld_small';
#
# x=0;
# y=0;
# # black width is double both dark grey and grey, white width is double black
# # H_building_tablet  = black_width*(4*(#bands-1) + 1)
# numbands = 4;
# black_width_float = H_building_tablet/(4*(numbands-1) + 1);
# black_width = int(round(black_width_float));
# darkgrey_width = int(round(.5 * black_width_float));
# grey_width = int(round(.5 * black_width_float));
# white_width = int(round(2 * black_width_float));
# draw_bands_vertical(x,y,W_building_tablet-1,H_building_tablet-1,H_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)
#
# numbands = 6;
# black_width_float = H_building_tablet/(4*(numbands-1) + 1);
# black_width = int(round(black_width_float));
# darkgrey_width = int(round(.5 * black_width_float));
# grey_width = int(round(.5 * black_width_float));
# white_width = int(round(2 * black_width_float));
# print(black_width,darkgrey_width,grey_width,white_width)
# draw_bands_horizontal(x,y,W_building_tablet-1,H_building_tablet-1,W_building_tablet,black_width,darkgrey_width,grey_width,white_width,im)
#
# im.show()

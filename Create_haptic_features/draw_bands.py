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

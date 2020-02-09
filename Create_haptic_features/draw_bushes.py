from PIL import Image
import random

def draw_bushes(x,y,x_max,y_max,W_building_tablet, H_building_tablet,im):
        colors = [(0,0,0),(169,169,169),(105,105,105),(200,200,200),(185,185,185)]
        for x_i in range (x,x+W_building_tablet):
            for y_i in range(y,y+H_building_tablet):
                if ((x_i<x_max) and (y_i<y_max) and (x_i>=0) and (y_i>=0)):
                    colorselect = random.randint(0,60)
                    if colorselect<5:
                        im.putpixel((x_i,y_i),colors[colorselect]);

from PIL import Image

def make_band(x,y,section_width,section_height,band_size,color,x_max,y_max,im):
    # Top band
    # 312 156
    for y_i in range(y,y+band_size):
        for x_i in range(x,x+section_width):
            if ((x_i<=x_max) and (y_i<=y_max) and (x_i>=0) and (y_i>=0)):
                #print(x_i,y_i);
                im.putpixel((x_i,y_i),color);
    # Bottom band
    for y_i in range(y+(section_height-1),y+(section_height-1)-band_size,-1):
        for x_i in range(x,x+section_width):
            if ((x_i<=x_max) and (y_i<=y_max) and (x_i>=0) and (y_i>=0)):
                #print(x_i,y_i);
                im.putpixel((x_i,y_i),color);
    # Left band
    for x_i in range(x,x+band_size):
        for y_i in range(y,y+section_height):
            if ((x_i<=x_max) and (y_i<=y_max) and (x_i>=0) and (y_i>=0)):
                #print(x_i,y_i);
                im.putpixel((x_i,y_i),color);
    # Right band
    for x_i in range(x+(section_width-1),x+(section_width-1)-band_size,-1):
        for y_i in range(y,y+section_height):
            if ((x_i<=x_max) and (y_i<=y_max) and (x_i>=0) and (y_i>=0)):
                #print(x_i,y_i);
                im.putpixel((x_i,y_i),color);

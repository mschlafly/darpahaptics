using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;

using TanvasTouch.Resources;
using TanvasTouch.Service;

namespace HelloTanvas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string docPath_person = "C:/Users/numur/Desktop/darpa/darpahaptics/HapticDisplay"; // Path text files with the person's positon
      
        // Starting position of person for initialization
        float x_person_unity;
        float y_person_unity;
        float th_person;

        // Distance in the unity environment to show on tablet (for height)
        int H_unity_local = 10;
        int H_unity_global = 30;

        // Size of haptic display
        int W_tablet = 1280; //738;
        int H_tablet = 780; //1024;

        // Position on the tablet for the person to be placed
        int x_person_tablet;
        int y_person_tablet;

        // Ratio for zooming
        float zoom_local;
        float zoom_global;

        // Initialize haptic resources
        TTexture person_texture;
        TMaterial person_material;
        TTexture global_texture;
        TMaterial global_material;
        TView myView;
        int tanvas_ydir_offset = 0; //30;

        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("Hello World!");

            if (!TanvasTouch.API.Initialize("co.tanvas.demos.examples", "CDBC90-9FD801-48E98D-C2678C-8C2E64-EC5C09"))
            {
                throw new Exception("Failed to initialize tanvas API");
            }
            else
            {
                TanvasTouch.Service.LicenseStatus myLicenseStatus = TanvasTouch.API.WaitForLicensing();
                if (myLicenseStatus != LicenseStatus.approved)
                {
                    throw new Exception("Failed to validate license");
                }
            }

            // Initialize Tanvas app
            myView = new TView();
            // Ratio for zooming
            zoom_local = H_tablet / H_unity_local;
            zoom_global = H_tablet / H_unity_global;

            // Starting position of person for initialization
            x_person_unity = 23f;
            y_person_unity = 10f;
            th_person = 0f;

            // Position of person in tablet
            x_person_tablet = W_tablet / 2;
            y_person_tablet = H_tablet / 2;

            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Person
            ///////////////////////////////////////////////////////////////////////////////////////
            int width = 0;
            int height = 0;
            // Create Material
            person_texture = new TTexture();
            person_material = new TMaterial();
            var uri = new Uri("pack://application:,,/Assets/person.png");
            byte[] data_person = ConvertPngToByteArray(uri, ref width, ref height);
            person_texture.SetData(data_person, width, height);
            person_material.AddTexture(0, person_texture);
            // Build Sprite
            Tuple<int, int> person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_person_unity)) - 1, Convert.ToInt32(Math.Round(y_person_unity)) + 1, x_person_unity, y_person_unity, zoom_local);
            TSprite person_sprite;
            person_sprite = new TSprite();
            person_sprite.Material = person_material;
            person_sprite.Width = width;
            person_sprite.Height = height;
            person_sprite.X = person_sprite_position.Item1;
            person_sprite.Y = person_sprite_position.Item2 + tanvas_ydir_offset;
            //person_sprite.PivotX = x_person_tablet;
            //person_sprite.PivotY = y_person_tablet;
            //person_sprite.Rotation = 0;
            myView.AddSprite(person_sprite);

            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Global
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            global_texture = new TTexture();
            global_material = new TMaterial();
            var uri_global = new Uri("pack://application:,,/Assets/global.png");
            width = 0; height = 0;
            byte[] data_global = ConvertPngToByteArray(uri_global, ref width, ref height);
            global_texture.SetData(data_global, width, height);
            global_material.AddTexture(0, global_texture);
            // Build Sprite
            TSprite global_sprite;
            global_sprite = new TSprite();
            global_sprite.Material = global_material;
            global_sprite.X = 0;
            global_sprite.Y = tanvas_ydir_offset - 250;
            global_sprite.Width = width;
            global_sprite.Height = height;
            global_sprite.PivotX = x_person_tablet;
            global_sprite.PivotY = y_person_tablet;
            global_sprite.Rotation = 0;
            myView.AddSprite(global_sprite);

           
            //    /////////////////////////////////////////////////////////////////////////////////////
            //                                 Update Sprites in Loop
            //    /////////////////////////////////////////////////////////////////////////////////////
            //     COMMENT REST OR PUT BRAKEPOINT HERE TO RUN EXAMPLES
            bool flag = true;
            string person_position_string = "";
            int MAX_STRING_SAVE_PREV = 50;
            char[] person_position_string_prev = new char[MAX_STRING_SAVE_PREV];
            bool strings_equal;
            int inputstringlen;
            string temp_string;
            int i, k;
            float temp_float;
            do
            {
                //Read file for person position and determine if it is new
                try
                    {
                        person_position_string = File.ReadAllText(System.IO.Path.Combine(docPath_person, "person_position.txt"));
                        //Console.WriteLine("Read person_position.txt");
                    }
                    catch
                    {
                        Console.WriteLine("Could not read person_position.txt");
                    }
                i = 0;
                inputstringlen = person_position_string.Length;
                strings_equal = (person_position_string_prev[i] == person_position_string[i]);
                while (strings_equal && (i < MAX_STRING_SAVE_PREV) && (i < inputstringlen))
                {
                    strings_equal = (strings_equal && (person_position_string_prev[i] == person_position_string[i]));
                    i++;
                    //printf("%d \n ", strings_equal);
                }

                //If the string is new, update the locally stored position of the person
                if (strings_equal)
                {
                }
                else
                {
                    //Update Tanvas
                    temp_string = "";
                    k = 0;
                    //i iterates through characters in file string
                    //k indicated how many numbers have been read to indicate xposition, yposition, and orientation
                    for (i = 0; i < inputstringlen; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(person_position_string[i]);
                        if (person_position_string[i] == ',')
                        {
                            temp_string = temp_string + '\0';
                            if (k == 0)
                            {

                                temp_float = float.Parse(temp_string);
                                x_person_unity = temp_float / 100;
                                System.Diagnostics.Debug.WriteLine("Person X position: {0}", x_person_unity);
                                k++;
                            }
                            else if (k == 1)
                            {
                                temp_float = float.Parse(temp_string);
                                y_person_unity = temp_float / 100;
                                System.Diagnostics.Debug.WriteLine("Person Y position: {0}", y_person_unity);
                                k++;
                            }
                            else if (k == 2)
                            {
                                temp_float = float.Parse(temp_string);
                                th_person = -temp_float / 1000; // make negative because the sprites take counterclockwise in radians 
                                System.Diagnostics.Debug.WriteLine("Person Theta position: {0}", th_person);
                                k++;
                            }
                            temp_string = "";
                        }
                        else // Store the next value in string
                        {
                            temp_string = temp_string + person_position_string[i];
                        }
                    }
                    //Save as new previous string
                    for (i = 0; i < inputstringlen; i++)
                    {
                        if (i == MAX_STRING_SAVE_PREV)
                        {
                            break;
                        }
                        person_position_string_prev[i] = person_position_string[i];
                    }

                    //Edit person and global sprites and place on screen 

                    //The person sprite has been made for the locel zoom_ratio, so it is no longer equal to 2 units in unity
                    int offset = 78; // The sprite for the person needs to be manually offset -78px in the x and -78px in the y
                    float unity_middle = 15f;

                    double tempx = (x_person_unity - unity_middle) * zoom_global;
                    double tempy = (y_person_unity - unity_middle) * zoom_global;

                    tempy = -tempy;

                    double x_transformed = tempx * Math.Cos(th_person) - tempy * Math.Sin(th_person);
                    double y_transformed = tempx * Math.Sin(th_person) + tempy * Math.Cos(th_person);
                    //System.Diagnostics.Debug.WriteLine("X-value equal before {0} after {1}", x_person_unity, x_transformed + unity_middle);
                    //System.Diagnostics.Debug.WriteLine("Y-value equal before {0} after {1}", y_person_unity, y_transformed + unity_middle);

                    int x_tanvas = Convert.ToInt32(Math.Round(x_transformed)) + x_person_tablet;
                    int y_tanvas = Convert.ToInt32(Math.Round(y_transformed)) + y_person_tablet;

                    //person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_transformed + unity_middle)) - 1, Convert.ToInt32(Math.Round(y_transformed + unity_middle)) + 1, unity_middle, unity_middle, zoom_global);
                    //person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_person_unity)), Convert.ToInt32(Math.Round(y_person_unity)), 15.0f, 15.0f, zoom_global);
                    person_sprite.X = person_sprite_position.Item1 - offset;
                    person_sprite.Y = person_sprite_position.Item2 - offset;

                    person_sprite.X = x_tanvas - offset;
                    person_sprite.Y = y_tanvas - offset;
                    //System.Diagnostics.Debug.WriteLine("Person X position global tanvas frame: {0}", person_sprite_position.Item1);
                    //System.Diagnostics.Debug.WriteLine("Person Y position global tanvas frame: {0}", person_sprite_position.Item2);
                    global_sprite.Rotation = th_person;
                    myView.AddSprite(global_sprite);
                    myView.AddSprite(person_sprite);
                }
            } while (flag == true);

        }

                    // Transformation for going from the unity frame to tanvas' local or global frame. 
                    // Make sure to use the correct zoom_ratio, either zoom_local or zoom_global
                    // For translating to global coordinates, use x_person_unity=y_person_unity=15
        Tuple<int, int> unitytotanvas(int x_unity, int y_unity, float x_person_unity, float y_person_unity, float zoom_ratio)
        {
            // +1 , -1

            // Flip y-axis 
            float tempy = - y_unity; 
            // Find the position of the building relative to the person at (0,0) in unity
            float tempx = x_unity - x_person_unity;
            tempy = tempy + y_person_unity;
            // Scale to tablet coordinate system
            tempx = tempx * zoom_ratio;
            tempy = tempy * zoom_ratio;
            // Position the person in the middle of the tablet and round
            int x_tanvas = Convert.ToInt32(Math.Round(tempx)) + x_person_tablet;
            int y_tanvas = Convert.ToInt32(Math.Round(tempy)) + y_person_tablet;

            return new Tuple<int, int>(x_tanvas, y_tanvas);
        }

        // From Tanvas example code
        private byte[] ConvertPngToByteArray(Uri imageUri, ref int width, ref int height)
        {
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            BitmapSource bmp = decoder.Frames[0];
            width = bmp.PixelWidth;
            height = bmp.PixelHeight;
            int bpp = bmp.Format.BitsPerPixel / 8;
            int numBytes = width * height * bpp;
            int stride = width * bpp;
            var data = new byte[numBytes];
            bmp.CopyPixels(data, width * bpp, 0);
            var textureData = new byte[numBytes / bpp];
            int i;
            double pixelLuminance;
            for (i = 0; i < numBytes / bpp; i++)
            {
                pixelLuminance = 0.21 * data[bpp * i] + 0.72 * data[bpp * i + 1] + 0.07 * data[bpp * i + 2];
                textureData[i] = (byte)(255- pixelLuminance + 0.5);
            }
            return textureData;
        }

    }


}

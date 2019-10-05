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

        string docPath = "C:/Users/numur/Desktop/Tanvas/darpahaptics/Haptic_display"; // Path for text files
        // Time vairables for detecting double tap and input
        DateTime onetap_time = DateTime.Now;
        DateTime starttime = DateTime.Now;
        DateTime lastsave_time = DateTime.Now;
        TimeSpan timediff;
        TimeSpan timediffsave;
        double timefordoubletap = .25; // min time between taps in double tap
        double timebetweenhits = 2; // min time between position recording
        double timediff_sec;
        double timediffsave_sec;
        string string_of_coordinates; // For storing coordinates in string
        bool collectdata = false; // Boolean for whether to save the data 

        // Coordinates from the unity environment with origin in the lower left-hand corner
        // Coordinates of buildings representing the upper left-hand corner of the buildings' locations
        // (note that then you would not have buildings at (0,0))
        int gridsize_unity = 30;
        int[] bld_small_x_unity = {0, 4, 14, 24, 28, // Row 1
                                   0, 4, 14, 24, 28, // Row 2
                                   0, 4, 14, 24, 28, // Row 5
                                   0, 4, 14, 24, 28, // Row 8
                                   0, 4, 14, 24, 28 // Row 9
                                    };
        int[] bld_small_y_unity = {1, 1, 1, 1, 1, // Row 1
                                   5, 5, 5, 5, 5, // Row 2
                                   15, 15, 15, 15, 15, // Row 5
                                   25, 25, 25, 25, 25, // Row 8
                                   29, 29, 29, 29, 29 // Row 9
                                    };
        int[] bld_2horiz_x_unity = {8, 18, // Row 1
                                    8, 18, // Row 2
                                    8, 18, // Row 5
                                    8, 18, // Row 8
                                    8, 18 // Row 9
                                    };
        int[] bld_2horiz_y_unity = {1, 1, // Row 1
                                    5, 5, // Row 2
                                    15, 15, // Row 5
                                    25, 25, // Row 8
                                    29, 29 // Row 9
                                    };
        int[] bld_2vert_x_unity = {0, 4, 14, 24, 28, // Row 3/4
                                   0, 4, 14, 24, 28 // Row 6/7
                                    };
        int[] bld_2vert_y_unity = {11, 11, 11, 11, 11, // Row 3/4
                                   21, 21, 21, 21, 21 // Row 6/7
                                    };
  
        int[] bld_large_x_unity = {8, 18, // Row 3/4
                                    8, 18 // Row 6/7
                                    };
        int[] bld_large_y_unity = {11, 11, // Row 3/4
                                    21, 21 // Row 6/7
                                    };

        // Starting position of person for initialization
        float x_person_unity;
        float y_person_unity;
        float th_person;

        // Distance in the unity environment to show on tablet (for height)
        int H_unity = 10;

        // Size of haptic display
        int W_tablet = 1280; //738;
        int H_tablet = 780; //1024;

        // Position on the tablet for the person to be placed
        int x_person_tablet;
        int y_person_tablet;

        // Ratio for zooming
        float zoom_ratio;

        // Initialize haptic resources
        TTexture person_texture;
        TMaterial person_material;
        TTexture bld_small_texture;
        TMaterial bld_small_material;
        TTexture bld_large_texture;
        TMaterial bld_large_material;
        TTexture bld_2horiz_texture;
        TMaterial bld_2horiz_material;
        TTexture bld_2vert_texture;
        TMaterial bld_2vert_material;
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
            zoom_ratio = H_tablet / H_unity;
            // Starting position of person for initialization
            x_person_unity = 7f;
            y_person_unity = 13f;
            th_person = 0f;
            //x_person_unity = 16f;
            //y_person_unity = 10f;
            //th_person = 0.2f;
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
            width = 0;
            height = 0;
            byte[] data_person = ConvertPngToByteArray(uri, ref width, ref height);
            person_texture.SetData(data_person, width, height);
            person_material.AddTexture(0, person_texture);
            // Build Sprite
            Tuple<int, int> person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_person_unity)) - 1, Convert.ToInt32(Math.Round(y_person_unity)) + 1, x_person_unity, y_person_unity, zoom_ratio);
            TSprite person_sprite;
            person_sprite = new TSprite();
            person_sprite.Material = person_material;
            person_sprite.Width = width;
            person_sprite.Height = height;
            person_sprite.X = person_sprite_position.Item1;
            person_sprite.Y = person_sprite_position.Item2 + tanvas_ydir_offset;
            myView.AddSprite(person_sprite);


            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Small Building
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            bld_small_texture = new TTexture();
            bld_small_material = new TMaterial();
            var uri_bld_small = new Uri("pack://application:,,/Assets/bld_small.png");
            width = 0;
            height = 0;
            byte[] data_bld_small = ConvertPngToByteArray(uri_bld_small, ref width, ref height);
            bld_small_texture.SetData(data_bld_small, width, height);
            bld_small_material.AddTexture(0, bld_small_texture);
            // Create variables to store building position in tablet coordinates to
            int num_bld_small = bld_small_x_unity.Length;
            int[] bld_small_x_tablet = new int[num_bld_small];
            int[] bld_small_y_tablet = new int[num_bld_small];
            // Starting locations
            int i;
            for (i = 0; i < num_bld_small; i++)
            {
                Tuple<int, int> result = unitytotanvas(bld_small_x_unity[i], bld_small_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                bld_small_x_tablet[i] = result.Item1;
                bld_small_y_tablet[i] = result.Item2;
                System.Diagnostics.Debug.WriteLine(" Building Position Tablet: X- {0} Y- {1}", bld_small_x_tablet[i], bld_small_y_tablet[i]);
            }
            // Create and render list of bld_small sprites 
            List<TSprite> bld_small_sprites = new List<TSprite>();
            for (i = 0; i < num_bld_small; i++)
            {
                TSprite mySprite;
                mySprite = new TSprite();
                mySprite.Material = bld_small_material;
                mySprite.Width = width;
                mySprite.Height = height;
                mySprite.X = bld_small_x_tablet[i];
                mySprite.Y = bld_small_y_tablet[i] + tanvas_ydir_offset;
                myView.AddSprite(mySprite);
                bld_small_sprites.Add(mySprite);
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Large Building
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            bld_large_texture = new TTexture();
            bld_large_material = new TMaterial();
            var uri_bld_large = new Uri("pack://application:,,/Assets/bld_large.png");
            width = 0;
            height = 0;
            byte[] data_bld_large = ConvertPngToByteArray(uri_bld_large, ref width, ref height);
            bld_large_texture.SetData(data_bld_large, width, height);
            bld_large_material.AddTexture(0, bld_large_texture);
            // Create variables to store building position in tablet coordinates to
            int num_bld_large = bld_large_x_unity.Length;
            int[] bld_large_x_tablet = new int[num_bld_large];
            int[] bld_large_y_tablet = new int[num_bld_large];
            // Starting locations
            for (i = 0; i < num_bld_large; i++)
            {
                Tuple<int, int> result = unitytotanvas(bld_large_x_unity[i], bld_large_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                bld_large_x_tablet[i] = result.Item1;
                bld_large_y_tablet[i] = result.Item2;
                System.Diagnostics.Debug.WriteLine(" Building Position Tablet: X- {0} Y- {1}", bld_large_x_tablet[i], bld_large_y_tablet[i]);
            }
            // Create and render list of bld_large sprites 
            List<TSprite> bld_large_sprites = new List<TSprite>();
            for (i = 0; i < num_bld_large; i++)
            {
                TSprite mySprite;
                mySprite = new TSprite();
                mySprite.Material = bld_large_material;
                mySprite.Width = width;
                mySprite.Height = height;
                mySprite.X = bld_large_x_tablet[i];
                mySprite.Y = bld_large_y_tablet[i] + tanvas_ydir_offset;
                myView.AddSprite(mySprite);
                bld_large_sprites.Add(mySprite);
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Two Buildings on the right/left to each other
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            bld_2horiz_texture = new TTexture();
            bld_2horiz_material = new TMaterial();
            var uri_bld_2horiz = new Uri("pack://application:,,/Assets/bld_2horiz.png");
            width = 0;
            height = 0;
            byte[] data_bld_2horiz = ConvertPngToByteArray(uri_bld_2horiz, ref width, ref height);
            bld_2horiz_texture.SetData(data_bld_2horiz, width, height);
            bld_2horiz_material.AddTexture(0, bld_2horiz_texture);
            // Create variables to store building position in tablet coordinates to
            int num_bld_2horiz = bld_2horiz_x_unity.Length;
            int[] bld_2horiz_x_tablet = new int[num_bld_2horiz];
            int[] bld_2horiz_y_tablet = new int[num_bld_2horiz];
            // Starting locations
            for (i = 0; i < num_bld_2horiz; i++)
            {
                Tuple<int, int> result = unitytotanvas(bld_2horiz_x_unity[i], bld_2horiz_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                bld_2horiz_x_tablet[i] = result.Item1;
                bld_2horiz_y_tablet[i] = result.Item2;
                System.Diagnostics.Debug.WriteLine(" Building Position Tablet: X- {0} Y- {1}", bld_2horiz_x_tablet[i], bld_2horiz_y_tablet[i]);
            }
            // Create and render list of bld_2horiz sprites 
            List<TSprite> bld_2horiz_sprites = new List<TSprite>();
            for (i = 0; i < num_bld_2horiz; i++)
            {
                TSprite mySprite;
                mySprite = new TSprite();
                mySprite.Material = bld_2horiz_material;
                mySprite.Width = width;
                mySprite.Height = height;
                mySprite.X = bld_2horiz_x_tablet[i];
                mySprite.Y = bld_2horiz_y_tablet[i] + tanvas_ydir_offset;
                myView.AddSprite(mySprite);
                bld_2horiz_sprites.Add(mySprite);
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Two Buildings on the up/down to each other
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            bld_2vert_texture = new TTexture();
            bld_2vert_material = new TMaterial();
            var uri_bld_2vert = new Uri("pack://application:,,/Assets/bld_2vert.png");
            width = 0;
            height = 0;
            byte[] data_bld_2vert = ConvertPngToByteArray(uri_bld_2vert, ref width, ref height);
            bld_2vert_texture.SetData(data_bld_2vert, width, height);
            bld_2vert_material.AddTexture(0, bld_2vert_texture);
            // Create variables to store building position in tablet coordinates to
            int num_bld_2vert = bld_2vert_x_unity.Length;
            int[] bld_2vert_x_tablet = new int[num_bld_2vert];
            int[] bld_2vert_y_tablet = new int[num_bld_2vert];
            // Starting locations
            for (i = 0; i < num_bld_2vert; i++)
            {
                Tuple<int, int> result = unitytotanvas(bld_2vert_x_unity[i], bld_2vert_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                bld_2vert_x_tablet[i] = result.Item1;
                bld_2vert_y_tablet[i] = result.Item2;
                System.Diagnostics.Debug.WriteLine(" Building Position Tablet: X- {0} Y- {1}", bld_2vert_x_tablet[i], bld_2vert_y_tablet[i]);
            }
            // Create and render list of bld_2vert sprites 
            List<TSprite> bld_2vert_sprites = new List<TSprite>();
            for (i = 0; i < num_bld_2vert; i++)
            {
                TSprite mySprite;
                mySprite = new TSprite();
                mySprite.Material = bld_2vert_material;
                mySprite.Width = width;
                mySprite.Height = height;
                mySprite.X = bld_2vert_x_tablet[i];
                mySprite.Y = bld_2vert_y_tablet[i] + tanvas_ydir_offset;
                myView.AddSprite(mySprite);
                bld_2vert_sprites.Add(mySprite);
            }

            // COMMENT TO RUN WITH TOUCH DETECTION

            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Update Sprites in Loop
            ///////////////////////////////////////////////////////////////////////////////////////
            bool flag = true;
            string person_position_string = "";
            int MAX_STRING_SAVE_PREV = 50;
            char[] person_position_string_prev = new char[MAX_STRING_SAVE_PREV];
            bool strings_equal;
            int inputstring;
            string temp_string;
            int j, k, temp_int;
            float temp_float;
            int radius_squared = (W_tablet / 2) ^ 2 + (H_tablet / 2) ^ 2;
            do
            {
                //person_position_string = "";
                try
                {
                    person_position_string = File.ReadAllText(System.IO.Path.Combine(docPath, "person_position.txt"));
                }
                catch
                {
                    Console.WriteLine("Didn't work");
                }

                i = 0;
                strings_equal = (person_position_string_prev[i] == person_position_string[i]);
                while (strings_equal && (i < MAX_STRING_SAVE_PREV))
                {
                    strings_equal = (strings_equal && (person_position_string_prev[i] == person_position_string[i]));
                    i++;
                    //printf("%d \n ", strings_equal);
                }
                if (strings_equal)
                {
                    Console.WriteLine("Strings equal");
                }
                else
                {
                    // Update Tanvas
                    inputstring = person_position_string.Length;
                    temp_string = "";
                    j = 0;
                    k = 0;
                    // i iterates through characters in file string
                    // is j needed?
                    // k indicated how many numbers have been read to indicate xposition, yposition, and orientation
                    for (i = 0; i < inputstring; i++)
                    {
                        //System.Diagnostics.Debug.WriteLine(person_position_string[i]);
                        if (person_position_string[i] == ',')
                        {
                            temp_string = temp_string + '\0';
                            if (k == 0)
                            {
                                temp_int = Convert.ToInt32(temp_string);
                                x_person_unity = temp_int;
                                System.Diagnostics.Debug.WriteLine("Person X position: {0}", temp_int);
                                k++;
                            }
                            else if (k == 1)
                            {
                                temp_int = Convert.ToInt32(temp_string);
                                y_person_unity = temp_int;
                                System.Diagnostics.Debug.WriteLine("Person Y position: {0}", temp_int);
                                k++;
                            }
                            else if (k == 2)
                            {
                                temp_float = float.Parse(temp_string);
                                th_person = temp_float;
                                System.Diagnostics.Debug.WriteLine("Person Theta position: {0}", temp_float);
                                k++;
                            }
                            temp_string = "";
                            j = 0;
                        }
                        else // Store the next vaue in string
                        {
                            temp_string = temp_string + person_position_string[i];
                            //j++;
                        }
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////
                    //                             Update Building Locations
                    ///////////////////////////////////////////////////////////////////////////////////////
                    for (i = 0; i < num_bld_small; i++)
                    {
                        Tuple<int, int> result = unitytotanvas(bld_small_x_unity[i], bld_small_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                        bld_small_x_tablet[i] = result.Item1;
                        bld_small_y_tablet[i] = result.Item2;
                    }
                    for (i = 0; i < num_bld_large; i++)
                    {
                        Tuple<int, int> result = unitytotanvas(bld_large_x_unity[i], bld_large_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                        bld_large_x_tablet[i] = result.Item1;
                        bld_large_y_tablet[i] = result.Item2;
                    }
                    for (i = 0; i < num_bld_2horiz; i++)
                    {
                        Tuple<int, int> result = unitytotanvas(bld_2horiz_x_unity[i], bld_2horiz_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                        bld_2horiz_x_tablet[i] = result.Item1;
                        bld_2horiz_y_tablet[i] = result.Item2;
                    }
                    for (i = 0; i < num_bld_2vert; i++)
                    {
                        Tuple<int, int> result = unitytotanvas(bld_2vert_x_unity[i], bld_2vert_y_unity[i], x_person_unity, y_person_unity, zoom_ratio);
                        bld_2vert_x_tablet[i] = result.Item1;
                        bld_2vert_y_tablet[i] = result.Item2;
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////
                    //                             Update Sprites
                    ///////////////////////////////////////////////////////////////////////////////////////
                    for (i = 0; i < num_bld_small; i++)
                    {
                        bld_small_sprites[i].X = bld_small_x_tablet[i];
                        bld_small_sprites[i].Y = bld_small_y_tablet[i] + tanvas_ydir_offset;
                        bld_small_sprites[i].PivotX = x_person_tablet;
                        bld_small_sprites[i].PivotY = y_person_tablet;
                        bld_small_sprites[i].Rotation = th_person;
                        myView.AddSprite(bld_small_sprites[i]);
                    }
                    for (i = 0; i < num_bld_large; i++)
                    {
                        bld_large_sprites[i].X = bld_large_x_tablet[i];
                        bld_large_sprites[i].Y = bld_large_y_tablet[i] + tanvas_ydir_offset;
                        bld_large_sprites[i].PivotX = x_person_tablet;
                        bld_large_sprites[i].PivotY = y_person_tablet;
                        bld_large_sprites[i].Rotation = th_person;
                        myView.AddSprite(bld_large_sprites[i]);
                    }
                    for (i = 0; i < num_bld_2horiz; i++)
                    {
                        bld_2horiz_sprites[i].X = bld_2horiz_x_tablet[i];
                        bld_2horiz_sprites[i].Y = bld_2horiz_y_tablet[i] + tanvas_ydir_offset;
                        bld_2horiz_sprites[i].PivotX = x_person_tablet;
                        bld_2horiz_sprites[i].PivotY = y_person_tablet;
                        bld_2horiz_sprites[i].Rotation = th_person;
                        myView.AddSprite(bld_2horiz_sprites[i]);
                    }
                    for (i = 0; i < num_bld_2vert; i++)
                    {
                        bld_2vert_sprites[i].X = bld_2vert_x_tablet[i];
                        bld_2vert_sprites[i].Y = bld_2vert_y_tablet[i] + tanvas_ydir_offset;
                        bld_2vert_sprites[i].PivotX = x_person_tablet;
                        bld_2vert_sprites[i].PivotY = y_person_tablet;
                        bld_2vert_sprites[i].Rotation = th_person;
                        myView.AddSprite(bld_2vert_sprites[i]);
                    }
                    //myView.RemoveSprite(sprites[2]);
                    //sprites.Remove(sprites[2]);
                }

            } while (flag == true);



        }
        Tuple<int, int> unitytotanvas(int x_unity, int y_unity, float x_person_unity, float y_person_unity, float zoom_ratio)
        {

            // Switch from coordinate system at lower left-hand corner to upper left-hand corner
            // no change to tempx
            float tempy = gridsize_unity - y_unity;
            // Find the position of the building relative to the person at (0,0) in unity
            float tempx = (x_unity - x_person_unity);
            tempy = (tempy - (gridsize_unity - y_person_unity));
            // Scale to tablet coordinate system
            tempx = tempx * zoom_ratio;
            tempy = tempy * zoom_ratio;
            // Position the person in the middle of the tablet and round
            int x_tanvas = Convert.ToInt32(Math.Round(tempx)) + x_person_tablet;
            int y_tanvas = Convert.ToInt32(Math.Round(tempy)) + y_person_tablet;

            return new Tuple<int, int>(x_tanvas, y_tanvas);
        }


        Tuple<int, int> tanvastounity(double x_tanvas, double y_tanvas, float x_person_unity, float y_person_unity, float zoom_ratio)
        {

            // Translate the coordinates so that the person is at (0,0)
            double tempx = x_tanvas - x_person_tablet;
            double tempy = y_tanvas - y_person_tablet;
            // Scale to unity coordinate system
            tempx = tempx / zoom_ratio;
            tempy = tempy / zoom_ratio;
            // Find the absolute position of the building in unity
            int x_unity = Convert.ToInt32(Math.Round(tempx + x_person_unity));
            int y_unity = Convert.ToInt32(Math.Round(tempy + (gridsize_unity - y_person_unity)));
            // Switch from coordinate system at upper left-hand corner to lower left-hand corner
            // no change to tempx
            y_unity = gridsize_unity - y_unity;

            return new Tuple<int, int>(x_unity, y_unity);
        }

        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            timediff = DateTime.Now - onetap_time;
            timediffsave = DateTime.Now - lastsave_time;
            timediff_sec = timediff.TotalSeconds;
            timediffsave_sec = timediffsave.TotalSeconds;
            e.Handled = true;
            if ((timediff_sec < timefordoubletap) && (timediffsave_sec > timebetweenhits))
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected");
                onetap_time = starttime;
                if (collectdata == false)
                {
                    System.Diagnostics.Debug.WriteLine("Touch posiion recording started");
                    string_of_coordinates = "";
                    collectdata = true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Saving to Text File");
                    onetap_time = starttime;
                    lastsave_time = DateTime.Now;
                    collectdata = false;
                    try
                    {
                        File.WriteAllText(System.IO.Path.Combine(docPath, "touch_locations.txt"), string_of_coordinates);
                    }
                    catch
                    {
                        Console.WriteLine("Didn't work");
                    }
                }
            }
            onetap_time = DateTime.Now;

        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            e.Handled = true;
            if (collectdata == true)
            {
                Tuple<int, int> result = tanvastounity(e.ManipulationOrigin.X, e.ManipulationOrigin.Y, x_person_unity, y_person_unity, zoom_ratio);
                if ((result.Item1 < 0) || (result.Item1 > 29) || (result.Item2 < 0) || (result.Item2 > 29))
                {
                    System.Diagnostics.Debug.WriteLine("Input out of range");
                }
                else
                {
                    string_of_coordinates += result.Item1.ToString();
                    string_of_coordinates += ",";
                    string_of_coordinates += result.Item2.ToString();
                    string_of_coordinates += ",";
                    //System.Diagnostics.Debug.WriteLine("Position Tablet: X- {0} Y- {1}", e.ManipulationOrigin.X, e.ManipulationOrigin.Y);
                    System.Diagnostics.Debug.WriteLine("Position Unity: X- {0} Y- {1}", result.Item1, result.Item2);
                }
            }
        }

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

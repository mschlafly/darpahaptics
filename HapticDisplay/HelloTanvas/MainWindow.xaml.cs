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
        string docPath_mode = "C:/Users/numur/Downloads"; // Path for text files with tanvas mode
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
            zoom_local = H_tablet / H_unity_local;
            zoom_global = H_tablet / H_unity_global;
            // Starting position of person for initialization
            x_person_unity = 7f;
            y_person_unity = 14f;
            th_person = 0f;
            //x_person_unity = 23f;
            //y_person_unity = 9f;
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
            global_sprite.Y = tanvas_ydir_offset;
            global_sprite.Width = width;
            global_sprite.Height = height;
            //myView.AddSprite(global_sprite);

            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Small Building
            ///////////////////////////////////////////////////////////////////////////////////////
            // Create Material
            bld_small_texture = new TTexture();
            bld_small_material = new TMaterial();
            var uri_bld_small = new Uri("pack://application:,,/Assets/bld_small.png");
            width = 0; height = 0;
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
                bld_small_y_unity[i] = bld_small_y_unity[i] + 1;
                Tuple<int, int> result = unitytotanvas(bld_small_x_unity[i], bld_small_y_unity[i], x_person_unity, y_person_unity, zoom_local);
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
            width = 0; height = 0;
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
                bld_large_y_unity[i] = bld_large_y_unity[i] + 1;
                Tuple<int, int> result = unitytotanvas(bld_large_x_unity[i], bld_large_y_unity[i], x_person_unity, y_person_unity, zoom_local);
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
            width = 0; height = 0;
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
                bld_2horiz_y_unity[i] = bld_2horiz_y_unity[i] + 1;
                Tuple<int, int> result = unitytotanvas(bld_2horiz_x_unity[i], bld_2horiz_y_unity[i], x_person_unity, y_person_unity, zoom_local);
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
            width = 0; height = 0;
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
                bld_2vert_y_unity[i] = bld_2vert_y_unity[i] + 1;
                Tuple<int, int> result = unitytotanvas(bld_2vert_x_unity[i], bld_2vert_y_unity[i], x_person_unity, y_person_unity, zoom_local);
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

            ///////////////////////////////////////////////////////////////////////////////////////
            //                             Update Sprites in Loop
            ///////////////////////////////////////////////////////////////////////////////////////
            // COMMENT REST OR PUT BRAKEPOINT HERE TO RUN EXAMPLES
            bool flag = true;
            string person_position_string = "";
            int MAX_STRING_SAVE_PREV = 50;
            char[] person_position_string_prev = new char[MAX_STRING_SAVE_PREV];
            bool strings_equal;
            int inputstringlen;
            string temp_string;
            int j, k;
            float temp_float;
            string mode = "l";
            string mode_prev = "l";
            do
            {
                // Read file for tanvas mode
                try
                {
                    mode = File.ReadAllText(System.IO.Path.Combine(docPath_mode, "tanvas_mode.txt"));
                }
                catch
                {
                    Console.WriteLine("Could not read tanvas_mode.txt");
                }

                // Read file for person position and determine if it is new
                try
                {
                    person_position_string = File.ReadAllText(System.IO.Path.Combine(docPath_person, "person_position.txt"));
                }
                catch
                {
                    Console.WriteLine("Could not read person_position.txt");
                }
                i = 0;
                inputstringlen = person_position_string.Length;
                strings_equal = (person_position_string_prev[i] == person_position_string[i]);
                while (strings_equal && (i < MAX_STRING_SAVE_PREV))
                {
                    strings_equal = (strings_equal && (person_position_string_prev[i] == person_position_string[i]));
                    i++;
                    //printf("%d \n ", strings_equal);
                }

                // If the string is new, update the locally-stored position of the person
                if (strings_equal)
                {
                }
                else
                {
                    // Update Tanvas
                    temp_string = "";
                    k = 0;
                    // i iterates through characters in file string
                    // k indicated how many numbers have been read to indicate xposition, yposition, and orientation
                    for (i = 0; i < inputstringlen; i++)
                    {
                        //System.Diagnostics.Debug.WriteLine(person_position_string[i]);
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
                    // Save as new previous string
                    for (i = 0; i < inputstringlen; i++)
                    {
                        if (i == MAX_STRING_SAVE_PREV)
                        {
                            break;
                        }
                        person_position_string_prev[i] = person_position_string[i];
                    }
                }

                // If the tanvas is in global mode, set global sprite, delete building sprites, place person 
                if (mode == "g")
                {
                    // Switch modes
                    if (mode_prev == "l") 
                    {
                        // Delete building sprites (may need to remove from list?) 
                        for (i = 0; i < num_bld_small; i++)
                        {
                            myView.RemoveSprite(bld_small_sprites[i]);
                        }
                        for (i = 0; i < num_bld_large; i++)
                        {
                            myView.RemoveSprite(bld_large_sprites[i]);
                        }
                        for (i = 0; i < num_bld_2horiz; i++)
                        {
                            myView.RemoveSprite(bld_2horiz_sprites[i]);
                        }
                        for (i = 0; i < num_bld_2vert; i++)
                        {
                            myView.RemoveSprite(bld_2vert_sprites[i]);
                        }

                        // Add the global sprite 
                        myView.AddSprite(global_sprite);

                        strings_equal = false;
                        mode_prev = "g";
                    }

                    // If the display needs changing
                    if (strings_equal)
                    {
                    }
                    else
                    {
                        // Place person sprite on screen
                        // The person sprite has been made for the locel zoom_ratio, so it is no longer equal to 2 units in unity
                        int offset = 78; // The sprite for the person needs to be manually offset -78px in the x and -78px in the y
                        person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_person_unity)), Convert.ToInt32(Math.Round(y_person_unity)), 15.0f, 15.0f, zoom_global);
                        person_sprite.X = person_sprite_position.Item1 - offset;
                        person_sprite.Y = person_sprite_position.Item2 - offset;
                        myView.AddSprite(person_sprite);
                        System.Diagnostics.Debug.WriteLine("Person X position global tanvas frame: {0}", person_sprite_position.Item1);
                        System.Diagnostics.Debug.WriteLine("Person Y position global tanvas frame: {0}", person_sprite_position.Item2);
                    }
                }
                else if (mode == "l")
                {
                    // Switch modes
                    if (mode_prev == "g")
                    {
                        // Correct for global settings: remove global sprite and re-place person
                        myView.RemoveSprite(global_sprite); // Remove the global sprite 
                        person_sprite_position = unitytotanvas(Convert.ToInt32(Math.Round(x_person_unity)) - 1, Convert.ToInt32(Math.Round(y_person_unity)) + 1, x_person_unity, y_person_unity, zoom_local);
                        person_sprite.X = person_sprite_position.Item1;
                        person_sprite.Y = person_sprite_position.Item2 + tanvas_ydir_offset;
                        myView.AddSprite(person_sprite);
                        strings_equal = false;
                        mode_prev = "l";
                    }

                    // If the display needs changing
                    if (strings_equal)
                    {
                        // Console.WriteLine("Strings equal");
                    }
                    else  
                    {
                        ///////////////////////////////////////////////////////////////////////////////////////
                        //                             Update Building Locations
                        ///////////////////////////////////////////////////////////////////////////////////////
                        for (i = 0; i < num_bld_small; i++)
                        {
                            Tuple<int, int> result = unitytotanvas(bld_small_x_unity[i], bld_small_y_unity[i], x_person_unity, y_person_unity, zoom_local);
                            bld_small_x_tablet[i] = result.Item1;
                            bld_small_y_tablet[i] = result.Item2;
                        }
                        for (i = 0; i < num_bld_large; i++)
                        {
                            Tuple<int, int> result = unitytotanvas(bld_large_x_unity[i], bld_large_y_unity[i], x_person_unity, y_person_unity, zoom_local);
                            bld_large_x_tablet[i] = result.Item1;
                            bld_large_y_tablet[i] = result.Item2;
                        }
                        for (i = 0; i < num_bld_2horiz; i++)
                        {
                            Tuple<int, int> result = unitytotanvas(bld_2horiz_x_unity[i], bld_2horiz_y_unity[i], x_person_unity, y_person_unity, zoom_local);
                            bld_2horiz_x_tablet[i] = result.Item1;
                            bld_2horiz_y_tablet[i] = result.Item2;
                        }
                        for (i = 0; i < num_bld_2vert; i++)
                        {
                            Tuple<int, int> result = unitytotanvas(bld_2vert_x_unity[i], bld_2vert_y_unity[i], x_person_unity, y_person_unity, zoom_local);
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
                } else { Console.WriteLine("tanvas_mode is not recognized"); }
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

        //// Transformation for going from tanvas' local or global frame to unity's frame. 
        //// Make sure to use the correct zoom_ratio, either zoom_local or zoom_global
        //// For translating to global coordinates, use x_person_unity=y_person_unity=15
        //Tuple<int, int> tanvastounity(double x_tanvas, double y_tanvas, float x_person_unity, float y_person_unity, float zoom_ratio)
        //{

        //    // Translate the coordinates so that the person is at (0,0)
        //    double tempx = x_tanvas - x_person_tablet;
        //    double tempy = y_tanvas - y_person_tablet;
        //    // Scale to unity coordinate system
        //    tempx = tempx / zoom_ratio;
        //    tempy = tempy / zoom_ratio;
        //    // Find the absolute position of the building in unity
        //    int x_unity = Convert.ToInt32(Math.Round(tempx + x_person_unity));
        //    int y_unity = Convert.ToInt32(Math.Round(tempy + (gridsize_unity - y_person_unity)));
        //    // Switch from coordinate system at upper left-hand corner to lower left-hand corner
        //    // no change to tempx
        //    y_unity = gridsize_unity - y_unity;

        //    return new Tuple<int, int>(x_unity, y_unity);
        //}

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

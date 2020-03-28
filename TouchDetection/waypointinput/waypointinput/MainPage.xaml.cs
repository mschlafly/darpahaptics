using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.Storage.Pickers;

namespace waypointinput
{
    public sealed partial class MainPage : Page
    {
        // Size of haptic display
        int W_tablet = 1280; //738;
        int H_tablet = 780; //1024;

        // Distance in the unity environment to show on tablet (for height)
        int H_unity_local = 10;
        int H_unity_global = 30;

        // Position of person in tablet
        int x_person_tablet;
        int y_person_tablet;

        // Variables for saving data to txt file
        StorageFile file_touch;
        bool collectdata = false; // Boolean for whether to save the data 

        // For checking to see if the same unity coordinate is repeated
        int x_unity_prev = 0;
        int y_unity_prev = 0;

        // For setting the agentID 
        Windows.UI.Input.PointerPoint prevPoint;
        bool setting_agent_ID = false;
        int wait_time = 4500; // ms - time period to specify how many drones
        int drone_number = 0;

        // For saving and recieving info from files
        String string_of_coordinates = "";
        MediaElement set_agent = new MediaElement();
        MediaElement mysong_start = new MediaElement();
        MediaElement mysong_end = new MediaElement();

        public MainPage()
        {
            this.InitializeComponent();

            // Position of person in tablet
            x_person_tablet = W_tablet / 2;
            y_person_tablet = H_tablet / 2;

            mainCanvas.DoubleTapped += new DoubleTappedEventHandler(target_DoubleTapped);
            mainCanvas.PointerMoved += new PointerEventHandler(Pointer_Moved);
            mainCanvas.Tapped += new TappedEventHandler(target_Tapped);

            pickfiles();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }
        // This functin is run at the beginning and opens the picker to select files from the 
        // downloads folder to make sounds and to save the touch coordinates to
        private async void pickfiles()
        {

            // Words downloaded from 
            System.Diagnostics.Debug.WriteLine("Pick audio file sound2.wav from Downloads folder");
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".wav");
            StorageFile file = await openPicker.PickSingleFileAsync();
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            set_agent.SetSource(stream, file.ContentType);

            // Words downloaded from learners dictionary.com
            System.Diagnostics.Debug.WriteLine("Pick audio file begin001.mp3 from Downloads folder");
            openPicker.FileTypeFilter.Add(".mp3");
            file = await openPicker.PickSingleFileAsync();
            stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong_start.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick audio file done0001.mp3 from Downloads folder");
            file = await openPicker.PickSingleFileAsync();
            stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong_end.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick text file touch_locations.txt to write to.");
            openPicker.FileTypeFilter.Add(".txt");
            file_touch = await openPicker.PickSingleFileAsync();

            savefile(string_of_coordinates);
        }
        // If the a double-tap is detected, either begin collecting data or save the data, 
        // depending on the state of the collectdata variable
        void target_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            if (collectdata == true)
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected - Waiting for agentID taps.");
                mysong_end.Play();  // Plays audio file
                savefile(string_of_coordinates);
                setting_agent_ID = true;
                collectdata = false;
                string_of_coordinates = "";

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected - Data collection starting now.");
                mysong_start.Play();  // Plays audio file
                collectdata = true;
            }
        }
        // If the pointer is moved and you are collecting data, 
        // convert the pixel coordinates to "Unity" coordinates (a 30x30 grid with the origin in the lower left hand corner),
        // and save the data to a string
        void Pointer_Moved(object sender, PointerRoutedEventArgs e)
        {

            if (collectdata == true)
            {
                // Retrieve the point associated with the current event
                Windows.UI.Input.PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
                prevPoint = currentPoint; // so that it can be referenced later if the taps are in the same spot
                // translate the coordinates so that the person is at (0,0)
                double tempx = currentPoint.Position.X - x_person_tablet;
                double tempy = currentPoint.Position.Y - y_person_tablet;
                // NOTE THE REST OF THE CONVERSION HAS BEEN REMOVES AND PERFORMED AFTER SENDING OVER TCP SOCKET
                int x_unity = Convert.ToInt32(Math.Round(tempx));
                int y_unity = Convert.ToInt32(Math.Round(tempy));
                if ((x_unity_prev == x_unity) && (y_unity_prev == y_unity))
                {
                    //System.Diagnostics.Debug.WriteLine("Repeat Coordinate- skipped");
                }
                else
                {
                    string_of_coordinates = string_of_coordinates + x_unity.ToString() + ',' + y_unity.ToString() + ',';
                    System.Diagnostics.Debug.WriteLine("Position: X- {0} Y- {1}", x_unity, y_unity);
                }
                x_unity_prev = x_unity;
                y_unity_prev = y_unity;
            }
        }
        // When the tanvas is tapped and if the tap is both after the user finished sending a 
        // path and tapped the screen in the same location as the last double-tap, add 1 to the drone number
        async void target_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (setting_agent_ID)
            {
                System.Diagnostics.Debug.WriteLine("Tap Detected for setting agent ID");
                Windows.Foundation.Point currentPoint = e.GetPosition(mainCanvas);
                int range_accepted = 50;
                int current_x = Convert.ToInt32(Math.Round(currentPoint.X));
                int current_y = Convert.ToInt32(Math.Round(currentPoint.Y));
                int prev_x = Convert.ToInt32(Math.Round(prevPoint.Position.X));
                int prev_y = Convert.ToInt32(Math.Round(prevPoint.Position.Y));

                if ((Math.Abs(prev_x - current_x) < range_accepted) && (Math.Abs(prev_y - current_y) < range_accepted))
                {
                    //System.Diagnostics.Debug.WriteLine("Near previous double tap");
                    drone_number = drone_number + 1; // Starts at 1 and add one every time there is one in the range
                    set_agent.Play();  // Plays audio file
                }
            }
        }
        // Save the string_of_coordinates to touch_locations.txt
        private async void savefile(String save_coordinates)
        {
            await System.Threading.Tasks.Task.Delay(wait_time); // wait to allow the user to input number of drones before sending
            setting_agent_ID = false;

            if ((drone_number > 0) && (drone_number < 4))
            {
                String string_of_coordinates2 = save_coordinates;
                if (string_of_coordinates2 == "") { } else
                {
                    drone_number = drone_number - 1;
                    string_of_coordinates2 = drone_number.ToString() + ',' + string_of_coordinates2;
                }
                bool need_save_file = true;
                // In our code, we index the drones (0,1,and 2), but display the drone number in the output as (1,2,or 3)
                System.Diagnostics.Debug.WriteLine("Drone number: {0}", drone_number+1); 
                while (need_save_file)
                {
                    need_save_file = false;
                    try
                    {
                        await Windows.Storage.FileIO.WriteTextAsync(file_touch, string_of_coordinates2);
                        System.Diagnostics.Debug.WriteLine("Saved touch coordinates");

                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine("Exception occurred while saving file.");
                        need_save_file = true;
                    }

                }
            }
            drone_number = 0;
        }
    }

}
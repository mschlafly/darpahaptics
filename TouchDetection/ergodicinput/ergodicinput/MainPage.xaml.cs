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

namespace ergodicinput
{
    public sealed partial class MainPage : Page
    {
        // Size of haptic display
        int W_tablet = 1280; //738;
        int H_tablet = 780; //1024;

        // Position of person in tablet
        int x_person_tablet;
        int y_person_tablet;

        // Variables for saving data to txt file
        StorageFile file_touch;
        bool collectdata = false; // Boolean for whether to save the data 

        // For checking to see if the same unity coordinate is repeated
        int x_unity_prev = 0;
        int y_unity_prev = 0;
        
        // For saving and recieving info from files
        String string_of_coordinates = "";
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
            pickfiles();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }
        // This functin is run at the beginning and opens the picker to select files from the 
        // downloads folder to make sounds and to save the touch coordinates to
        private async void pickfiles()
        {
            // Words downloaded from learners dictionary.com
            System.Diagnostics.Debug.WriteLine("Pick audio file begin001.mp3 from Downloads folder");
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".mp3");
            StorageFile file = await openPicker.PickSingleFileAsync();
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong_start.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick audio file done0001.mp3 from Downloads folder");
            file = await openPicker.PickSingleFileAsync();
            stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong_end.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick text file touch_locations.txt to write to.");
            openPicker.FileTypeFilter.Add(".txt");
            file_touch = await openPicker.PickSingleFileAsync();

            savefile();
        }
        // If the a double-tap is detected, either begin collecting data or save the data, 
        // depending on the state of the collectdata variable
        void target_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            if (collectdata == true)
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected - Saving data.");

                mysong_end.Play();  // Plays audio file
                savefile();
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
                // translate the coordinates so that the person is at (0,0)
                double tempx = currentPoint.Position.X - x_person_tablet;
                double tempy = currentPoint.Position.Y - y_person_tablet;
                // NOTE THE REST OF THE CONVERSION HAS BEEN REMOVES AND PERFORMED AFTER SENDING OVER TCP SOCKET
                int x_unity = Convert.ToInt32(Math.Round(tempx));
                int y_unity = Convert.ToInt32(Math.Round(tempy));
                if (string_of_coordinates == "")
                {
                    string_of_coordinates = string_of_coordinates + "5,";
                }
                if ((x_unity_prev==x_unity) && (y_unity_prev == y_unity))
                {
                    //System.Diagnostics.Debug.WriteLine("Repeat Coordinate- skipped");
                } else
                {
                    string_of_coordinates = string_of_coordinates + x_unity.ToString() + ',' + y_unity.ToString() + ',';
                    System.Diagnostics.Debug.WriteLine("Position: X- {0} Y- {1}", x_unity, y_unity);
                }
                x_unity_prev = x_unity;
                y_unity_prev = y_unity;
            }
        }
        // Save the string_of_coordinates to touch_locations.txt
        private async void savefile()
        {
            bool need_save_file = true;
            String string_of_coordinates2 = string_of_coordinates;
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
    }
}

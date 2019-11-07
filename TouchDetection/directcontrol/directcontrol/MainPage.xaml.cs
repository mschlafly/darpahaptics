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

// Set multiple_drones variable!

namespace directcontrol
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile newFile;

        bool multiple_drones = true;

        int wait_time = 3500; // ms - time period to send all commands
        // Size of haptic display
        int W_tablet = 1280; //738;
        int H_tablet = 780; //1024;

        // Distance in the unity environment to show on tablet (for height)
        int H_unity = 10;

        // Position of person in tablet
        int x_person_tablet;
        int y_person_tablet;
        // Ratio for zooming
        float zoom_ratio;

        int drone_number = 0;
        int drone_number2 = 0;
        bool use_save_file1 = true;
        int previoustap_x = 0;
        int previoustap_y = 0;
        int previoustap_x2 = 0;
        int previoustap_y2 = 0;

        MediaElement mysong1 = new MediaElement();
        MediaElement mysong2 = new MediaElement();

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            // Position of person in tablet
            x_person_tablet = W_tablet / 2;
            y_person_tablet = H_tablet / 2;
            // Ratio for zooming
            zoom_ratio = H_tablet / H_unity;


            mainCanvas.Tapped += new TappedEventHandler(target_Tapped);

            pickfiles();
        }
        private async void pickfiles()
        {
            System.Diagnostics.Debug.WriteLine("Pick audio file sound1.wav from Downloads folder");
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker.FileTypeFilter.Add(".wav");
            StorageFile file = await openPicker.PickSingleFileAsync();
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong1.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick audio file sound1.wav from Downloads folder");
            FileOpenPicker openPicker3 = new FileOpenPicker();
            openPicker3.ViewMode = PickerViewMode.Thumbnail;
            openPicker3.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker3.FileTypeFilter.Add(".wav");
            file = await openPicker.PickSingleFileAsync();
            stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong2.SetSource(stream, file.ContentType);

            System.Diagnostics.Debug.WriteLine("Pick text file touch_locations.txt to write to.");
            FileOpenPicker openPicker2 = new FileOpenPicker();
            openPicker2.ViewMode = PickerViewMode.Thumbnail;
            openPicker2.SuggestedStartLocation = PickerLocationId.Downloads;
            openPicker2.FileTypeFilter.Add(".txt");
            newFile = await openPicker2.PickSingleFileAsync();
        }
        async void target_Tapped(object sender, TappedRoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Tap Detected");

            Windows.Foundation.Point currentPoint = e.GetPosition(mainCanvas);
            int range_accepted = 50;
            int current_x = Convert.ToInt32(Math.Round(currentPoint.X));
            int current_y = Convert.ToInt32(Math.Round(currentPoint.Y));

            if (use_save_file1)
            {
                if ((Math.Abs(previoustap_x - current_x) < range_accepted) && (Math.Abs(previoustap_y - current_y) < range_accepted))
                {
                    //System.Diagnostics.Debug.WriteLine("Near previous tap");
                    if (drone_number == 0)
                    {
                        savefile();
                    }
                    drone_number = drone_number + 1; // Starts at 1 and add one every time there is one in the range
                    mysong1.Play();  // Plays audio file
                } else
                {
                    use_save_file1 = false;
                }
            } else
            {
                if ((Math.Abs(previoustap_x2 - current_x) < range_accepted) && (Math.Abs(previoustap_y2 - current_y) < range_accepted))
                {
                    //System.Diagnostics.Debug.WriteLine("Near previous tap");
                    if (drone_number2 == 0)
                    {
                        savefile2();
                    }
                    drone_number2 = drone_number2 + 1; // Starts at 1 and add one every time there is one in the range
                    mysong2.Play();  // Plays audio file
                }
                else
                {
                    use_save_file1 = true;
                }
            }
            if (drone_number == 0)
            {
                previoustap_x = current_x;
                previoustap_y = current_y;
            }
            if (drone_number2 == 0)
            {
                previoustap_x2 = current_x;
                previoustap_y2 = current_y;
            }
        }

        private async void savefile()
        {
            await System.Threading.Tasks.Task.Delay(wait_time); // wait for 2 seconds (= 2000ms)

            if ((drone_number > 0) && (drone_number < 4))
            {
                // If a valid number of drones            // translate the coordinates so that the person is at (0,0)
                double tempx = previoustap_x - x_person_tablet;
                double tempy = previoustap_y - y_person_tablet;
                // scale to unity coordinate system
                tempx = tempx / zoom_ratio;
                tempy = tempy / zoom_ratio;
                // NOTE THE REST OF THE CONVERSION HAS BEEN REMOVES AND PERFORMED AFTER SENDING OVER TCP SOCKET
                int x_unity = Convert.ToInt32(Math.Round(tempx));
                int y_unity = Convert.ToInt32(Math.Round(tempy));

                if (multiple_drones)
                {
                    drone_number = drone_number - 1; // So that indexing for drones starts at 0
                } else
                {
                    drone_number = 0;
                }
                String string_of_coordinates = drone_number.ToString() + ',' + x_unity.ToString() + ',' + y_unity.ToString() + ',' + '!';
                System.Diagnostics.Debug.WriteLine("Drone number: {0} Position: X- {1} Y- {2}", drone_number, x_unity, y_unity);
                await Windows.Storage.FileIO.WriteTextAsync(newFile, string_of_coordinates);
            }
            drone_number = 0;
        }

        private async void savefile2()
        {
            await System.Threading.Tasks.Task.Delay(wait_time); // wait for 2 seconds (= 2000ms)

            if ((drone_number > 0) && (drone_number < 4))
            {
                // If a valid number of drones            // translate the coordinates so that the person is at (0,0)
                double tempx = previoustap_x - x_person_tablet;
                double tempy = previoustap_y - y_person_tablet;
                // scale to unity coordinate system
                tempx = tempx / zoom_ratio;
                tempy = tempy / zoom_ratio;
                // NOTE THE REST OF THE CONVERSION HAS BEEN REMOVES AND PERFORMED AFTER SENDING OVER TCP SOCKET
                int x_unity = Convert.ToInt32(Math.Round(tempx));
                int y_unity = Convert.ToInt32(Math.Round(tempy));

                if (multiple_drones)
                {
                    drone_number2 = drone_number2 - 1; // So that indexing for drones starts at 0
                }
                else
                {
                    drone_number2 = 0; 
                }
                String string_of_coordinates = drone_number2.ToString() + ',' + x_unity.ToString() + ',' + y_unity.ToString() + ',' + '!';
                System.Diagnostics.Debug.WriteLine("Drone number: {0} Position: X- {1} Y- {2}", drone_number2, x_unity, y_unity);
                await Windows.Storage.FileIO.WriteTextAsync(newFile, string_of_coordinates);
            }
            drone_number2 = 0;
        }
    }
}

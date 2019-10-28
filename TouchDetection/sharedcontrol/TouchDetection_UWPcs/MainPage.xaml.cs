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


using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace sharedcontrol
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile newFile;
        bool file_created = false;

        bool collectdata = false; // Boolean for whether to save the data 
        bool doubletapended = false;
        bool doubletapped = false;


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

        String string_of_coordinates = "";

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        //MaximizeWindowOnLoad();

            // Position of person in tablet
            x_person_tablet = W_tablet / 2;
            y_person_tablet = H_tablet / 2;
            // Ratio for zooming
            zoom_ratio = H_tablet / H_unity;

            mainCanvas.PointerPressed += new PointerEventHandler(Pointer_Pressed);
            mainCanvas.PointerMoved += new PointerEventHandler(Pointer_Moved);
            mainCanvas.PointerReleased += new PointerEventHandler(Pointer_Released);
            mainCanvas.DoubleTapped += new DoubleTappedEventHandler(target_DoubleTapped);

        }

        //private static void MaximizeWindowOnLoad()
        //{
        //    ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        //}

        void target_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            if (collectdata == true)
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected - Saving data.");

                savefile();

                collectdata = false;
                string_of_coordinates = "";

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Double Tap Detected - Data collection starting now.");
                collectdata = true;
            }
        }

        void Pointer_Pressed(object sender, PointerRoutedEventArgs e)
        {
            //Windows.UI.Input.PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        }

        void Pointer_Moved(object sender, PointerRoutedEventArgs e)
        {

            if (collectdata == true)
            {
                // Retrieve the point associated with the current event
                Windows.UI.Input.PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);


                // translate the coordinates so that the person is at (0,0)
                double tempx = currentPoint.Position.X - x_person_tablet;
                double tempy = currentPoint.Position.X - y_person_tablet;
                // scale to unity coordinate system
                tempx = tempx / zoom_ratio;
                tempy = tempy / zoom_ratio;

                // NOTE THE REST OF THE CONVERSION HAS BEEN REMOVES AND PERFORMED AFTER SENDING OVER TCP SOCKET


                int x_unity = Convert.ToInt32(Math.Round(tempx));
                int y_unity = Convert.ToInt32(Math.Round(tempy));

                string_of_coordinates = string_of_coordinates + x_unity.ToString() + ',' + y_unity.ToString() + ',';

                System.Diagnostics.Debug.WriteLine("Position: X- {0} Y- {1}", x_unity, y_unity);

            }

        }

        void Pointer_Released(object sender, PointerRoutedEventArgs e)
        {
            // Retrieve the point associated with the current event
            //Windows.UI.Input.PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);

        }


        private async void savefile()
        {
            if (!file_created)
            {

                newFile = await DownloadsFolder.CreateFileAsync("touch_locations.txt");
                file_created = true;
            }

            await Windows.Storage.FileIO.WriteTextAsync(newFile, string_of_coordinates);

        }

        //private async void CreateFileButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (!file_created)
        //    {

        //        newFile = await DownloadsFolder.CreateFileAsync("file.txt");
        //        file_created = true;
        //    }

        //    await Windows.Storage.FileIO.WriteTextAsync(newFile, "Swift as a shadow");
        //    await Windows.Storage.FileIO.WriteTextAsync(newFile, "Fast as a bird");

        //}
    }
}

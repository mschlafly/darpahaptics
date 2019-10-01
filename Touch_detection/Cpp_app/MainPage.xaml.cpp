//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace Cpp_app;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;



using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;


using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Navigation;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Provider;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

// For writing from file
using namespace std;
#include <string>
#include <iostream>
#include <fstream>
// Set up file for reading 
String^ docPath = "C:/Users/brandon/Desktop/Tanvas/DARPA_haptics/";
String^ docPath_touch = "C:/Users/brandon/Desktop/Tanvas/DARPA_haptics/touch_locations.txt";
int filefetch_wait = 500; // Milliseconds between trying to open file

String^ string_of_coordinates = ""; // For storing coordinates in string


string string_of_coordinates_std = ""; // For storing coordinates in string
string docPath_std = "C:/Users/brandon/Desktop/Tanvas/DARPA_haptics/";
string docPath_touch_std = "C:/Users/brandon/Desktop/Tanvas/DARPA_haptics/touch_locations.txt";

bool collectdata = false; // Boolean for whether to save the data 
bool doubletapended = false; 
bool doubletapped = false; 
#define MAX_NUMBERS 500
unsigned __int16 pointer_position_x[MAX_NUMBERS];
unsigned __int16 index;


// Size of haptic display
int W_tablet = 1280; //738;
int H_tablet = 780; //1024;
// Distance in the unity environment to show on tablet (for height)
int H_unity = 10;
// Position of person in tablet
int x_person_tablet = W_tablet / 2;
int y_person_tablet = H_tablet / 2;
// Ratio for zooming
float zoom_ratio = H_tablet / H_unity;


//tuple<int, int> tanvastounity_partialconversion(double x_tanvas, double y_tanvas, float zoom_ratio)
//{
//	// Translate the coordinates so that the person is at (0,0)
//	double tempx = x_tanvas - x_person_tablet;
//	double tempy = y_tanvas - y_person_tablet;
//	// Scale to unity coordinate system
//	tempx = tempx / zoom_ratio;
//	tempy = tempy / zoom_ratio;
//	// conversion finished in linux
//	int x_unity = int(round(tempx));
//	int y_unity = int(round(tempy));
//	//// Find the absolute position of the building in unity
//	//int x_unity = Convert.ToInt32(Math.Round(tempx)) + x_person_unity;
//	//int y_unity = Convert.ToInt32(Math.Round(tempy)) + (gridsize_unity - y_person_unity);
//	//// Switch from coordinate system at upper left-hand corner to lower left-hand corner
//	//// no change to tempx
//	//y_unity = gridsize_unity - y_unity;
//
//	return tuple<int, int>(x_unity, y_unity);
//}


MainPage::MainPage()
{
	InitializeComponent();
	OutputDebugStringW(L"Print Works!\n");

	// Launch in full screen mode
	ApplicationView::PreferredLaunchWindowingMode = 1 ? ApplicationViewWindowingMode::FullScreen : ApplicationViewWindowingMode::Auto;

	mainCanvas->PointerPressed += ref new PointerEventHandler(this, &Cpp_app::MainPage::Pointer_Pressed);
	mainCanvas->PointerMoved += ref new PointerEventHandler(this, &Cpp_app::MainPage::Pointer_Moved);
	mainCanvas->PointerReleased += ref new PointerEventHandler(this, &Cpp_app::MainPage::Pointer_Released);
	mainCanvas->DoubleTapped += ref new DoubleTappedEventHandler(this, &Cpp_app::MainPage::target_DoubleTapped);



}

void Cpp_app::MainPage::Pointer_Pressed(Platform::Object^ sender, PointerRoutedEventArgs^ e)
{
	//OutputDebugStringW(L"Collecting Data");
	//string_of_coordinates = "";
	//OutputDebugStringW(L"Pointer Pressed\n");
	//index = 0;
	//OutputDebugStringW(L"Touch Detected!\n");

	//Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);

	//OutputDebugString(("X: " + currentPoint->Position.X + "    Y: " + currentPoint->Position.Y + "\n")->Data());
	// Retrieve the properties for the curret PointerPoint and display
	// them alongside the pointer's location on screen
	//CreateOrUpdatePropertyPopUp(currentPoint);
}

void Cpp_app::MainPage::Pointer_Moved(Platform::Object^ sender, PointerRoutedEventArgs^ e)
{
	if (collectdata == true)
	{
		// Retrieve the point associated with the current event
		Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);


		// translate the coordinates so that the person is at (0,0)
		double tempx = currentPoint->Position.X - x_person_tablet;
		double tempy = currentPoint->Position.Y - y_person_tablet;
		// scale to unity coordinate system
		tempx = tempx / zoom_ratio;
		tempy = tempy / zoom_ratio;
		// conversion finished in linux
		int x_unity = int(round(tempx));
		int y_unity = int(round(tempy));


		//if ((x_unity < 0) || (x_unity > 29) || (y_unity < 0) || (y_unity > 29))
		//{
		//	OutputDebugStringW(L"Input out of range \n");
		//}
		//tuple<int, int> result = tanvastounity_partialconversion(currentPoint->Position.X, currentPoint->Position.Y, zoom_ratio);
		////if ((get<0>(result) < 0) || (get<0>(result) > 29) || (get<1>(result) < 0) || (get<1>(result) > 29))
		////{
		////	OutputDebugStringW(L"Input out of range");
		////}
		//else {
			//string_of_coordinates += to_string(get<0>(result));
			//string_of_coordinates += ",";
			//string_of_coordinates += to_string(get<1>(result));
			//string_of_coordinates += ",";
			//OutputDebugString(("X: " + get<0>(result) + "    Y: " + get<1>(result) + "\n")->Data());
	/*	x_unity
		ToString(x_unity);*/



		//String^ x_unity_string = x_unity.ToString;
		////String to char
		//string hi = "hello";
		//String hi2 = Platform::String(L"hello",hi.length)
		//char16 * newText = string_of_coordinates.Data();
		//string_of_coordinates = Platform::String::Concat(x_unity.ToString,",");
		//Platform::String::ToString(x_unity);
		//Convert::ToString()
		//(x_unity)->Data()
	/*	string_of_coordinates = string_of_coordinates + x_unity.ToString;
		string_of_coordinates = string_of_coordinates + ",";
		string_of_coordinates = string_of_coordinates + y_unity.ToString;
		string_of_coordinates = string_of_coordinates + ",";*/
		OutputDebugString(("X: " + x_unity + "    Y: " + y_unity + "\n")->Data());
		//}
	}
	//if (collectdata == true) {
	//	Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);
	//	OutputDebugString(("X: " + currentPoint->Position.X + "    Y: " + currentPoint->Position.Y + "\n")->Data());
	//}
		//index = index + 1;
}

void Cpp_app::MainPage::Pointer_Released(Platform::Object^ sender, PointerRoutedEventArgs^ e)
{
	//if ((doubletapped == true) && (collectdata == false)) {
	//	collectdata = true;
	//	doubletapped = false;
	//	//OutputDebugStringW(L"begin collecting.\n");
	//}
	//else if (collectdata == true) {
	//	//OutputDebugStringW(L"Pointer released - Saving data.\n");
	//	//doubletapped = false;
	//	//collectdata = false;
	//}

	// Retrieve the point associated with the current event
	//Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);
		
	//Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);
	//OutputDebugString(("X: " + currentPoint->Position.X + "    Y: " + currentPoint->Position.Y + "\n")->Data());

}

void Cpp_app::MainPage::target_DoubleTapped(Platform::Object^ sender, DoubleTappedRoutedEventArgs^ e)
{
	if (collectdata == true) {
		OutputDebugStringW(L"Double Tap Detected - Saving data.\n");
		

		string_of_coordinates_std = "Hello!";
		int flag2 = 0; // For determining if the file has been opened properly
		fstream file_personlocation;
		do {
			file_personlocation.open(docPath_touch_std);
			// Check is file has been opened properly, if not wait 
			// (important because other program is opening and writing to this file)
			if (file_personlocation.is_open())
			{
				flag2 = 1;
				//printf("File opened \n");
			}
			else {
				OutputDebugStringW(L"Failed to open file.\n");
				Sleep(filefetch_wait);
			}
		} while (flag2 == 0);
		file_personlocation << string_of_coordinates_std;
		file_personlocation.close();
		collectdata = false;
		string_of_coordinates = "";

		/*string_of_coordinates = "Hi";


		FileSavePicker^ savePicker = ref new FileSavePicker();
		savePicker->SuggestedStartLocation = PickerLocationId::DocumentsLibrary;

		auto plainTextExtensions = ref new Platform::Collections::Vector<String^>();
		plainTextExtensions->Append(".txt");
		savePicker->FileTypeChoices->Insert("Plain Text", plainTextExtensions);
		savePicker->SuggestedFileName = "New Document";
		savePicker->PickSaveFileAsync();
		StorageFile^ file;
		file->GetFileFromPathAsync(docPath_touch);
		OutputDebugString(("Name: " + file->Name + " Path: " + file->Path + "\n")->Data());
		FileIO::WriteTextAsync(file, file->Name);*/

		//create_task(savePicker->PickSaveFileAsync()).then((StorageFile^ file) {

		//	if (file != nullptr)
		//	{
		//		// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
		//		CachedFileManager::DeferUpdates(file);
		//		// write to file
		//		create_task(FileIO::WriteTextAsync(file, file->Name)).then([this, file]()
		//			{
		//			});

		//	}
		//});


		//// Get the app's installation folder
		//StorageFolder^ appFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
		////appFolder
		////appFolder->Path = "C:\Users\brandon\Desktop\Tanvas\DARPA_touchdetection";
		////appFolder->GetFolderFromPathAsync("C:\Users\brandon\Desktop\Tanvas\DARPA_touchdetection");
		////appFolder->GetFolderAsync("C:\Users\brandon\Desktop\Tanvas\DARPA_touchdetection");

		//OutputDebugString(("Folder: " + appFolder->Path + "\n")->Data());

		//// Get the app's manifest file from the current folder
		//String^ name = "touch_locations.txt";
		//appFolder->GetFileAsync(name);
		//FileIO::WriteTextAsync(appFolder, "i")

		//create_task(appFolder->GetFileAsync(name)).then([=](StorageFile^ manifest) {
		//	FileIO::WriteTextAsync(manifest, string_of_coordinates);
		//	//Do something with the manifest file  
		//	});


	//	// Get the path to the app's installation folder.
	//	String^ root = Windows::ApplicationModel::Package::Current->InstalledLocation->Path;

	//	// Get the folder object that corresponds to
	//	// this absolute path in the file system.
	//	create_task(StorageFolder::GetFolderFromPathAsync(root)).then([=](StorageFolder^ folder) {
	//		});



	//	StorageFolder^ folder = StorageFolder::GetFolderFromPathAsync(docPath);
	//		
	//		StorageFolder::GetFolderFromPathAsync(docPath_touch);

	//	StorageFile file;

	//	FileOpenPicker fileOpenPicker = new FileOpenPicker();

	//	await FileIO.WriteTextAsync(targetFile, "This content will be written to the file");

	//	Text = await FileIO.ReadTextAsync(file);
	//	FileName = file.Name;

	//	StorageFile file;
	//	file = StorageFile::GetFileAsync(docPath_touch);

	//	StorageFile^ file;
	//	file->Path = docPath_touch;
	//	
	//	 StorageFile::GetFileFromPathAsync(docPath_touch);
	//		//StorageFolder::GetFolderFromPathAsync(docPath_touch);
	//	//StorageFolder.GetF .GetFolderFromPathAsync(docPath_touch);
	///*	if (file != nullptr)
	//	{*/
	//		String^ userContent = string_of_coordinates;
	//		create_task(FileIO::WriteTextAsync(file, userContent))
				//{
				//	try
				//	{
				//		task.get();
				//		rootPage->NotifyUser("The following text was written to '" + file->Name + "':\n" + userContent, NotifyType::StatusMessage);
				//	}
				//	catch (COMException^ ex)
				//	{
				//		// I/O errors are reported as exceptions.
				//		rootPage->HandleIoException(ex, "Error writing to '" + file->Name + "'");
				//	}
				//});
		//}
		//else
		//{
		//	rootPage->NotifyUserFileNotExist();
		//}

	}
	else {
		OutputDebugStringW(L"Double Tap Detected - Data collection starting now.\n");
		collectdata = true;
		//doubletapped = true;
	}
	//collectdata = true;
	// Retrieve the point associated with the current event
	//Windows::UI::Input::PointerPoint^ currentPoint = e->GetCurrentPoint(mainCanvas);

}

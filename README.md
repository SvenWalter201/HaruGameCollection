# HaruGameCollection


## 1. Setup:

Please download the latest Azure Kinect odytracking SDK and latest Azure Kinect SDK
[Kinect SDK](https://docs.microsoft.com/de-de/azure/kinect-dk/sensor-sdk-download)
[Kinect Bodytracking SDK](https://docs.microsoft.com/de-de/azure/kinect-dk/body-sdk-download)

Please open up the project. (Preferably in Unity 2020.3.15f1)
There will be errors so if Unity asks if you wish to enter in safe mode just ignore the message

In Unity open up the Visual Studio Solution by clicking on one of the scripts in Assets/Scripts (you might need to set Visual Studio as your code editor in Preferences/External Tools)

In Visual Studio go to Tools/NuGet Package Manager/Package Manager Console. On the command line of the console at type the following command:
> Update-Package -reinstall

Then close up the project again.

Now please download the zip file from this link and extract. 

[UnityDeps](https://drive.google.com/file/d/1f2DMU0VHB9wQqWHNaQAsnG4EkQMC0gGZ/view?usp=sharing)

Alternatively you can go to the following link and perform steps three and four.

[Unity Bodytracking](https://github.com/microsoft/Azure-Kinect-Samples/blob/master/body-tracking-samples/sample_unity_bodytracking/README.md)

The zip contains three folders. Please drag the dlls in the folders to the following locations:
- PluginFolder: Move dlls to Assets/Plugins
- RootProjectFolder: Move dlls to the root folder of the project (the folder containing the Assets Folder)

Now you should be able to open the project and use the Kinect Azure. A quick way to test this is opening the Scene KinectMoCapStudio
Press Play and click on the buttons for Image Tracking and Body Tracking. 


If you want to make a standalone Build you need to move the dlls from the RootExeFolder to the root folder of the build (The folder containing the .exe file) 


## Working on the project

A detailed explanation of how to navigate and add more content to the project can be found in `HOWTO.md` file located at the project root.
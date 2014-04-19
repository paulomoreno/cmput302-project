CMPUT 302 Project - Kinect Fitness
================

1. Prerequisites
    - Java 7
    - Windows 7 or 8
    - Webcam and microphone
    - Samsung Galaxy S4 phone with the Sensor Read android application
    - Netbeans 7.3 or 7.4
    - Kinect SDK
    - Xbox 360 Kinect
    - Xbox 360 Controller
2. Installation steps to run the application
    1. Install Java 7 at https://www.java.com/en/download/
    2. Install Netbeans 7.4 at https://netbeans.org/downloads/7.4/
    3. Install the Kinect SDK at http://www.microsoft.com/en-us/kinectforwindowsdev/Downloads.aspx
    4. Download the KinectFitness application at
    5. Import the zip in the Netbeans
    6. Resolve all VLCJ jar file issues by clicking "Resolve problems" upon trigger then browsing to where the specified jar files are.
    7. "Clean and build" the project and run.
3. How to use the application 
  1. Doctor's side of the Application
      1. Enter "doctor" in the input field then press "Enter".
      2. Wait for the patient to connect. (Must inform the patient of your IP address)
      3. Press Mute/unmute button if one wants to mute/unmute the specified patient.
      4. Press the expand/collapse button if one wants to expand one patient's screen to full screen or collapse it back to multiple screen UI.
  2. Patient's side of the Application
      1. Connect the Xbox 360 Kinect and the controller to the laptop/desktop.
      2. Change the IP in the FitnessMain.java class at line 132 to the IP address of the doctor and save the file.
      3. Open the Command Prompt as an administrator then open a hosted network.
          1. Enter "netsh wlan set hostednetwork mode=allow ssid=kinectfitness key=kinect14". (just for the first time)
          2. Enter "netsh wlan start hostednetwork".
          3. When the session is done the enter "netsh wlan stop hostednetwork".
          4. Enter "ipconfig" and enter the IP address of the hosted network to the Sensor Read Application.
      4. Connect the phone's wifi to the hosted network created above.
      5. Run the Java application in the netbeans and enter "patient1" (or patient2,...,or patient8) in the input field then press "Enter".
      6. Press "Wifi connect" button on the Sensor Read application.
      7. Wear the heart rate monitor then press "Connect" button on Sensor Read application for the heart rate then press "Start".
      8. Clip on the oximeter on the finger tip then press "Connect" button on Sensor Read application for the oximeter then press "Start".
      9. Connect the Xbox controller and use the D-pad, A and B button to navigate through the Kinect Application UI.

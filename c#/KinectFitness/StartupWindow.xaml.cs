using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Diagnostics;
using System.Threading;

namespace KinectFitness
{
    public partial class StartupWindow : Window
    {
        //Global Variables for Kinect
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        Skeleton first;
        //AudioCommands myCommands;

        //Buttons
        Rect playButton;
        Rect recordButton;
        Rect quitButton;
        Rect optionsButton;
        Rect rightHandPos;

        //Hover Checker Timer
        Stopwatch hoverTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        //Controller variables
        //private Controller control;
        //private Thread newThread;
        //private AudioCommands myCommands;
        int buttons;


        public StartupWindow()
        {
            //control = new Controller();
           
            InitializeComponent();
            InitializeUI();
            //InitializeAudioCommands();
            /*
            if (control.isConnected() == true)
            {
                Console.WriteLine("control null");
                InitializeHoverChecker(0);
                int result = 10;

                buttons = 0; // 0 == play; 1 == options; 2 == record; 3 == quit 
                playborder.Opacity = 1;

                newThread = new Thread(() =>
                {
                    control.updateStates();
                    while (true && control != null)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {

                            result = control.getPOV();
                            updateHighlights(result);
                            checkButtonPressed(buttons, control.getButton(0), control.getButton(1));
                            // Console.WriteLine(control.getButton());

                        }));

                    }

                });

                newThread.Start(); 
            }
            else */InitializeHoverChecker(1);
            
           

            var navWindow = Window.GetWindow(this) as NavigationWindow;
            if (navWindow != null) navWindow.ShowsNavigationUI = false;
            this.WindowState = System.Windows.WindowState.Maximized;

            
        }

        // 0 == play; 1 == options; 2 == record; 3 == quit 
        private void updateHighlights(int result) {
           //button play
            if (buttons == 0) {
                if (result == 0) {
                    playborder.Opacity = 0;
                    optionsborder.Opacity = 1;
                    buttons = 1;
                }
                else if (result == 9000) 
                {
                    playborder.Opacity = 0;
                    quitborder.Opacity = 1;
                    buttons = 3;
                }
            }

            //button options
            if (buttons == 1)
            {
                if (result == 18000)
                {
                    playborder.Opacity = 1;
                    optionsborder.Opacity = 0;
                    buttons = 0;
                }
                else if (result == 9000)
                {
                    optionsborder.Opacity = 0;
                    recordborder.Opacity = 1;
                    buttons = 2;
                }
            }

            //button record
            if (buttons == 2)
            {
                if (result == 27000)
                {
                    recordborder.Opacity = 0;
                    optionsborder.Opacity = 1;
                    buttons = 1;
                }
                else if (result == 18000)
                {
                    recordborder.Opacity = 0;
                    quitborder.Opacity = 1;
                    buttons = 3;
                }
            }

            //button quit
            if (buttons == 3)
            {
                if (result == 0)
                {
                    quitborder.Opacity = 0;
                    recordborder.Opacity = 1;
                    buttons = 2;
                }
                else if (result == 27000)
                {
                    quitborder.Opacity = 0;
                    playborder.Opacity = 1;
                    buttons = 0;
                }
            }     
        }

        private void checkButtonPressed(int button_number, bool button, bool button_2) 
        {
            if (button_2 == true) { 
            
            }
            //Console.WriteLine("Hey, I'm here!");
            if(button == true){
                //Console.WriteLine("HEy, I'm here as well!: " + button_number);
                switch (button_number)
                { 
                    
                    case 0:
                        Button_Play(new object(), new RoutedEventArgs());
                        break;

                    case 1:
                        Button_Options(new object(), new RoutedEventArgs());
                        break;

                    case 2:
                        Button_Record(new object(), new RoutedEventArgs());
                        break;
                    case 3:
                        QuitApplication(new object(), new RoutedEventArgs());
                        break;

                    default:
                        break;
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        /*
        private void InitializeAudioCommands()
        {
            myCommands = new AudioCommands(0.82, "quit", "play", "record");//instantiate an AudioCommands object with the possible commands
            myCommands.setFunction("play", Button_Play);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function
            myCommands.setFunction("record", Button_Record);
            myCommands.setFunction("quit", QuitApplication);
        }*/

        private void InitializeUI()
        {
            rightHandProgressBar.Width = 0;

            //Get positions of buttons and hands
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
            playButton = new Rect();
            playButton.Location = new Point(Canvas.GetLeft(playButtonImg), Canvas.GetTop(playButtonImg));
            playButton.Size = new Size(playButtonImg.Width, playButtonImg.Height);
            recordButton = new Rect();
            recordButton.Location = new Point(Canvas.GetLeft(recordButtonImg), Canvas.GetTop(recordButtonImg));
            recordButton.Size = new Size(recordButtonImg.Width, recordButtonImg.Height);
            quitButton = new Rect();
            quitButton.Location = new Point(Canvas.GetLeft(quitButtonImg), Canvas.GetTop(quitButtonImg));
            quitButton.Size = new Size(quitButtonImg.Width, quitButtonImg.Height);
            optionsButton = new Rect();
            optionsButton.Location = new Point(Canvas.GetLeft(optionsButtonImg), Canvas.GetTop(optionsButtonImg));
            optionsButton.Size = new Size(optionsButtonImg.Width, optionsButtonImg.Height);
        }

        void InitializeHoverChecker(int control)
        {

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions

            if (control == 1)
            {
                dispatcherTimer.Tick += new EventHandler(checkHands);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            }
          
          
            dispatcherTimer.Start();

            hoverTimer = new Stopwatch();
        }

        /**
       * Checks to see if hands are hovering over a button
       */
        private void checkHands(object sender, EventArgs e)
        {
            try
            {

                if (rightHandPos.IntersectsWith(playButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(playButtonImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);


                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        Button_Play(new object(), new RoutedEventArgs());
                        hoverTimer.Reset();
                    }
                }
                else if (rightHandPos.IntersectsWith(recordButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(recordButtonImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);


                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        Button_Record(new object(), new RoutedEventArgs());

                        //this.NavigationService.Navigate(kw);
                        this.Content = null;
                        hoverTimer.Reset();
                    }
                }
                else if (rightHandPos.IntersectsWith(quitButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(quitButtonImg, new RoutedEventArgs());
                    //Set progress bar to increase on hands to indicate if hand is hovering on button
                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);


                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        QuitApplication(new object(), new RoutedEventArgs());
                    }
                }
                else if (rightHandPos.IntersectsWith(optionsButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(optionsButtonImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button
                    setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);


                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        hoverTimer.Reset();
                    }
                }
                else  //If hand is not hovering on any button.  Reset timer.
                {
                    resetHandProgressBars();
                    hoverTimer.Reset();
                    //Unhighlight all images
                    leaveImage(playButtonImg, new RoutedEventArgs());
                    leaveImage(optionsButtonImg, new RoutedEventArgs());
                    leaveImage(quitButtonImg, new RoutedEventArgs());
                    leaveImage(recordButtonImg, new RoutedEventArgs());

                }
            }
            catch (NullReferenceException)
            {
                //Do Nothing
            }
        }

        private void QuitApplication(object sender, RoutedEventArgs e)
        {
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            //myCommands.StopSpeechRecognition();
            this.Close();

            try
            {
                //newThread.Abort();
                //control.ReleaseDevice();
            }
            catch (Exception ex) { }

            Process.GetCurrentProcess().Kill();
        }
        private void Button_Play(object sender, RoutedEventArgs e)
        {
            closing = true; 
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            //myCommands.StopSpeechRecognition();
            SelectLevelWindow slw = new SelectLevelWindow();
            //newThread.Abort();
            //control.ReleaseDevice();
            this.Close();
            slw.Show();
            //this.NavigationService.Navigate(slw);            
        }

        private void Button_Record(object sender, RoutedEventArgs e)
        {
            closing = true; 
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            //myCommands.StopSpeechRecognition();
            RecordWindow rw = new RecordWindow();
            this.Close();
            rw.Show();
            //this.NavigationService.Navigate(rw);
        }

        private void Button_Options(object sender, RoutedEventArgs e)
        {

        }

        /**
         * Highlights images when the mouse hovers over them
         */
        private void hoverImage(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playButtonImg.Name))
            {
                playborder.Opacity = 1;
            }
            else if (i.Name.Equals(optionsButtonImg.Name))
            {
                optionsborder.Opacity = 1;
            }
            else if (i.Name.Equals(quitButtonImg.Name))
            {
                quitborder.Opacity = 1;
            }
            else if (i.Name.Equals(recordButtonImg.Name))
            {
                recordborder.Opacity = 1;
            }
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leaveImage(object sender, RoutedEventArgs e)
        {
                Image i = (Image)sender;   
                if(i.Name.Equals(playButtonImg.Name))
                {
                    playborder.Opacity = 0;
                }
                else if (i.Name.Equals(optionsButtonImg.Name))
                {
                    optionsborder.Opacity = 0;
                }
                else if (i.Name.Equals(quitButtonImg.Name))
                {
                    quitborder.Opacity = 0;
                }
                else if (i.Name.Equals(recordButtonImg.Name))
                {
                    recordborder.Opacity = 0;
                }

        }

        private void setHandProgressBar(bool leftHand, long timeElapsed)
        {
            double t = timeElapsed;
            double w = t / 2000 * 50;

            rightHandProgressBar.Width = w;

        }

        /**
         * Resets the progress bar on the hands
         */
        private void resetHandProgressBars()
        {
            rightHandProgressBar.Width = 0;
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            KinectSensor sensor = (KinectSensor)e.NewValue;

            if (sensor == null)
            {
                return;
            }

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.7f,
                Correction = 0.3f,
                Prediction = 1.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 1.0f
            };
            sensor.SkeletonStream.Enable(parameters);

            sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            try
            {
                sensor.Start();
                ksensor = sensor;
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            if (closing)
            {
                return;
            }

            //Get a skeleton
            first = GetFirstSkeleton(e);

            if (first == null)
            {
                return;
            }

            GetCameraPoint(first, e);
            //set scaled position

            ScalePosition(rightHand, first.Joints[JointType.HandRight]);
            ScalePosition(rightHandProgressBar, first.Joints[JointType.HandRight]);
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null || !kinectSensorChooser1.Kinect.IsRunning)
                {
                    return;
                }

                //Map a joint location to a point on the depth map
                //left hand
                DepthImagePoint leftDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);



                //Map a depth point to a point on the color image
                //left hand
                ColorImagePoint leftColorPoint =
                    depth.MapToColorImagePoint(leftDepthPoint.X, leftDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightColorPoint =
                    depth.MapToColorImagePoint(rightDepthPoint.X, rightDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);


                //Set location
                CameraPosition(rightHand, rightColorPoint);
                CameraPosition(rightHandProgressBar, rightColorPoint);
            }
        }


        Skeleton GetFirstSkeleton(AllFramesReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }


                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //get the first tracked skeleton
                Skeleton first = (from s in allSkeletons
                                  where s.TrackingState == SkeletonTrackingState.Tracked
                                  select s).FirstOrDefault();

                return first;

            }
        }

        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
            }
        }

        private void CameraPosition(FrameworkElement element, ColorImagePoint point)
        {
            //Divide by 2 for width and height so point is right in the middle 
            // instead of in top/left corner
            Canvas.SetLeft(element, point.X - element.Width / 2);
            Canvas.SetTop(element, point.Y - element.Height / 2);
        }

        private void ScalePosition(FrameworkElement element, Joint joint)
        {
            Joint scaledJoint = joint.ScaleTo(900, 800, .3f, .3f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

        }

        private void kinectSkeletonViewer1_Unloaded(object sender, RoutedEventArgs e)
        {
            closing = true;
            StopKinect(kinectSensorChooser1.Kinect);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try { 
                //newThread.Abort(); 
                //control.ReleaseDevice(); 
            } 
            catch (Exception ex) { }
            
            //AudioCommands.StopSpeechRecognition(myCommands);
            //myCommands = null;
        }


    }
}

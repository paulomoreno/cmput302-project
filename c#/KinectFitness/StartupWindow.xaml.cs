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

        //Start Up Buttons
        Rect playButton;
        Rect recordButton;
        Rect quitButton;
        Rect optionsButton;
        Rect rightHandPos;

        //Select Level Buttons
        Rect warmUp;
        Rect moderateCardio;
        Rect intenseCardio;
        Rect backButton;

        //Hover Checker Timer
        Stopwatch hoverTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        System.Windows.Threading.DispatcherTimer animator;

        //Controller variables
        //private Controller control;
        //private Thread newThread;
        //private AudioCommands myCommands;
        int buttons;

        //List of buttons that user can press
        List<Rect> buttonsList = new List<Rect>();
        //List of corresponding actions that each button performs
        List<Action<object, RoutedEventArgs>> actionsList = new List<Action<object, RoutedEventArgs>>();

        public StartupWindow()
        {           
            InitializeComponent();
            InitializeUI();
            InitializeStartUpUI();            
            InitializeHoverChecker(1);          

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
                        Button_Quit(new object(), new RoutedEventArgs());
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
            loadBackground();


            //Get position of hand
            rightHandPos = new Rect();
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
        }        

        /**
         * Initializer for StartUp UI
         */
        private void InitializeStartUpUI()
        {
            rightHandProgressBar.Width = 0;

            //Get positions of buttons
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

            
            buttonsList.Add(playButton);
            buttonsList.Add(recordButton);
            buttonsList.Add(quitButton);
            buttonsList.Add(optionsButton);

            actionsList.Add(Button_Play);
            actionsList.Add(Button_Record);
            actionsList.Add(Button_Quit);
            actionsList.Add(Button_Options);

            StartUp.Margin = new Thickness(0, 0, 0, 0);
            SelectLevel.Margin = new Thickness(1300, 0, 0, 0);
        }

        private void deInitializeStartUpUI()
        {
            buttonsList.Clear();
            actionsList.Clear();
        }

        private void initializeSelectLevelUI()
        {        
            rightHandProgressBar.Width = 0;

            //Get positions of buttons
            warmUp = new Rect();            
            warmUp.Location = new Point(Canvas.GetLeft(warmUpImg), Canvas.GetTop(warmUpImg));
            warmUp.Size = new Size(warmUpImg.Width, warmUpImg.Height);
            moderateCardio = new Rect();
            moderateCardio.Location = new Point(Canvas.GetLeft(moderateImg), Canvas.GetTop(moderateImg));
            moderateCardio.Size = new Size(moderateImg.Width, moderateImg.Height);
            intenseCardio = new Rect();
            intenseCardio.Location = new Point(Canvas.GetLeft(intenseImg), Canvas.GetTop(intenseImg));
            intenseCardio.Size = new Size(intenseImg.Width, intenseImg.Height);
            backButton = new Rect();
            backButton.Location = new Point(Canvas.GetLeft(backButtonImg), Canvas.GetTop(backButtonImg));
            backButton.Size = new Size(backButtonImg.Width, backButtonImg.Height);

            buttonsList.Add(warmUp);
            buttonsList.Add(moderateCardio);
            buttonsList.Add(intenseCardio);
            buttonsList.Add(backButton);
            
            actionsList.Add(warmUpWorkout);
            actionsList.Add(moderateWorkout);
            actionsList.Add(intenseWorkout);
            actionsList.Add(backButtonPressed);
        }

        private void deInitializeSelectLevelUI()
        {
            buttonsList.Clear();
            actionsList.Clear();
        }

        private void loadBackground()
        {
            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            background.Source = new Uri(path + "\\KinectFitness\\background.mp4");
            background.Play();
        }

        /**
        * Take the old screen and hide it
        * Take the new screen and show it
        */
        private void startNewCanvas(Canvas newScreen, Canvas oldScreen, bool goBack)
        {
            //If not going back a screen
            //then do the forward animation
            if (goBack == false)
            {
                animator = new System.Windows.Threading.DispatcherTimer();
                animator.Tick += (s, args) => animateShowNewCanvas(newScreen, oldScreen);
                animator.Interval = new TimeSpan(0, 0, 0, 0, 10);
                animator.Start();
            }
            //Else do the backward animation
            else
            {
                animator = new System.Windows.Threading.DispatcherTimer();
                animator.Tick += (s, args) => animateShowNewCanvasBack(newScreen, oldScreen);
                animator.Interval = new TimeSpan(0, 0, 0, 0, 10);
                animator.Start();
            }
        }

        private void animateShowNewCanvas(Canvas newScreen, Canvas oldScreen)
        {
            if (oldScreen.Margin.Left > -1300)
            {
                oldScreen.Margin = new Thickness(oldScreen.Margin.Left - 60, 0, 0, 0);
                newScreen.Margin = new Thickness(newScreen.Margin.Left - 60, 0, 0, 0);
            }
            else
            {
                animator.Stop();
            }
        }

        private void animateShowNewCanvasBack(Canvas newScreen, Canvas oldScreen)
        {
            if (oldScreen.Margin.Left < 1300)
            {
                oldScreen.Margin = new Thickness(oldScreen.Margin.Left + 60, 0, 0, 0);
                newScreen.Margin = new Thickness(newScreen.Margin.Left + 60, 0, 0, 0);
            }
            else
            {
                animator.Stop();
            }
        }

        /**
         * Loop the background
         */
        private void Media_Ended(object sender, EventArgs e)
        {
            background.Position = new TimeSpan(0, 0, 0);
            background.Play();
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
                //Boolean to check if a button is being hovered
                bool noButtonsBeingHovered = true;
                for (int i = 0; i < buttonsList.Count(); i++)
                {
                    if (rightHandPos.IntersectsWith(buttonsList.ElementAt(i)))
                    {
                        hoverTimer.Start();
                        //Set to false
                        noButtonsBeingHovered = false;

                        hoverImage(buttonsList.ElementAt(i), new RoutedEventArgs());

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);

                        if (hoverTimer.ElapsedMilliseconds >= 2000)
                        {
                            //Call the appropriate function for the button being pressed
                            buttonPressed(buttonsList.ElementAt(i), new RoutedEventArgs());
                            hoverTimer.Reset();
                        }
                    }
                }
                if (noButtonsBeingHovered)
                {
                    resetHandProgressBars();
                    hoverTimer.Reset();
                    resetImages();
                }
            }
            catch (NullReferenceException)
            {
                //Do Nothing
            }
        }

        private void buttonPressed(object sender, RoutedEventArgs e)
        {
            Rect r = (Rect)sender;
            for (int i = 0; i < buttonsList.Count(); i++)
            {
                if (r.Equals(buttonsList.ElementAt(i)))
                {
                    actionsList.ElementAt(i)(new object(), new RoutedEventArgs());
                }
            }
        }

        private void Button_Quit(object sender, RoutedEventArgs e)
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
            startNewCanvas(SelectLevel, StartUp, false);
            deInitializeStartUpUI();
            initializeSelectLevelUI();
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
            Rect r = (Rect)sender;


            if (r.Equals(playButton))
            {
                playborder.Opacity = 1;
            }
            else if (r.Equals(optionsButton))
            {
                optionsborder.Opacity = 1;
            }
            else if (r.Equals(quitButton))
            {
                quitborder.Opacity = 1;
            }
            else if (r.Equals(recordButton))
            {
                recordborder.Opacity = 1;
            }
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void resetImages()
        {
                    playborder.Opacity = 0;
                    optionsborder.Opacity = 0;
                    quitborder.Opacity = 0;
                    recordborder.Opacity = 0;


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
            rightHandProgressBar.Margin = new Thickness(rightHandProgressBar.Margin.Left, rightHandProgressBar.Margin.Top + 40, 0, 0);
            rightHandPos.Location = new Point(rightHand.Margin.Left, rightHand.Margin.Top);
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
            element.Margin = new Thickness(scaledJoint.Position.X, scaledJoint.Position.Y, 0, 0);

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

        private void SetFile(Image exercise)
        {
            List<string> video = new List<string>();

            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            if (exercise.Name == warmUpImg.Name)
            {
                video.Add(warmUpImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == moderateImg.Name)
            {
                video.Add(moderateImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == intenseImg.Name)
            {
                video.Add(intenseImg.Name);
                System.IO.File.WriteAllLines(path + "\\KinectFitness\\FitnessVideos\\video.txt", video);
            }
        }

        private void intenseWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(intenseImg);
        }

        private void moderateWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(moderateImg);
        }

        private void warmUpWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(warmUpImg);
        }

        private void backButtonPressed(object sender, RoutedEventArgs e)
        {
            startNewCanvas(StartUp, SelectLevel, true);
            deInitializeSelectLevelUI();
            InitializeStartUpUI();
        }


    }
}

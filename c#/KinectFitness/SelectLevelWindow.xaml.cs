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
    /// <summary>
    /// Interaction logic for SelectLevelWindow.xaml
    /// </summary>
    public partial class SelectLevelWindow : Window
    {
        bool closing = false;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        Skeleton first;
        Stopwatch hoverTimer;
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        //AudioCommands myCommands;

        //Hand Positions
        Rect rightHandPos;

        //Button Positions
        Rect warmUp;
        Rect moderateCardio;
        Rect intenseCardio;
        Rect backButton;

        //Controller variables
        private Controller control;
        private Thread newThread;

        int buttons;
        //Thread patientDataThread;

        //private AudioCommands myCommands;

        public SelectLevelWindow()
        {

            control = new Controller();

            InitializeComponent();
            InitializeUI();
            //InitializeAudioCommands();

            if (control.isConnected() == true)
            {
                Console.WriteLine("control null");
                InitializeHoverChecker(0);
                buttons = 0; // 0 == play; 1 == options; 2 == record; 3 == quit 
                int result = 10;

                warmUpImgBorder.Opacity = 1;

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
            else InitializeHoverChecker(1);

            this.WindowState = System.Windows.WindowState.Maximized;

            PatientTcpServer.getHeartRate();

            //addExercises();
        }

        private void checkButtonPressed(int buttons, bool button, bool button_2)
        {             //Console.WriteLine("Hey, I'm here!");
            if (button_2 == true) 
            {
                backButtonPressed(new object(), new RoutedEventArgs());
            }
            
            if (button == true)
            {
                //Console.WriteLine("HEy, I'm here as well!: " + button_number);
                switch (buttons)
                {
                    case 0:  
                        warmUpWorkout(new object(), new RoutedEventArgs());
                        break;

                    case 1:
                        moderateWorkout(new object(), new RoutedEventArgs());
                        break;

                    case 2:
                        intenseWorkout(new object(), new RoutedEventArgs());
                        break;

                    default:
                        break;
                }
            }
        }

        //0 == warm up, 1 == moderate, 2 == last one
        private void updateHighlights(int result) 
        {
            if (buttons == 0) 
            {
                if (result == 9000) {
                    warmUpImgBorder.Opacity = 0;
                    moderateImgBorder.Opacity = 1;
                    buttons = 1;
                    Thread.Sleep(250);
                }
            }
            else if (buttons == 1) {
                if (result == 9000) {
                    moderateImgBorder.Opacity = 0;
                    intenseImgBorder.Opacity = 1;
                    buttons = 2;
                    Thread.Sleep(250);
                }
                else if (result == 27000) 
                {
                    moderateImgBorder.Opacity = 0;
                    warmUpImgBorder.Opacity = 1;
                    buttons = 0;
                    Thread.Sleep(250);
                }
            }
            else if (buttons == 2)
            {
                if (result == 27000) {
                    intenseImgBorder.Opacity = 0;
                    moderateImgBorder.Opacity = 1;
                    buttons = 1;
                    Thread.Sleep(250);
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
            myCommands = new AudioCommands(0.82, "warmUp", "moderate", "intense", "back");//instantiate an AudioCommands object with the possible commands
            myCommands.setFunction("warmUp", warmUpWorkout);//tell AudioCommands what to do when the speech "play" is recognized. The second parameter is a function
            myCommands.setFunction("moderate", moderateWorkout);
            myCommands.setFunction("intense", intenseWorkout);
            myCommands.setFunction("back", backButtonPressed);
        }*/

        private void InitializeUI()
        {
            rightHandProgressBar.Width = 0;


            //Get positions of buttons and hands
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
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

                if (rightHandPos.IntersectsWith(warmUp))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        warmUpWorkout(new object(), new RoutedEventArgs());
                    }
                }
                else if (rightHandPos.IntersectsWith(moderateCardio))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(moderateImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());

                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        moderateWorkout(new object(), new RoutedEventArgs());
                        
                        //this.NavigationService.Navigate(kw);
                        this.Content = null;
                        hoverTimer.Reset();                        
                    }
                }
                else if (rightHandPos.IntersectsWith(intenseCardio))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(intenseImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());


                    //Set progress bar to increase on hands to indicate if hand is hovering on button
                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                   

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        //Tells the Kinect Window which file to load
                        intenseWorkout(new object(), new RoutedEventArgs());

                    }
                }
                else if (rightHandPos.IntersectsWith(backButton))
                {
                    hoverTimer.Start();
                    //Highlight the correct image and unhighlight the others
                    hoverImage(backButtonImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());


                    //Set progress bar to increase on hands to indicate if hand is hovering on button

                        setHandProgressBar(false, hoverTimer.ElapsedMilliseconds);
                    

                    //Check if hand has been hovering on target for 2 seconds or more   
                    if (hoverTimer.ElapsedMilliseconds >= 2000)
                    {
                        StopKinect(kinectSensorChooser1.Kinect);
                        dispatcherTimer.Stop();
                        StartupWindow sw = new StartupWindow();                        
                        sw.Show();
                        this.Close();
                        hoverTimer.Reset();
                    }
                }
                else  //If hand is not hovering on any button.  Reset timer.
                {
                    resetHandProgressBars();
                    hoverTimer.Reset();
                    //Unhighlight all images
                    leaveImage(warmUpImg, new RoutedEventArgs());
                    leaveImage(moderateImg, new RoutedEventArgs());
                    leaveImage(intenseImg, new RoutedEventArgs());
                    leaveImage(backButtonImg, new RoutedEventArgs());

                }
            }
            catch (NullReferenceException)
            {
                //Do Nothing
            }
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
                System.IO.File.WriteAllLines(path + "\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == moderateImg.Name)
            {
                video.Add(moderateImg.Name);
                System.IO.File.WriteAllLines(path + "\\FitnessVideos\\video.txt", video);
            }
            else if (exercise.Name == intenseImg.Name)
            {
                video.Add(intenseImg.Name);
                System.IO.File.WriteAllLines(path + "\\FitnessVideos\\video.txt", video);
            }
        }


        /**
        * Highlights play when the mouse or hand hovers over them
        */
        private void hoverImage(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(warmUpImg.Name))
            {
                warmUpImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(moderateImg.Name))
            {
                moderateImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(intenseImg.Name))
            {
                intenseImgBorder.Opacity = 1;
            }
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonImg.Opacity = 0;
                backButtonHoverImg.Opacity = 1;
            }

        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leaveImage(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(warmUpImg.Name))
            {
                warmUpImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(moderateImg.Name))
            {
                moderateImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(intenseImg.Name))
            {
                intenseImgBorder.Opacity = 0;
            }
            else if (i.Name.Equals(backButtonImg.Name))
            {
                backButtonHoverImg.Opacity = 0;
                backButtonImg.Opacity = 1;
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
            //sensor.SkeletonStream.Enable(parameters);

            sensor.SkeletonStream.Enable();

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
                    kinectSensorChooser1.Kinect == null  || !kinectSensorChooser1.Kinect.IsRunning)
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

        private void closeWindow()
        {
            StopKinect(kinectSensorChooser1.Kinect);
            dispatcherTimer.Stop();
            //myCommands.StopSpeechRecognition();
            hoverTimer.Reset();
            
            /*
            if(patientDataThread.IsAlive)
            {
                patientDataThread.Abort();
            }
            /**/
           
            this.Close();
        }

        private void intenseWorkout(object sender, RoutedEventArgs e)
        {
            SetFile(intenseImg);            
            KinectWindow kw = new KinectWindow();            
            kw.Show();
            closeWindow();
        }

        private void moderateWorkout(object sender,RoutedEventArgs e)
        {
            SetFile(moderateImg);
            
            KinectWindow kw = new KinectWindow();
            kw.Show();
            closeWindow();
        }

        private void warmUpWorkout(object sender,RoutedEventArgs e)
        {



            Kinect2JavaClient data = new Kinect2JavaClient("Hello world");
            data.sendFlag();



    
            SetFile(warmUpImg);
            KinectWindow kw = new KinectWindow(); // What\s gpomg pm jere
            kw.Show();
            closeWindow();
        }

        private void backButtonPressed(object sender, RoutedEventArgs e)
        {
            StartupWindow sw = new StartupWindow();
            sw.Show();
            closeWindow();           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                newThread.Abort();
                control.ReleaseDevice();

                //patientDataThread.Abort();
            }
            catch (Exception ex) { }
           // AudioCommands.StopSpeechRecognition(myCommands);
            //myCommands = null;
        }
    }
}

﻿using System;
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
using Kinect.Toolbox;
using Kinect.Toolbox.Record;
using Microsoft.Win32;
using System.IO;

namespace KinectFitness
{
    /// <summary>
    /// Interaction logic for KinectWindow.xaml
    /// </summary>
    /// 

    

    public partial class KinectWindow : Page
    {
        public KinectWindow()
        {
            InitializeComponent();
            
        }
        
        bool closing = false;
        bool videoPlaying;
        bool timerInitialized;
        const int skeletonCount = 6;
        Skeleton[] allSkeletons = new Skeleton[skeletonCount];
        KinectSensor ksensor;
        double numberOfPts;
        Stopwatch stopwatch;
        Stopwatch hoverTimer;
        Random r;
        Skeleton first;
        List<string> jointAngles;
        List<JointAngles> loadedSkeleton;
        System.Windows.Threading.DispatcherTimer skeletonMatcherTimer;

        //Hand Positions
        Rect leftHandPos;
        Rect rightHandPos;

        //Button Positions
        Rect playIconPos;

        KinectRecorder recorder;
        KinectReplay replay;

        Stopwatch comparisonStopWatch;

        readonly ColorStreamManager colorManager = new ColorStreamManager();
        SkeletonDisplayManager skeletonDisplayManager;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
            initializeUI();
            initializeHoverChecker();
            loadExercise();
        }

        private void loadExercise()
        {
            string line;
            JointAngles ja = new JointAngles();
            loadedSkeleton = new List<JointAngles>();
            // Read the file and display it line by line.
            String path = System.AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;
            path = System.IO.Directory.GetParent(path).FullName;

            System.IO.StreamReader file =
               new System.IO.StreamReader(path + "\\Recordings\\chosenExercise.txt");
            while ((line = file.ReadLine()) != null)
            {
                /*Add in time to check*/
                /* if (line.Contains("Time:"))
                 {
                     foreach (var line in File.ReadAllLines("file").Where(line => line.StartsWith("Time:")))
                 * {
                 * int value = 0;
                 * if(int.TryParse(line.Replace("Time:", "").Trim(), out value))
                 * {
                 * foreach (var 
                 * }
                 * }
                 }*/


                if(line.Contains("Left Shoulder"))
                {
                    ja.leftShoulder = Convert.ToInt32(file.ReadLine());          
                }
                else if (line.Contains("Right Shoulder"))
                {
                    ja.rightShoulder = Convert.ToInt32(file.ReadLine());                 
                }
                else if (line.Contains("Left Elbow"))
                {
                    ja.leftElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Left Hip"))
                {
                    ja.leftHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Left Knee"))
                {
                    ja.leftKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Elbow"))
                {
                    ja.rightElbow = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Hip"))
                {
                    ja.rightHip = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("Right Knee"))
                {
                    ja.rightKnee = Convert.ToInt32(file.ReadLine());
                }
                else if (line.Contains("End"))
                {
                    loadedSkeleton.Add(ja);
                    ja = new JointAngles();
                }
            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        void initializeHoverChecker()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //Timer to check for hand positions
            dispatcherTimer.Tick += new EventHandler(checkHands);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            dispatcherTimer.Start();

            hoverTimer = new Stopwatch();
        }

        void initializeTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            
            //Timer for Pseudo Heart Rate
            dispatcherTimer.Tick += new EventHandler(heartRate);
            dispatcherTimer.Interval = new TimeSpan(0,0,1);
            dispatcherTimer.Start();

            stopwatch = new Stopwatch();
            stopwatch.Start();
            r = new Random();
        }

        /**
         * Starts playing back the skeleton and matching it against the user
         */
        private void startPlaybackSkeleton()
        {
            skeletonMatcherTimer = new System.Windows.Threading.DispatcherTimer();
            skeletonMatcherTimer.Tick += new EventHandler(matchSkeleton);
            skeletonMatcherTimer.Interval = new TimeSpan(0, 0, 1);
            skeletonMatcherTimer.Start();
        }

        /**
         * Checks if the loaded skeleton data matches the users skeleton 
         * periodically every second
         */
        private void matchSkeleton(object sender, EventArgs e)
        {
            //Get amount of seconds passed in video        
            //Check if loaded skeleton at this point matches the users current data
            //comparisonStopWatch = new Stopwatch();
            TimeSpan timePassed = comparisonStopWatch.Elapsed;
            int secondsPassed = timePassed.Seconds;
            Debug.WriteLine(secondsPassed.ToString());


            //Check if loaded skeleton at this point matches the users current data within +- 1 second of the video
            try
            {
                if (SkeletonMatchesCloselyEnough(loadedSkeleton.ElementAt(secondsPassed)) || SkeletonMatchesCloselyEnough(loadedSkeleton.ElementAt(secondsPassed - 1)) || SkeletonMatchesCloselyEnough(loadedSkeleton.ElementAt(secondsPassed + 1)))
                {
                    points.Text = "GOOD!";
                }
                else
                {
                    points.Text = "BAD!";
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                //Do nothing
            }
        }

        /**
         * Checks if the skeleton data matches within certain
         * angles to the users
         */
        private bool SkeletonMatchesCloselyEnough(JointAngles ja)
        {
            if (first == null)
            {
                return false;
            }
            int leftElbow = AngleBetweenJoints(first.Joints[JointType.HandLeft], first.Joints[JointType.ElbowLeft], first.Joints[JointType.ShoulderLeft]);
            int rightElbow = AngleBetweenJoints(first.Joints[JointType.HandRight], first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight]);
            int rightShoulder = AngleBetweenJoints(first.Joints[JointType.ElbowRight], first.Joints[JointType.ShoulderRight], first.Joints[JointType.ShoulderCenter]);          
            int leftHip = AngleBetweenJoints(first.Joints[JointType.ShoulderLeft], first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft]);
            int rightHip = AngleBetweenJoints(first.Joints[JointType.ShoulderRight], first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight]); 
            int leftKnee = AngleBetweenJoints(first.Joints[JointType.HipLeft], first.Joints[JointType.KneeLeft], first.Joints[JointType.FootLeft]);
            int rightKnee = AngleBetweenJoints(first.Joints[JointType.HipRight], first.Joints[JointType.KneeRight], first.Joints[JointType.FootRight]);


            //Check if patient's joint angle is +- 20 degrees of the exercise
            if (leftElbow < (ja.leftElbow - 30) || leftElbow > (ja.leftElbow + 30))
            {
                suggestionBlock.Text = "Fix your left elbow!";
                return false;
            }
            else if (rightElbow < (ja.rightElbow - 30) || rightElbow > (ja.rightElbow + 30))
            {
                suggestionBlock.Text = "Fix your right elbow!";
                return false;
            }
            else if (leftHip < (ja.leftHip - 30) || leftHip > (ja.leftHip + 30))
            {
                suggestionBlock.Text = "Fix your left hip!";
                return false;
            }
            else if (rightHip < (ja.rightHip - 30) || rightHip > (ja.rightHip + 30))
            {
                suggestionBlock.Text = "Fix your right hip!";
                return false;
            }
            else if (rightKnee < (ja.rightKnee - 30) || rightKnee > (ja.rightKnee + 30))
            {
                suggestionBlock.Text = "Fix your right knee!";
                return false;
            }
            else if (leftKnee < (ja.leftKnee - 30) || leftKnee > (ja.leftKnee + 30))
            {
                suggestionBlock.Text = "Fix your left knee!";
                return false;
            }
            else if (rightShoulder < (ja.rightShoulder - 30) || rightShoulder > (ja.rightShoulder + 30))
            {
                suggestionBlock.Text = "Fix your right shoulder!";
                return false;
            }
            else return true;

        }
        /**
         * Pause the playback of the skeleton
         */
        private void pausePlaybackSkeleton()
        {
            skeletonMatcherTimer.Stop();
        }

        /**
         * Pseudo Heart Rate Simulator
         */
        private void heartRate(object sender, EventArgs e)
        {            
            int heartRate = r.Next(50,130);
            heartRateToPoints(heartRate);            
        }

        /**
         * Checks the heart rate and converts it to points
         */
        void heartRateToPoints(int heartRate)
        {
            stopwatch.Stop();
            long elapsed = stopwatch.ElapsedMilliseconds;
            if (heartRate > 90 && heartRate < 110)
            {
                numberOfPts += elapsed;
                setPoints();
            }

            var heartarrowangle = heartarrow.RenderTransform as RotateTransform;
            heartarrowangle.Angle = heartRate*3;                  
            
            stopwatch.Reset();
            stopwatch.Start();
        }

        /**
         * Sets the points to the progress bar and number of points
         */
        void setPoints()
        {            
           points.Text = numberOfPts + "Pts.";            
        }

        /**
         * Initializes the UI
         */
        void initializeUI()
        {
            numberOfPts = 0;
            points.Text = numberOfPts + "Pts.";
            videoPlaying = false;
            timerInitialized = false;
          
            //Get positions of buttons
            leftHandPos = new Rect();
            leftHandPos.Location = new Point(Canvas.GetLeft(leftHand), Canvas.GetTop(leftHand));
            leftHandPos.Size = new Size(leftHand.Width, leftHand.Height);
            rightHandPos = new Rect();
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));
            rightHandPos.Size = new Size(rightHand.Width, rightHand.Height);
            playIconPos = new Rect();
            playIconPos.Location = new Point(Canvas.GetLeft(playicon), Canvas.GetTop(playicon));
            playIconPos.Size = new Size(playicon.Width, playicon.Height);

            //Set video source for video player
           // FitnessPlayer.Source = new Uri("C:\\Users\\Public\\Videos\\Sample Videos\\Wildlife.wmv");
        }

        /**
        * Checks to see if hands are hovering over a button
        */
        private void checkHands(object sender, EventArgs e)
        {
            
            if (leftHandPos.IntersectsWith(playIconPos) || rightHandPos.IntersectsWith(playIconPos))
            {
                hoverTimer.Start();
                hoverPlay(playicon, new RoutedEventArgs());
                //Check if hand has been hovering on target for 1 second or more   
                if (hoverTimer.ElapsedMilliseconds >= 1000)
                {
                    //Presses the play button
                    btnPlay_Click(sender, new RoutedEventArgs());
                    //Resets hoverTimer
                    hoverTimer.Reset();
                }
            }
            else  //If hand is not hovering on any button.  Reset timer.
            {
                hoverTimer.Reset();                
                leavePlay(playicon, new RoutedEventArgs());
            }                       
        }


        /**
         * Highlights play when the mouse or hand hovers over them
         */
        private void hoverPlay(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playicon.Name))
            {
                if (!videoPlaying)
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 1;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 0;                    
                }
                else
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 1;
                }
            }
            
        }

        /**
         * Stops highlighting the images when the mouse leaves
         */
        private void leavePlay(object sender, RoutedEventArgs e)
        {
            Image i = (Image)sender;

            if (i.Name.Equals(playicon.Name))
            {
                if (!videoPlaying)
                {
                    playicon.Opacity = 1;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 0;
                    hoverpauseicon.Opacity = 0;
                }
                else
                {
                    playicon.Opacity = 0;
                    hoverplayicon.Opacity = 0;
                    pauseicon.Opacity = 1;
                    hoverpauseicon.Opacity = 0;
                }
            }
        }



        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor old = (KinectSensor)e.OldValue;

            StopKinect(old);

            ksensor = (KinectSensor)e.NewValue;

            if (ksensor == null)
            {
                return;
            }

            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.5f
            };
            //sensor.SkeletonStream.Enable(parameters);

            ksensor.SkeletonStream.Enable();

            ksensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
            ksensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            ksensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);

            skeletonDisplayManager = new SkeletonDisplayManager(ksensor, MainCanvas);
            try
            {
                ksensor.Start();
                ksensor = ksensor;
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
            ScalePosition(leftHand, first.Joints[JointType.HandLeft]);
            ScalePosition(rightHand, first.Joints[JointType.HandRight]);
            leftHandPos.Location = new Point(Canvas.GetLeft(leftHand), Canvas.GetTop(leftHand));
            rightHandPos.Location = new Point(Canvas.GetLeft(rightHand), Canvas.GetTop(rightHand));

            
        }

        //Returns the angle between the joints
        public static int AngleBetweenJoints(Joint j1, Joint j2, Joint j3)
        {
            double Angulo = 0;
            double shrhX = j1.Position.X - j2.Position.X;
            double shrhY = j1.Position.Y - j2.Position.Y;
            double shrhZ = j1.Position.Z - j2.Position.Z;
            double hsl = vectorNorm(shrhX, shrhY, shrhZ);
            double unrhX = j3.Position.X - j2.Position.X;
            double unrhY = j3.Position.Y - j2.Position.Y;
            double unrhZ = j3.Position.Z - j2.Position.Z;
            double hul = vectorNorm(unrhX, unrhY, unrhZ);
            double mhshu = shrhX * unrhX + shrhY * unrhY + shrhZ * unrhZ;
            double x = mhshu / (hul * hsl);
            if (x != Double.NaN)
            {
                if (-1 <= x && x <= 1)
                {
                    double angleRad = Math.Acos(x);
                    Angulo = angleRad * (180.0 / Math.PI);
                }
                else
                    Angulo = 0;


            }
            else
                Angulo = 0;


            return Convert.ToInt32(Angulo);

        }


        private static double vectorNorm(double x, double y, double z)
        {

            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    kinectSensorChooser1.Kinect == null)
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
                CameraPosition(leftHand, leftColorPoint);
                CameraPosition(rightHand, rightColorPoint);
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
            //Joint scaledJoint = joint.ScaleTo(1280, 720); 

            Joint scaledJoint = joint.ScaleTo(900, 800, .3f, .3f);

            Canvas.SetLeft(element, scaledJoint.Position.X);
            Canvas.SetTop(element, scaledJoint.Position.Y);

        }

        private void motorUp_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int x = ksensor.ElevationAngle;
                ksensor.ElevationAngle += 5;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Failed to move Kinect motor.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Elevation angle is out of range.");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No Kinect Attached");
            }
        }

        private void motorDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = ksensor.ElevationAngle;
                ksensor.ElevationAngle -= 5;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Failed to move Kinect motor.");
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Elevation angle is out of range.");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No Kinect Attached");
            }
        }


        private void kinectSkeletonViewer1_Unloaded(object sender, RoutedEventArgs e)
        {
            closing = true;
            comparisonStopWatch.Reset();
            StopKinect(kinectSensorChooser1.Kinect); 
        }


        /*
         * Media Player Stuff
         */
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            
             
            if (!videoPlaying)
            {               
                //kinectDisplay.Play();
                
                videoPlaying = true;
                if (!timerInitialized)
                {
                    initializeTimer();
                    timerInitialized = true;
                    startPlaybackSkeleton();
                }                
            }
            else
            {                
               // kinectDisplay.Pause();
                videoPlaying = false;
                pausePlaybackSkeleton();
            }
            
        }
        private void replayButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Title = "Select filename", Filter = "Replay files|*.replay" };

            if (openFileDialog.ShowDialog() == true)
            {
                if (replay != null)
                {
                    replay.SkeletonFrameReady -= replay_SkeletonFrameReady;
                    replay.ColorImageFrameReady -= replay_ColorImageFrameReady;
                    replay.Stop();
                    comparisonStopWatch.Stop();
                    comparisonStopWatch.Reset();
                    setPoints();
                    replayOption.Content = "Record";
                }
                Stream recordStream = File.OpenRead(openFileDialog.FileName);


                replay = new KinectReplay(recordStream);

                replay.SkeletonFrameReady += replay_SkeletonFrameReady;
                replay.ColorImageFrameReady += replay_ColorImageFrameReady;

                replay.Start();
                comparisonStopWatch = new Stopwatch();
                comparisonStopWatch.Start();
                startPlaybackSkeleton();
                //Debug.WriteLine("REPLAY");
                

            }
        }

        void ProcessFrame2(ReplayColorImageFrame frame)
        {
          //  Debug.WriteLine("COLORIMAGE");
        }

        void ProcessFrame(ReplaySkeletonFrame frame)
        {

           
           //skeletonDisplayManager.Draw(frame.Skeletons, seatedMode.IsChecked == true);
        }

        void replay_ColorImageFrameReady(object sender, ReplayColorImageFrameReadyEventArgs e)
        {
            //if (displayDepth)
            //return;

            colorManager.Update(e.ColorImageFrame);
            ProcessFrame2(e.ColorImageFrame);
            //Debug.WriteLine("replayColor");
        }

        void replay_SkeletonFrameReady(object sender, ReplaySkeletonFrameReadyEventArgs e)
        {
            ProcessFrame(e.SkeletonFrame);
          //  Debug.WriteLine("replaySkeleton");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewButton.Content = "Play";
            kinectDisplay.DataContext = colorManager;
            kinectDisplay2.DataContext = skeletonDisplayManager;

        }

        private void seatedMode_Checked_1(object sender, RoutedEventArgs e)
        {
            if (ksensor == null)
                return;

            ksensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
        }

        private void seatedMode_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (ksensor == null)
                return;

            ksensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
        }

    }
}
